using BEL.CommonDataContract;
using BEL.DataAccessLayer;
using BEL.MaterialTrackingWorkflow.BusinessLayer;
using BEL.MaterialTrackingWorkflow.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEL.MaterialTrackingWorkflow.Controllers.Report
{
    /// <summary>
    ///Report Base Controller 
    /// </summary>
    /// <seealso cref="BEL.MaterialTrackingWorkflow.BaseController" />
    public class ReportBaseController : BaseController
    {
        /// <summary>
        /// Gets the report details.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        public List<ReportDetails> GetReportDetails(string fieldName, string fieldValue)
        {
            return CommonBusinessLayer.Instance.GetReportDetails(fieldName, fieldValue);
        }

        /// <summary>
        /// Gets the serial numbers.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns></returns>
        public JsonResult GetSerialNumbers(string q)
        {
            string data = CommonBusinessLayer.Instance.GetSerialNumbers(q);
            if (!string.IsNullOrEmpty(data))
            {
                var master = JSONHelper.Deserialize<List<NameValueData>>(data);
                return this.Json((from item in master select new { id = item.Value, name = item.Value }).ToList(), JsonRequestBehavior.AllowGet);
            }
            return this.Json("[]", JsonRequestBehavior.AllowGet);

        }
        

        /// <summary>
        /// Gets the bu list.
        /// </summary>
        /// <returns></returns>
        public List<NameValueData> GetBUList()
        {
            return CommonBusinessLayer.Instance.GetBUList();
        }

        /// <summary>
        /// Gets the material location list.
        /// </summary>
        /// <returns></returns>
        public List<NameValueData> GetMaterialLocationList()
        {
            return CommonBusinessLayer.Instance.GetMaterialLocationList();
        }
    }
}