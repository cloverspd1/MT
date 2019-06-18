namespace BEL.DataAccessLayer
{
    using Microsoft.SharePoint.Client;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using System.Net;
    using System.Security;

    /// <summary>
    /// Rest helper 
    /// </summary>
    public class RESTHelper : BELDataAccessLayer
    {
        /// <summary>
        /// Gets the data using rest.
        /// </summary>
        /// <param name="restQuery">The rest query.</param>
        /// <param name="method">The method.</param>
        /// <returns>Json Object</returns>
        public static JObject GetDataUsingRest(string restQuery, string method)
        {
            Logger.Info("Calling Service Rest Query=" + restQuery + ", method=" + method);
            HttpWebRequest endpointRequest = (HttpWebRequest)WebRequest.Create(restQuery);
            //// BELDataAccessLayer helper = new BELDataAccessLayer();
            string userName = Instance.GetConfigVariable("spUserName");
            string password = Instance.GetConfigVariable("spPassword");
           ////string domain = BELDataAccessLayer.Instance.GetConfigVariable("spDomain");
            var securePassword = new SecureString();
            foreach (var c in password)
            {
                securePassword.AppendChar(c);
            }
            var credentials = new SharePointOnlineCredentials(userName, securePassword);

            endpointRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            endpointRequest.Credentials = credentials; //new NetworkCredential(userName, password, domain);
            endpointRequest.Method = method;
            endpointRequest.Accept = "application/json;odata=verbose";
           //// endpointRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            Stream webStream = endpointResponse.GetResponseStream();
            StreamReader responseReader = new StreamReader(webStream);
            string response = responseReader.ReadToEnd();
            JObject jobj = JObject.Parse(response);
            return jobj;
        }
    }
}