namespace BEL.MaterialTrackingWorkflow.Common
{
    using Microsoft.IdentityModel;
    using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
    using Microsoft.IdentityModel.S2S.Tokens;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.EventReceivers;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Script.Serialization;
    using AudienceRestriction = Microsoft.IdentityModel.Tokens.AudienceRestriction;
    using AudienceUriValidationFailedException = Microsoft.IdentityModel.Tokens.AudienceUriValidationFailedException;
    using SecurityTokenHandlerConfiguration = Microsoft.IdentityModel.Tokens.SecurityTokenHandlerConfiguration;
    using X509SigningCredentials = Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials;

    /// <summary>
    /// Token Helper
    /// </summary>
    public static class TokenHelper
    {
        #region public fields

        /// <summary>
        /// SharePoint principal.
        /// </summary>
        public const string SharePointPrincipal = "00000003-0000-0ff1-ce00-000000000000";

        /// <summary>
        /// Lifetime of HighTrust access token, 12 hours.
        /// </summary>
        public static readonly TimeSpan HighTrustAccessTokenLifetime = TimeSpan.FromHours(12.0);

        #endregion public fields

        #region public methods

        /// <summary>
        /// Retrieves the context token string from the specified request by looking for well-known parameter names in the
        /// POSTed form parameters and the querystring. Returns null if no context token is found.
        /// </summary>
        /// <param name="request">HttpRequest in which to look for a context token</param>
        /// <returns>
        /// The context token string
        /// </returns>
        public static string GetContextTokenFromRequest(HttpRequest request)
        {
            return GetContextTokenFromRequest(new HttpRequestWrapper(request));
        }

