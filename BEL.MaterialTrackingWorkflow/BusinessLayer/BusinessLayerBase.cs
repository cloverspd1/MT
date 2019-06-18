namespace BEL.MaterialTrackingWorkflow.BusinessLayer
{
    using BEL.CommonDataContract;
    using BEL.DataAccessLayer;
    using Microsoft.SharePoint.Client;
    using System;


    public  class BusinessLayerBase
    {
        /// <summary>
        /// The context
        /// </summary>
        public static ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        public static Web web = null;

        protected BusinessLayerBase()
        {
            try
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                string siteURL = helper.GetSiteURL(SiteURLs.MTSITEURL);
                if (!string.IsNullOrEmpty(siteURL))
                {
                    context = helper.CreateClientContext(siteURL);
                    web = helper.CreateWeb(context);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}