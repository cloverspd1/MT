namespace BEL.MaterialTrackingWorkflow.Controllers
{
    using System.Web.Mvc;
    using System.Web.Helpers;

    /// <summary>
    /// Master Data Controller
    /// </summary>
    public class MasterController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Index View</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>
        /// error view
        /// </returns>
        public ActionResult Error(string msg)
        {
            return this.View();
        }

        /// <summary>
        /// Nots the authorize.
        /// </summary>
        /// <returns>NotAuthorize View</returns>
        public ActionResult NotAuthorize()
        {
            return this.View();
        }


        /// <summary>
        /// Get Token for CSRF
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTocken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            HttpContext.Cache[this.CurrentUser.UserEmail + "_formToken"] = formToken;
            return this.Json(cookieToken + ":" + formToken, JsonRequestBehavior.AllowGet);
        }              
    }
}