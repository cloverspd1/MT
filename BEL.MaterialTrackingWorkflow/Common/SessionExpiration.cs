namespace BEL.MaterialTrackingWorkflow.Common
{
    using System.Web.Mvc;
    
    /// <summary>
    /// Session Expiration Class.
    /// </summary>
    public class SessionExpiration : ActionFilterAttribute
    {
        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          base.OnActionExecuting(filterContext);
        }
    }
}