        /// <summary>
        /// Retrieves the context token string from the specified request by looking for well-known parameter names in the
        /// POSTed form parameters and the querystring. Returns null if no context token is found.
        /// </summary>
        /// <param name="request">HttpRequest in which to look for a context token</param>
        /// <returns>
        /// The context token string
        /// </returns>
        public static string GetContextTokenFromRequest(HttpRequestBase request)
        {
            if (request != null)
            {
                string[] paramNames = { "AppContext", "AppContextToken", "AccessToken", "SPAppToken" };
                foreach (string paramName in paramNames)
                {
                    if (!string.IsNullOrEmpty(request.Form[paramName]))
                    {
                        return request.Form[paramName];
                    }
                    if (!string.IsNullOrEmpty(request.QueryString[paramName]))
                    {
                        return request.QueryString[paramName];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Validate that a specified context token string is intended for this application based on the parameters
        /// specified in web.config. Parameters used from web.config used for validation include ClientId,
        /// HostedAppHostNameOverride, HostedAppHostName, ClientSecret, and Realm (if it is specified). If HostedAppHostNameOverride is present,
        /// it will be used for validation. Otherwise, if the <paramref name="appHostName" /> is not
        /// null, it is used for validation instead of the web.config's HostedAppHostName. If the token is invalid, an
        /// exception is thrown. If the token is valid, TokenHelper's static STS metadata url is updated based on the token contents
        /// and a JsonWebSecurityToken based on the context token is returned.
        /// </summary>
        /// <param name="contextTokenString">The context token to validate</param>
        /// <param name="appHostName">The URL authority, consisting of  Domain Name System (DNS) host name or IP address and the port number, to use for token audience validation.
        /// If null, HostedAppHostName web.config setting is used instead. HostedAppHostNameOverride web.config setting, if present, will be used
        /// for validation instead of <paramref name="appHostName" /> .</param>
        /// <returns>
        /// A JsonWebSecurityToken based on the context token.
        /// </returns>
        /// <exception cref="AudienceUriValidationFailedException">Audience Uri Fail</exception>
        public static SharePointContextToken ReadAndValidateContextToken(string contextTokenString, string appHostName = null)
        {
            JsonWebSecurityTokenHandler tokenHandler = CreateJsonWebSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.ReadToken(contextTokenString);
            JsonWebSecurityToken jsonToken = securityToken as JsonWebSecurityToken;
            SharePointContextToken token = SharePointContextToken.Create(jsonToken);

            string stsAuthority = (new Uri(token.SecurityTokenServiceUri)).Authority;
            int firstDot = stsAuthority.IndexOf('.');

            globalEndPointPrefix = stsAuthority.Substring(0, firstDot);
            acsHostUrl = stsAuthority.Substring(firstDot + 1);

            tokenHandler.ValidateToken(jsonToken);

            string[] acceptableAudiences;
            if (!string.IsNullOrEmpty(HostedAppHostNameOverride))
            {
                acceptableAudiences = HostedAppHostNameOverride.Split(';');
            }
            else if (appHostName == null)
            {
                acceptableAudiences = new[] { HostedAppHostName };
            }
            else
            {
                acceptableAudiences = new[] { appHostName };
            }

            bool validationSuccessful = false;
            string realm = Realm ?? token.Realm;
            foreach (var audience in acceptableAudiences)
            {
                string principal = GetFormattedPrincipal(ClientId, audience, realm);
                if (StringComparer.OrdinalIgnoreCase.Equals(token.Audience, principal))
                {
                    validationSuccessful = true;
                    break;
                }
            }

            if (!validationSuccessful)
            {
                throw new AudienceUriValidationFailedException(string.Format(CultureInfo.CurrentCulture, "\"{0}\" is not the intended audience \"{1}\"", string.Join(";", acceptableAudiences), token.Audience));
            }

            return token;
        }

        /// <summary>
        /// Retrieves an access token from ACS to call the source of the specified context token at the specified
        /// targetHost. The targetHost must be registered for the principal that sent the context token.
        /// </summary>
        /// <param name="contextToken">Context token issued by the intended access token audience</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <returns>
        /// An access token with an audience matching the context token's source
        /// </returns>
        public static OAuth2AccessTokenResponse GetAccessToken(SharePointContextToken contextToken, string targetHost)
        {
            if (contextToken != null)
            {
                string targetPrincipalName = contextToken.TargetPrincipalName;

                // Extract the refreshToken from the context token
                string refreshToken = contextToken.RefreshToken;

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return null;
                }

                string targetRealm = Realm ?? contextToken.Realm;

                return GetAccessToken(refreshToken, targetPrincipalName, targetHost, targetRealm);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Uses the specified authorization code to retrieve an access token from ACS to call the specified principal
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to exchange for access token</param>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <param name="redirectUri">Redirect URI registerd for this app</param>
        /// <returns>
        /// An access token with an audience of the target principal
        /// </returns>
        /// <exception cref="System.Net.WebException">Web Exception</exception>
        public static OAuth2AccessTokenResponse GetAccessToken(
            string authorizationCode,
            string targetPrincipalName,
            string targetHost,
            string targetRealm,
            Uri redirectUri)
        {
            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, null, targetRealm);

            // Create request for token. The RedirectUri is null here.  This will fail if redirect uri is registered
            OAuth2AccessTokenRequest oauth2Request =
                OAuth2MessageFactory.CreateAccessTokenRequestWithAuthorizationCode(
                    clientId,
                    ClientSecret,
                    authorizationCode,
                    redirectUri,
                    resource);

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();
            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Uses the specified refresh token to retrieve an access token from ACS to call the specified principal
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="refreshToken">Refresh token to exchange for access token</param>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>
        /// An access token with an audience of the target principal
        /// </returns>
        /// <exception cref="System.Net.WebException">Web Exception</exception>
        public static OAuth2AccessTokenResponse GetAccessToken(
            string refreshToken,
            string targetPrincipalName,
            string targetHost,
            string targetRealm)
        {
            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, null, targetRealm);

            OAuth2AccessTokenRequest oauth2Request = OAuth2MessageFactory.CreateAccessTokenRequestWithRefreshToken(clientId, ClientSecret, refreshToken, resource);

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();
            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Retrieves an app-only access token from ACS to call the specified principal
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>
        /// An access token with an audience of the target principal
        /// </returns>
        /// <exception cref="System.Net.WebException">Web Exception</exception>
        public static OAuth2AccessTokenResponse GetAppOnlyAccessToken(
            string targetPrincipalName,
            string targetHost,
            string targetRealm)
        {
            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, HostedAppHostName, targetRealm);

            OAuth2AccessTokenRequest oauth2Request = OAuth2MessageFactory.CreateAccessTokenRequestWithClientCredentials(clientId, ClientSecret, resource);
            oauth2Request.Resource = resource;

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();

            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Creates a client context based on the properties of a remote event receiver
        /// </summary>
        /// <param name="properties">Properties of a remote event receiver</param>
        /// <returns>
        /// A ClientContext ready to call the web where the event originated
        /// </returns>
        public static ClientContext CreateRemoteEventReceiverClientContext(SPRemoteEventProperties properties)
        {
            if (properties != null)
            {
                Uri sharepointUrl;
                if (properties.ListEventProperties != null)
                {
                    sharepointUrl = new Uri(properties.ListEventProperties.WebUrl);
                }
                else if (properties.ItemEventProperties != null)
                {
                    sharepointUrl = new Uri(properties.ItemEventProperties.WebUrl);
                }
                else if (properties.WebEventProperties != null)
                {
                    sharepointUrl = new Uri(properties.WebEventProperties.FullUrl);
                }
                else
                {
                    return null;
                }

                if (IsHighTrustApp())
                {
                    return GetS2SClientContextWithWindowsIdentity(sharepointUrl, null);
                }

                return CreateAcsClientContextForUrl(properties, sharepointUrl);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a client context based on the properties of an app event
        /// </summary>
        /// <param name="properties">Properties of an app event</param>
        /// <param name="useAppWeb">True to target the app web, false to target the host web</param>
        /// <returns>
        /// A ClientContext ready to call the app web or the parent web
        /// </returns>
        public static ClientContext CreateAppEventClientContext(SPRemoteEventProperties properties, bool useAppWeb)
        {
            if (properties == null || properties.AppEventProperties == null)
            {
                return null;
            }

            Uri sharepointUrl = useAppWeb ? properties.AppEventProperties.AppWebFullUrl : properties.AppEventProperties.HostWebFullUrl;
            if (IsHighTrustApp())
            {
                return GetS2SClientContextWithWindowsIdentity(sharepointUrl, null);
            }

            return CreateAcsClientContextForUrl(properties, sharepointUrl);
        }

        /// <summary>
        /// Retrieves an access token from ACS using the specified authorization code, and uses that access token to
        /// create a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="authorizationCode">Authorization code to use when retrieving the access token from ACS</param>
        /// <param name="redirectUri">Redirect URI registerd for this app</param>
        /// <returns>
        /// A ClientContext ready to call targetUrl with a valid access token
        /// </returns>
        public static ClientContext GetClientContextWithAuthorizationCode(
            string targetUrl,
            string authorizationCode,
            Uri redirectUri)
        {
            return GetClientContextWithAuthorizationCode(targetUrl, SharePointPrincipal, authorizationCode, GetRealmFromTargetUrl(new Uri(targetUrl)), redirectUri);
        }

        /// <summary>
        /// Retrieves an access token from ACS using the specified authorization code, and uses that access token to
        /// create a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="targetPrincipalName">Name of the target SharePoint principal</param>
        /// <param name="authorizationCode">Authorization code to use when retrieving the access token from ACS</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <param name="redirectUri">Redirect URI registerd for this app</param>
        /// <returns>
        /// A ClientContext ready to call targetUrl with a valid access token
        /// </returns>
        public static ClientContext GetClientContextWithAuthorizationCode(
            string targetUrl,
            string targetPrincipalName,
            string authorizationCode,
            string targetRealm,
            Uri redirectUri)
        {
            Uri targetUri = new Uri(targetUrl);

            string accessToken =
                GetAccessToken(authorizationCode, targetPrincipalName, targetUri.Authority, targetRealm, redirectUri).AccessToken;

            return GetClientContextWithAccessToken(targetUrl, accessToken);
        }

        /// <summary>
        /// Uses the specified access token to create a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="accessToken">Access token to be used when calling the specified targetUrl</param>
        /// <returns>
        /// A ClientContext ready to call targetUrl with the specified access token
        /// </returns>
        public static ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        {
            using (ClientContext clientContext = new ClientContext(targetUrl))
            {
                string spUserName = ConfigurationManager.AppSettings["spUserName"];
                string spPassword = ConfigurationManager.AppSettings["spPassword"];
                string spDomain = ConfigurationManager.AppSettings["spDomain"];
                clientContext.Credentials = new NetworkCredential(spUserName, spPassword, spDomain);
                return clientContext;
            }
        }

        /// <summary>
        /// Retrieves an access token from ACS using the specified context token, and uses that access token to create
        /// a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="contextTokenString">Context token received from the target SharePoint site</param>
        /// <param name="appHostUrl">Url authority of the hosted app.  If this is null, the value in the HostedAppHostName
        /// of web.config will be used instead</param>
        /// <returns>
        /// A ClientContext ready to call targetUrl with a valid access token
        /// </returns>
        public static ClientContext GetClientContextWithContextToken(
            string targetUrl,
            string contextTokenString,
            string appHostUrl)
        {
            SharePointContextToken contextToken = ReadAndValidateContextToken(contextTokenString, appHostUrl);

            Uri targetUri = new Uri(targetUrl);

            string accessToken = GetAccessToken(contextToken, targetUri.Authority).AccessToken;

            return GetClientContextWithAccessToken(targetUrl, accessToken);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request consent and get back
        /// an authorization code.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="scope">Space-delimited permissions to request from the SharePoint site in "shorthand" format
        /// (e.g. "Web.Read Site.Write")</param>
        /// <returns>
        /// Url of the SharePoint site's OAuth authorization page
        /// </returns>
        public static string GetAuthorizationUrl(string contextUrl, string scope)
        {
            return string.Format(
                "{0}{1}?IsDlg=1&client_id={2}&scope={3}&response_type=code",
                EnsureTrailingSlash(contextUrl),
                AuthorizationPage,
                ClientId,
                scope);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request consent and get back
        /// an authorization code.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="scope">Space-delimited permissions to request from the SharePoint site in "shorthand" format
        /// (e.g. "Web.Read Site.Write")</param>
        /// <param name="redirectUri">Uri to which SharePoint should redirect the browser to after consent is
        /// granted</param>
        /// <returns>
        /// Url of the SharePoint site's OAuth authorization page
        /// </returns>
        public static string GetAuthorizationUrl(string contextUrl, string scope, string redirectUri)
        {
            return string.Format(
                "{0}{1}?IsDlg=1&client_id={2}&scope={3}&response_type=code&redirect_uri={4}",
                EnsureTrailingSlash(contextUrl),
                AuthorizationPage,
                ClientId,
                scope,
                redirectUri);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request a new context token.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="redirectUri">Uri to which SharePoint should redirect the browser to with a context token</param>
        /// <returns>
        /// Url of the SharePoint site's context token redirect page
        /// </returns>
        public static string GetAppContextTokenRequestUrl(string contextUrl, string redirectUri)
        {
            return string.Format(
                "{0}{1}?client_id={2}&redirect_uri={3}",
                EnsureTrailingSlash(contextUrl),
                RedirectPage,
                ClientId,
                redirectUri);
        }

        /// <summary>
        /// Retrieves an S2S access token signed by the application's private certificate on behalf of the specified
        /// WindowsIdentity and intended for the SharePoint at the targetApplicationUri. If no Realm is specified in
        /// web.config, an auth challenge will be issued to the targetApplicationUri to discover it.
        /// </summary>
        /// <param name="targetApplicationUri">Url of the target SharePoint site</param>
        /// <param name="identity">Windows identity of the user on whose behalf to create the access token</param>
        /// <returns>
        /// An access token with an audience of the target principal
        /// </returns>
        public static string GetS2SAccessTokenWithWindowsIdentity(
            Uri targetApplicationUri,
            WindowsIdentity identity)
        {
            if (targetApplicationUri != null)
            {
                string realm = string.IsNullOrEmpty(Realm) ? GetRealmFromTargetUrl(targetApplicationUri) : Realm;

                JsonWebTokenClaim[] claims = identity != null ? GetClaimsWithWindowsIdentity(identity) : null;

                return GetS2SAccessTokenWithClaims(targetApplicationUri.Authority, realm, claims);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves an S2S client context with an access token signed by the application's private certificate on
        /// behalf of the specified WindowsIdentity and intended for application at the targetApplicationUri using the
        /// targetRealm. If no Realm is specified in web.config, an auth challenge will be issued to the
        /// targetApplicationUri to discover it.
        /// </summary>
        /// <param name="targetApplicationUri">Url of the target SharePoint site</param>
        /// <param name="identity">Windows identity of the user on whose behalf to create the access token</param>
        /// <returns>
        /// A ClientContext using an access token with an audience of the target application
        /// </returns>
        public static ClientContext GetS2SClientContextWithWindowsIdentity(
            Uri targetApplicationUri,
            WindowsIdentity identity)
        {
            if (targetApplicationUri != null)
            {
                string realm = string.IsNullOrEmpty(Realm) ? GetRealmFromTargetUrl(targetApplicationUri) : Realm;

                JsonWebTokenClaim[] claims = identity != null ? GetClaimsWithWindowsIdentity(identity) : null;

                string accessToken = GetS2SAccessTokenWithClaims(targetApplicationUri.Authority, realm, claims);

                return GetClientContextWithAccessToken(targetApplicationUri.ToString(), accessToken);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get authentication realm from SharePoint
        /// </summary>
        /// <param name="targetApplicationUri">Url of the target SharePoint site</param>
        /// <returns>
        /// String representation of the realm GUID
        /// </returns>
        public static string GetRealmFromTargetUrl(Uri targetApplicationUri)
        {
            WebRequest request = WebRequest.Create(targetApplicationUri + "/_vti_bin/client.svc");
            request.Headers.Add("Authorization: Bearer ");

            try
            {
                using (request.GetResponse())
                {
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    return null;
                }

                string bearerResponseHeader = e.Response.Headers["WWW-Authenticate"];
                if (string.IsNullOrEmpty(bearerResponseHeader))
                {
                    return null;
                }

                const string Bearer = "Bearer realm=\"";
                int bearerIndex = bearerResponseHeader.IndexOf(Bearer, StringComparison.Ordinal);
                if (bearerIndex < 0)
                {
                    return null;
                }

                int realmIndex = bearerIndex + Bearer.Length;

                if (bearerResponseHeader.Length >= realmIndex + 36)
                {
                    string targetRealm = bearerResponseHeader.Substring(realmIndex, 36);

                    Guid realmGuid;

                    if (Guid.TryParse(targetRealm, out realmGuid))
                    {
                        return targetRealm;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if this is a high trust app.
        /// </summary>
        /// <returns>
        /// True if this is a high trust app.
        /// </returns>
        public static bool IsHighTrustApp()
        {
            return SigningCredentials != null;
        }

        /// <summary>
        /// Ensures the trailing slash.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>ensure trailing slash</returns>
        public static string EnsureTrailingSlash(string url)
        {
            if (!string.IsNullOrEmpty(url) && url[url.Length - 1] != '/')
            {
                return url + "/";
            }

            return url;
        }

        #endregion

        #region private fields
        // Configuration Constants

        /// <summary>
        /// The authorization page
        /// </summary>
        private const string AuthorizationPage = "_layouts/15/OAuthAuthorize.aspx";

        /// <summary>
        /// The redirect page
        /// </summary>
        private const string RedirectPage = "_layouts/15/AppRedirect.aspx";

        /// <summary>
        /// The acs principal name
        /// </summary>
        private const string AcsPrincipalName = "00000001-0000-0000-c000-000000000000";

        /// <summary>
        /// The acs metadata end point relative URL
        /// </summary>
        private const string AcsMetadataEndPointRelativeUrl = "metadata/json/1";

        /// <summary>
        /// The s2 s protocol
        /// </summary>
        private const string S2SProtocol = "OAuth2";

        /// <summary>
        /// The delegation issuance
        /// </summary>
        private const string DelegationIssuance = "DelegationIssuance1.0";

        /// <summary>
        /// The name identifier claim type
        /// </summary>
        private const string NameIdentifierClaimType = JsonWebTokenConstants.ReservedClaims.NameIdentifier;

        /// <summary>
        /// The trusted for impersonation claim type
        /// </summary>
        private const string TrustedForImpersonationClaimType = "trustedfordelegation";

        /// <summary>
        /// The actor token claim type
        /// </summary>
        private const string ActorTokenClaimType = JsonWebTokenConstants.ReservedClaims.ActorToken;

        // Environment Constants

        /// <summary>
        /// The global end point prefix
        /// </summary>
        private static string globalEndPointPrefix = "accounts";

        /// <summary>
        /// The acs host URL
        /// </summary>
        private static string acsHostUrl = "accesscontrol.windows.net";

        // Hosted app configuration

        /// <summary>
        /// The client identifier
        /// </summary>
        private static readonly string ClientId = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientId")) ? WebConfigurationManager.AppSettings.Get("HostedAppName") : WebConfigurationManager.AppSettings.Get("ClientId");

        /// <summary>
        /// The issuer identifier
        /// </summary>
        private static readonly string IssuerId = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("IssuerId")) ? ClientId : WebConfigurationManager.AppSettings.Get("IssuerId");

        /// <summary>
        /// The hosted application host name override
        /// </summary>
        private static readonly string HostedAppHostNameOverride = WebConfigurationManager.AppSettings.Get("HostedAppHostNameOverride");

        /// <summary>
        /// The hosted application host name
        /// </summary>
        private static readonly string HostedAppHostName = WebConfigurationManager.AppSettings.Get("HostedAppHostName");

        /// <summary>
        /// The client secret
        /// </summary>
        private static readonly string ClientSecret = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientSecret")) ? WebConfigurationManager.AppSettings.Get("HostedAppSigningKey") : WebConfigurationManager.AppSettings.Get("ClientSecret");

        /// <summary>
        /// The secondary client secret
        /// </summary>
        private static readonly string SecondaryClientSecret = WebConfigurationManager.AppSettings.Get("SecondaryClientSecret");

        /// <summary>
        /// The realm
        /// </summary>
        private static readonly string Realm = WebConfigurationManager.AppSettings.Get("Realm");

        /// <summary>
        /// The service namespace
        /// </summary>
        private static readonly string ServiceNamespace = WebConfigurationManager.AppSettings.Get("Realm");

        /// <summary>
        /// The client signing certificate path
        /// </summary>
        private static readonly string ClientSigningCertificatePath = WebConfigurationManager.AppSettings.Get("ClientSigningCertificatePath");

        /// <summary>
        /// The client signing certificate password
        /// </summary>
        private static readonly string ClientSigningCertificatePassword = WebConfigurationManager.AppSettings.Get("ClientSigningCertificatePassword");

        /// <summary>
        /// The client certificate
        /// </summary>
        private static readonly X509Certificate2 ClientCertificate = (string.IsNullOrEmpty(ClientSigningCertificatePath) || string.IsNullOrEmpty(ClientSigningCertificatePassword)) ? null : new X509Certificate2(ClientSigningCertificatePath, ClientSigningCertificatePassword);

        /// <summary>
        /// The signing credentials
        /// </summary>
        private static readonly X509SigningCredentials SigningCredentials = (ClientCertificate == null) ? null : new X509SigningCredentials(ClientCertificate, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);
        #endregion

        #region private methods

        /// <summary>
        /// Creates the acs client context for URL.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="sharepointUrl">The sharepoint URL.</param>
        /// <returns>Client Context</returns>
        private static ClientContext CreateAcsClientContextForUrl(SPRemoteEventProperties properties, Uri sharepointUrl)
        {
            string contextTokenString = properties.ContextToken;

            if (string.IsNullOrEmpty(contextTokenString))
            {
                return null;
            }

            SharePointContextToken contextToken = ReadAndValidateContextToken(contextTokenString, OperationContext.Current.IncomingMessageHeaders.To.Host);
            string accessToken = GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;

            return GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
        }

        /// <summary>
        /// Gets the acs metadata endpoint URL.
        /// </summary>
        /// <returns>Combined Path</returns>
        private static string GetAcsMetadataEndpointUrl()
        {
            return Path.Combine(GetAcsGlobalEndpointUrl(), AcsMetadataEndPointRelativeUrl);
        }

        /// <summary>
        /// Gets the formatted principal.
        /// </summary>
        /// <param name="principalName">Name of the principal.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="realm">The realm.</param>
        /// <returns>Formatted Principal</returns>
        private static string GetFormattedPrincipal(string principalName, string hostName, string realm)
        {
            if (!string.IsNullOrEmpty(hostName))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}@{2}", principalName, hostName, realm);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", principalName, realm);
        }

        /// <summary>
        /// Gets the name of the acs principal.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>ACS Principal Name</returns>
        private static string GetAcsPrincipalName(string realm)
        {
            return GetFormattedPrincipal(AcsPrincipalName, new Uri(GetAcsGlobalEndpointUrl()).Host, realm);
        }

        /// <summary>
        /// Gets the acs global endpoint URL.
        /// </summary>
        /// <returns>ACS Global Endpoint URL</returns>
        private static string GetAcsGlobalEndpointUrl()
        {
            return string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}/", globalEndPointPrefix, acsHostUrl);
        }

        /// <summary>
        /// Creates the json web security token handler.
        /// </summary>
        /// <returns>Json Web Security Token</returns>
        private static JsonWebSecurityTokenHandler CreateJsonWebSecurityTokenHandler()
        {
            JsonWebSecurityTokenHandler handler = new JsonWebSecurityTokenHandler();
            handler.Configuration = new SecurityTokenHandlerConfiguration();
            handler.Configuration.AudienceRestriction = new AudienceRestriction(AudienceUriMode.Never);
            handler.Configuration.CertificateValidator = X509CertificateValidator.None;

            List<byte[]> securityKeys = new List<byte[]>();
            securityKeys.Add(Convert.FromBase64String(ClientSecret));
            if (!string.IsNullOrEmpty(SecondaryClientSecret))
            {
                securityKeys.Add(Convert.FromBase64String(SecondaryClientSecret));
            }

            List<SecurityToken> securityTokens = new List<SecurityToken>();
            securityTokens.Add(new MultipleSymmetricKeySecurityToken(securityKeys));

            handler.Configuration.IssuerTokenResolver =
                SecurityTokenResolver.CreateDefaultSecurityTokenResolver(
                new ReadOnlyCollection<SecurityToken>(securityTokens),
                false);
            SymmetricKeyIssuerNameRegistry issuerNameRegistry = new SymmetricKeyIssuerNameRegistry();
            foreach (byte[] securitykey in securityKeys)
            {
                issuerNameRegistry.AddTrustedIssuer(securitykey, GetAcsPrincipalName(ServiceNamespace));
            }
            handler.Configuration.IssuerNameRegistry = issuerNameRegistry;
            return handler;
        }

        /// <summary>
        /// Gets the s2 s access token with claims.
        /// </summary>
        /// <param name="targetApplicationHostName">Name of the target application host.</param>
        /// <param name="targetRealm">The target realm.</param>
        /// <param name="claims">The claims.</param>
        /// <returns>S2S Access Token</returns>
        private static string GetS2SAccessTokenWithClaims(
            string targetApplicationHostName,
            string targetRealm,
            IEnumerable<JsonWebTokenClaim> claims)
        {
            return IssueToken(
                ClientId,
                IssuerId,
                targetRealm,
                SharePointPrincipal,
                targetRealm,
                targetApplicationHostName,
                true,
                claims,
                claims == null);
        }

        /// <summary>
        /// Gets the claims with windows identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>list of JSON Claims</returns>
        private static JsonWebTokenClaim[] GetClaimsWithWindowsIdentity(WindowsIdentity identity)
        {
            JsonWebTokenClaim[] claims = new JsonWebTokenClaim[]
            {
                new JsonWebTokenClaim(NameIdentifierClaimType, identity.User.Value.ToLower()),
                new JsonWebTokenClaim("nii", "urn:office:idp:activedirectory")
            };
            return claims;
        }

        /// <summary>
        /// Issues the token.
        /// </summary>
        /// <param name="sourceApplication">The source application.</param>
        /// <param name="issuerApplication">The issuer application.</param>
        /// <param name="sourceRealm">The source realm.</param>
        /// <param name="targetApplication">The target application.</param>
        /// <param name="targetRealm">The target realm.</param>
        /// <param name="targetApplicationHostName">Name of the target application host.</param>
        /// <param name="trustedForDelegation">if set to <c>true</c> [trusted for delegation].</param>
        /// <param name="claims">The claims.</param>
        /// <param name="appOnly">if set to <c>true</c> [application only].</param>
        /// <returns>Issue Token</returns>
        /// <exception cref="System.InvalidOperationException">SigningCredentials was not initialized</exception>
        private static string IssueToken(
            string sourceApplication,
            string issuerApplication,
            string sourceRealm,
            string targetApplication,
            string targetRealm,
            string targetApplicationHostName,
            bool trustedForDelegation,
            IEnumerable<JsonWebTokenClaim> claims,
            bool appOnly = false)
        {
            if (null == SigningCredentials)
            {
                throw new InvalidOperationException("SigningCredentials was not initialized");
            }

            string issuer = string.IsNullOrEmpty(sourceRealm) ? issuerApplication : string.Format("{0}@{1}", issuerApplication, sourceRealm);
            string nameid = string.IsNullOrEmpty(sourceRealm) ? sourceApplication : string.Format("{0}@{1}", sourceApplication, sourceRealm);
            string audience = string.Format("{0}/{1}@{2}", targetApplication, targetApplicationHostName, targetRealm);

            List<JsonWebTokenClaim> actorClaims = new List<JsonWebTokenClaim>();
            actorClaims.Add(new JsonWebTokenClaim(JsonWebTokenConstants.ReservedClaims.NameIdentifier, nameid));
            if (trustedForDelegation && !appOnly)
            {
                actorClaims.Add(new JsonWebTokenClaim(TrustedForImpersonationClaimType, "true"));
            }

            // Create token
            JsonWebSecurityToken actorToken = new JsonWebSecurityToken(
                issuer: issuer,
                audience: audience,
                validFrom: DateTime.UtcNow,
                validTo: DateTime.UtcNow.Add(HighTrustAccessTokenLifetime),
                signingCredentials: SigningCredentials,
                claims: actorClaims);

            string actorTokenString = new JsonWebSecurityTokenHandler().WriteTokenAsString(actorToken);

            if (appOnly)
            {
                // App-only token is the same as actor token for delegated case
                return actorTokenString;
            }

            List<JsonWebTokenClaim> outerClaims = null == claims ? new List<JsonWebTokenClaim>() : new List<JsonWebTokenClaim>(claims);
            outerClaims.Add(new JsonWebTokenClaim(ActorTokenClaimType, actorTokenString));

            JsonWebSecurityToken jsonToken = new JsonWebSecurityToken(
                nameid, // outer token issuer should match actor token nameid
                audience,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(HighTrustAccessTokenLifetime),
                outerClaims);

            string accessToken = new JsonWebSecurityTokenHandler().WriteTokenAsString(jsonToken);

            return accessToken;
        }

        #endregion

        #region AcsMetadataParser

        // This class is used to get MetaData document from the global STS endpoint. It contains
        // methods to parse the MetaData document and get endpoints and STS certificate.

        /// <summary>
        /// ACS Meta Data Parser
        /// </summary>
        public static class AcsMetadataParser
        {
            /// <summary>
            /// Gets the acs signing cert.
            /// </summary>
            /// <param name="realm">The realm.</param>
            /// <returns>ACS Signing Certificate</returns>
            /// <exception cref="System.Exception">Metadata document does not contain ACS signing certificate.</exception>
            public static X509Certificate2 GetAcsSigningCert(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                if (null != document.Keys && document.Keys.Count > 0)
                {
                    JsonKey signingKey = document.Keys[0];

                    if (null != signingKey && null != signingKey.KeyValue)
                    {
                        return new X509Certificate2(Encoding.UTF8.GetBytes(signingKey.KeyValue.Value));
                    }
                }

                throw new Exception("Metadata document does not contain ACS signing certificate.");
            }

            /// <summary>
            /// Gets the delegation service URL.
            /// </summary>
            /// <param name="realm">The realm.</param>
            /// <returns>Delegation Service URL</returns>
            /// <exception cref="System.Exception">Metadata document does not contain Delegation Service endpoint Url</exception>
            public static string GetDelegationServiceUrl(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                JsonEndpoint delegationEndpoint = document.Endpoints.SingleOrDefault(e => e.Protocol == DelegationIssuance);

                if (null != delegationEndpoint)
                {
                    return delegationEndpoint.Location;
                }
                throw new Exception("Metadata document does not contain Delegation Service endpoint Url");
            }

            /// <summary>
            /// Gets the STS URL.
            /// </summary>
            /// <param name="realm">The realm.</param>
            /// <returns>STS Url</returns>
            /// <exception cref="System.Exception">Metadata document does not contain STS endpoint url</exception>
            public static string GetStsUrl(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                JsonEndpoint s2sEndpoint = document.Endpoints.SingleOrDefault(e => e.Protocol == S2SProtocol);

                if (null != s2sEndpoint)
                {
                    return s2sEndpoint.Location;
                }

                throw new Exception("Metadata document does not contain STS endpoint url");
            }

            /// <summary>
            /// Gets the metadata document.
            /// </summary>
            /// <param name="realm">The realm.</param>
            /// <returns>JSON Meta Data Document</returns>
            /// <exception cref="System.Exception">No metadata document found at the global endpoint  + acsMetadataEndpointUrlWithRealm</exception>
            private static JsonMetadataDocument GetMetadataDocument(string realm)
            {
                string acsMetadataEndpointUrlWithRealm = string.Format(CultureInfo.InvariantCulture, "{0}?realm={1}", GetAcsMetadataEndpointUrl(), realm);
                byte[] acsMetadata;
                using (WebClient webClient = new WebClient())
                {
                    acsMetadata = webClient.DownloadData(acsMetadataEndpointUrlWithRealm);
                }
                string jsonResponseString = Encoding.UTF8.GetString(acsMetadata);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonMetadataDocument document = serializer.Deserialize<JsonMetadataDocument>(jsonResponseString);

                if (null == document)
                {
                    throw new Exception("No metadata document found at the global endpoint " + acsMetadataEndpointUrlWithRealm);
                }

                return document;
            }

            /// <summary>
            /// JSON Meta Data Document
            /// </summary>
            public class JsonMetadataDocument
            {
                /// <summary>
                /// Gets or sets the name of the service.
                /// </summary>
                /// <value>
                /// The name of the service.
                /// </value>
                public string ServiceName { get; set; }

                /// <summary>
                /// Gets or sets the endpoints.
                /// </summary>
                /// <value>
                /// The endpoints.
                /// </value>
                public List<JsonEndpoint> Endpoints { get; set; }

                /// <summary>
                /// Gets or sets the keys.
                /// </summary>
                /// <value>
                /// The keys.
                /// </value>
                public List<JsonKey> Keys { get; set; }
            }

            /// <summary>
            /// JSON End Point
            /// </summary>
            public class JsonEndpoint
            {
                /// <summary>
                /// Gets or sets the location.
                /// </summary>
                /// <value>
                /// The location.
                /// </value>
                public string Location { get; set; }

                /// <summary>
                /// Gets or sets the protocol.
                /// </summary>
                /// <value>
                /// The protocol.
                /// </value>
                public string Protocol { get; set; }

                /// <summary>
                /// Gets or sets the usage.
                /// </summary>
                /// <value>
                /// The usage.
                /// </value>
                public string Usage { get; set; }
            }

            /// <summary>
            /// JSON Key Value
            /// </summary>
            public class JsonKeyValue
            {
                /// <summary>
                /// Gets or sets the type.
                /// </summary>
                /// <value>
                /// The type.
                /// </value>
                public string Type { get; set; }

                /// <summary>
                /// Gets or sets the value.
                /// </summary>
                /// <value>
                /// The value.
                /// </value>
                public string Value { get; set; }
            }

            /// <summary>
            /// JSON Key
            /// </summary>
            public class JsonKey
            {
                /// <summary>
                /// Gets or sets the usage.
                /// </summary>
                /// <value>
                /// The usage.
                /// </value>
                public string Usage { get; set; }

                /// <summary>
                /// Gets or sets the key value.
                /// </summary>
                /// <value>
                /// The key value.
                /// </value>
                public JsonKeyValue KeyValue { get; set; }
            }
        }

        #endregion
    }

    /// <summary>
    /// A JsonWebSecurityToken generated by SharePoint to authenticate to a 3rd party application and allow callbacks using a refresh token
    /// </summary>
    public class SharePointContextToken : JsonWebSecurityToken
    {
        /// <summary>
        /// Creates the specified context token.
        /// </summary>
        /// <param name="contextToken">The context token.</param>
        /// <returns>Share Point Context Token</returns>
        public static SharePointContextToken Create(JsonWebSecurityToken contextToken)
        {
            if (contextToken != null)
            {
                return new SharePointContextToken(contextToken.Issuer, contextToken.Audience, contextToken.ValidFrom, contextToken.ValidTo, contextToken.Claims);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointContextToken"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="claims">The claims.</param>
        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims)
            : base(issuer, audience, validFrom, validTo, claims)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointContextToken"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="issuerToken">The issuer token.</param>
        /// <param name="actorToken">The actor token.</param>
        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims, SecurityToken issuerToken, JsonWebSecurityToken actorToken)
            : base(issuer, audience, validFrom, validTo, claims, issuerToken, actorToken)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointContextToken"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="signingCredentials">The signing credentials.</param>
        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims, SigningCredentials signingCredentials)
            : base(issuer, audience, validFrom, validTo, claims, signingCredentials)
        {
        }

        /// <summary>
        /// Gets the name identifier.
        /// </summary>
        /// <value>
        /// The name identifier.
        /// </value>
        public string NameId
        {
            get
            {
                return GetClaimValue(this, "nameid");
            }
        }

        /// <summary>
        /// Gets the name of the target principal.
        /// </summary>
        /// <value>
        /// The name of the target principal.
        /// </value>
        public string TargetPrincipalName
        {
            get
            {
                string appctxsender = GetClaimValue(this, "appctxsender");

                if (appctxsender == null)
                {
                    return null;
                }

                return appctxsender.Split('@')[0];
            }
        }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken
        {
            get
            {
                return GetClaimValue(this, "refreshtoken");
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        public string CacheKey
        {
            get
            {
                string appctx = GetClaimValue(this, "appctx");
                if (appctx == null)
                {
                    return null;
                }

                using (ClientContext ctx = new ClientContext("http://tempuri.org"))
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)ctx.ParseObjectFromJsonString(appctx);
                    string cacheKey = (string)dict["CacheKey"];
                    return cacheKey;
                }
            }
        }

        /// <summary>
        /// Gets the security token service URI.
        /// </summary>
        /// <value>
        /// The security token service URI.
        /// </value>
        public string SecurityTokenServiceUri
        {
            get
            {
                string appctx = GetClaimValue(this, "appctx");
                if (appctx == null)
                {
                    return null;
                }

                using (ClientContext ctx = new ClientContext("http://tempuri.org"))
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)ctx.ParseObjectFromJsonString(appctx);
                    string securityTokenServiceUri = (string)dict["SecurityTokenServiceUri"];
                    return securityTokenServiceUri;
                }
            }
        }

        /// <summary>
        /// Gets the realm.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Realm
        {
            get
            {
                string aud = Audience;
                if (aud == null)
                {
                    return null;
                }

                string tokenRealm = aud.Substring(aud.IndexOf('@') + 1);

                return tokenRealm;
            }
        }

        /// <summary>
        /// Gets the claim value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claimType">Type of the claim.</param>
        /// <returns>Claim Value</returns>
        /// <exception cref="System.ArgumentNullException">JSON token</exception>
        private static string GetClaimValue(JsonWebSecurityToken token, string claimType)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            foreach (JsonWebTokenClaim claim in token.Claims)
            {
                if (StringComparer.Ordinal.Equals(claim.ClaimType, claimType))
                {
                    return claim.Value;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Represents a security token which contains multiple security keys that are generated using symmetric algorithms.
    /// </summary>
    public class MultipleSymmetricKeySecurityToken : SecurityToken
    {
        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        public MultipleSymmetricKeySecurityToken(IEnumerable<byte[]> keys)
            : this(UniqueId.CreateUniqueId(), keys)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="tokenId">The unique identifier of the security token.</param>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        /// <exception cref="System.ArgumentNullException">list of keys</exception>
        /// <exception cref="System.ArgumentException">
        /// Value cannot be a null or empty string.;tokenId
        /// or
        /// The key length must be greater then zero.;keys
        /// </exception>
        public MultipleSymmetricKeySecurityToken(string tokenId, IEnumerable<byte[]> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            if (string.IsNullOrEmpty(tokenId))
            {
                throw new ArgumentException("Value cannot be a null or empty string.", "tokenId");
            }

            foreach (byte[] key in keys)
            {
                if (key.Length <= 0)
                {
                    throw new ArgumentException("The key length must be greater then zero.", "keys");
                }
            }

            this.id = tokenId;
            this.effectiveTime = DateTime.UtcNow;
            this.securityKeys = this.CreateSymmetricSecurityKeys(keys);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public override string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Gets the cryptographic keys associated with the security token.
        /// </summary>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return this.securityKeys.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the first instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidFrom
        {
            get
            {
                return this.effectiveTime;
            }
        }

        /// <summary>
        /// Gets the last instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidTo
        {
            get
            {
                // Never expire
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the key identifier for this instance can be resolved to the specified key identifier.
        /// </summary>
        /// <param name="keyIdentifierClause">A SecurityKeyIdentifierClause to compare to this instance</param>
        /// <returns>
        /// true if keyIdentifierClause is a SecurityKeyIdentifierClause and it has the same unique identifier as the Id property; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">key Identifier Clause</exception>
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException("keyIdentifierClause");
            }

            // Since this is a symmetric token and we do not have IDs to distinguish tokens, we just check for the
            // presence of a SymmetricIssuerKeyIdentifier. The actual mapping to the issuer takes place later
            // when the key is matched to the issuer.
            if (keyIdentifierClause is SymmetricIssuerKeyIdentifierClause)
            {
                return true;
            }
            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }

        #region private members

        /// <summary>
        /// Creates the symmetric security keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>Symetric Security Keys</returns>
        private List<SecurityKey> CreateSymmetricSecurityKeys(IEnumerable<byte[]> keys)
        {
            List<SecurityKey> symmetricKeys = new List<SecurityKey>();
            foreach (byte[] key in keys)
            {
                symmetricKeys.Add(new InMemorySymmetricSecurityKey(key));
            }
            return symmetricKeys;
        }

        /// <summary>
        /// The identifier
        /// </summary>
        private string id;

        /// <summary>
        /// The effective time
        /// </summary>
        private DateTime effectiveTime;

        /// <summary>
        /// The security keys
        /// </summary>
        private List<SecurityKey> securityKeys;

        #endregion
    }
}
