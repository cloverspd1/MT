
using BEL.MaterialTrackingWorkflow.Models.Master;

namespace BEL.MaterialTrackingWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.DataAccessLayer;
    using BEL.MaterialTrackingWorkflow;
    using BEL.MaterialTrackingWorkflow.BusinessLayer;
    using BEL.MaterialTrackingWorkflow.Models.InwardRequest;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;


    public class InwardRequestBaseController : BaseController
    { /// <summary>
        /// Get DCR Details
        /// </summary>
        /// <param name="objDict">Object Parameter</param>
        /// <returns>DCR Contract Object</returns>
        public InwardContract GetInwardRequestDetails(IDictionary<string, string> objDict)
        {
            return InwardRequestBusinessLayer.Instance.GetInwardRequestDetails(objDict);
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
            status = InwardRequestBusinessLayer.Instance.SaveBySection(section, objDict);
            return status;
        }

        /// <summary>
        /// Gets the Vendor.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns>json object</returns>
        public JsonResult GetProject(string q)
        {
            string data = InwardRequestBusinessLayer.Instance.GetProjectMasterData(q);
            if (!string.IsNullOrEmpty(data))
            {
                var master = JSONHelper.ToObject<ProjectNameMaster>(data);
                return this.Json((from item in master.Items select new { id = item.Title, name = item.Title }).ToList(), JsonRequestBehavior.AllowGet);
            }
            return this.Json("[]", JsonRequestBehavior.AllowGet);

        }

        ///// <summary>
        ///// Gets the Vendor.
        ///// </summary>
        ///// <param name="q">The q.</param>
        ///// <returns>json object</returns>
        // [HttpGet]
        //public JsonResult GetTesters(string projectName)
        //{
        //    List<TesterMasterListItem> list = null;
        //    if (projectName != null)
        //    {
        //        list = InwardRequestBusinessLayer.Instance.GetTesters(projectName);
        //    }
        //    return this.Json(list, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// Gets the outward identifier.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <param name="outwardID">The outward identifier.</param>
        /// <returns></returns>
        //public JsonResult GetOutwardId(string q, string outwardID)
        //{
        //    List<OutwardDetail> list = InwardRequestBusinessLayer.Instance.RetrieveOutwardDetails(q);
        //    if (list != null)
        //    {
        //        if (outwardID !=null && outwardID != "")
        //        {
        //            OutwardDetail OutwardDetail = new OutwardDetail();
        //            OutwardDetail.OutwardId = outwardID;
        //            list.Add(OutwardDetail);
        //        }
        //        return this.Json((from item in list select new { id = item.OutwardId, name = item.OutwardId, items = item }).ToList(), JsonRequestBehavior.AllowGet);
        //       // return this.Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    return this.Json("[]", JsonRequestBehavior.AllowGet);

        //}
    }
}