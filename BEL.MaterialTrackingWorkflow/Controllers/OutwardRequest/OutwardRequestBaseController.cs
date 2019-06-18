namespace BEL.MaterialTrackingWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow;
    using BEL.MaterialTrackingWorkflow.BusinessLayer;
    using BEL.MaterialTrackingWorkflow.Models.OutwardRequest;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    public class OutwardRequestBaseController : BaseController
    { /// <summary>
        /// Get DCR Details
        /// </summary>
        /// <param name="objDict">Object Parameter</param>
        /// <returns>DCR Contract Object</returns>
        public OutwardAssmblContract GetOutwardAssembledDetails(IDictionary<string, string> objDict)
        {
            return OutwardRequestBusinessLayer.Instance.GetOutwardRequestDetails(objDict);
        }

        /// <summary>
        /// Get DCR Details
        /// </summary>
        /// <param name="objDict">Object Parameter</param>
        /// <returns>DCR Contract Object</returns>
        public OutwardSingleContract GetOutwardSingleDetails(IDictionary<string, string> objDict)
        {
            return OutwardRequestBusinessLayer.Instance.GetOutwardSingleDetails(objDict);
        }

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <typeparam name="T">dict parameter</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        protected ActionStatus SaveSection(ISection section, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();
            status = OutwardRequestBusinessLayer.Instance.SaveBySection(section, objDict);
            return status;
        }


        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <typeparam name="T">dict parameter</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        protected ActionStatus SaveBySingleSection(ISection section, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();
            status = OutwardRequestBusinessLayer.Instance.SaveBySingleSection(section, objDict);
            return status;
        }

        public List<InwardDetail> RetrieveAllInwardId(string projectName, string projectCode, string currentUserAlise)
        {
            List<InwardDetail> listdata = OutwardRequestBusinessLayer.Instance.RetrieveAllInwardId(projectName, projectCode, currentUserAlise);
            return listdata;
        }

        public List<InwardDetail> RetrieveInwardDetails(string inwardId)
        {
            List<InwardDetail> listdata = OutwardRequestBusinessLayer.Instance.RetrieveInwardDetails(inwardId);
            return listdata;
        }

        public JsonResult RetrieveInwardProjectDetails(string q, string alias)
        {
            List<InwardDetail> list = OutwardRequestBusinessLayer.Instance.RetrieveInwardProjectDetails(q, this.CurrentUser.UserId);
            if (list != null)
            {
                return this.Json((from item in list select new { id = item.ProjectCode + " - " + item.ProjectName, name = item.ProjectCode + " - " + item.ProjectName, items = item }).ToList().Distinct(), JsonRequestBehavior.AllowGet);
            }
            return this.Json("[]", JsonRequestBehavior.AllowGet);

        }
    }
}