namespace BEL.MaterialTrackingWorkflow.Controllers.Report
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Reports;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Report Controller
    /// </summary>
    /// <seealso cref="BEL.MaterialTrackingWorkflow.Controllers.Report.ReportBaseController" />
    public class ReportController : ReportBaseController
    {
        #region project wise search and export to excel
        /// <summary>
        /// Reports the search.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult ProjectWiseReportSearch(string filter)
        {
            BEL.MaterialTrackingWorkflow.Models.Reports.Report report = new BEL.MaterialTrackingWorkflow.Models.Reports.Report();
            report.ReportList = this.GetReportDetails(Constants.FilterByProjectName, filter);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ReportList", report.ReportList);
            }
            else
                return View("ProjectWiseReport", report);
        }

        /// <summary>
        /// Projects the wise export to excel.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult ProjectWiseExportToExcel(string filter)
        {
            List<ReportDetails> reportDetails = this.GetReportDetails(Constants.FilterByProjectName, filter);
            // Response.ContentType = "text/csv";
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "AllProject Report";
            }
            //Response.AddHeader("Content-Disposition", "attachment; filename='" + filter + ".csv'");
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename='" + filter + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Millisecond + ".xls'");
            return PartialView("_ReportList", reportDetails);
        }

        #endregion

        #region BU wise search and export to excel

        /// <summary>
        /// Bu the wise report search.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult BUWiseReportSearch(string filter)
        {
            BEL.MaterialTrackingWorkflow.Models.Reports.Report report = new BEL.MaterialTrackingWorkflow.Models.Reports.Report();
            report.BUList = this.GetBUList();
            report.ReportList = this.GetReportDetails(Constants.FilterByBUName, filter);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ReportList", report.ReportList);
            }
            else
                return View("BUWiseReport", report);
        }

        /// <summary>
        /// Bus the wise export to excel.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult BUWiseExportToExcel(string filter)
        {
            List<ReportDetails> reportDetails = this.GetReportDetails(Constants.FilterByBUName, filter);

            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "All Business Unit Report";
            }
            else if (filter.IndexOf(',') > 0)
            {
                filter = "Business Unit Report";
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename='" + filter + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Millisecond + ".xls'");
            return PartialView("_ReportList", reportDetails);
        }
        #endregion

        #region Material Location wise search and export to excel

        /// <summary>
        /// Materials the location wise report search.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult MaterialLocationWiseReportSearch(string filter)
        {
            BEL.MaterialTrackingWorkflow.Models.Reports.Report report = new BEL.MaterialTrackingWorkflow.Models.Reports.Report();
            report.MaterialLocationList = this.GetMaterialLocationList();
            report.ReportList = this.GetReportDetails(Constants.FilterByMaterialLocation, filter);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ReportList", report.ReportList);
            }
            else
                return View("MaterialLocationWiseReport", report);
        }

        /// <summary>
        /// Materials the location wise export to excel.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult MaterialLocationWiseExportToExcel(string filter)
        {
            List<ReportDetails> reportDetails = this.GetReportDetails(Constants.FilterByMaterialLocation, filter);

            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "All Material Locations Report";
            }
            else if (filter.IndexOf(',') > 0)
            {
                filter = "Material Locations Report";
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename='" + filter + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Millisecond + ".xls'");
            return PartialView("_ReportList", reportDetails);
        }
        #endregion

        #region Serial Number wise search and export to excel

        /// <summary>
        /// Serials the number wise report search.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult SerialNumberWiseReportSearch(string filter)
        {
            BEL.MaterialTrackingWorkflow.Models.Reports.Report report = new BEL.MaterialTrackingWorkflow.Models.Reports.Report();
            report.ReportList = this.GetReportDetails(Constants.FilterBySerialNumber, filter);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ReportList", report.ReportList);
            }
            else
                return View("SerialNumberWiseReport", report);
        }

        /// <summary>
        /// Serials the number wise export to excel.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult SerialNumberWiseExportToExcel(string filter)
        {
            List<ReportDetails> reportDetails = this.GetReportDetails(Constants.FilterBySerialNumber, filter);

            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "All Serial Numbers Report";
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename='" + filter + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Millisecond + ".xls'");
            return PartialView("_ReportList", reportDetails);
        }
        #endregion

        #region Date wise search and export to excel

        /// <summary>
        /// Dates the wise report search.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult DateWiseReportSearch(string filter)
        {
            BEL.MaterialTrackingWorkflow.Models.Reports.Report report = new BEL.MaterialTrackingWorkflow.Models.Reports.Report();
            if (!string.IsNullOrWhiteSpace(filter))
                filter = string.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(filter));
            report.ReportList = this.GetReportDetails(Constants.FilterByDate, filter);
            if (Request.IsAjaxRequest())
            {

                return PartialView("_ReportList", report.ReportList);
            }
            else
                return View("DateWiseReport", report);
        }

        /// <summary>
        /// Dates the wise export to excel.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public ActionResult DateWiseExportToExcel(string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
                filter = string.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(filter));
            List<ReportDetails> reportDetails = this.GetReportDetails(Constants.FilterByDate, filter);

            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "All Iward_Outward Dates";
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename='" + filter + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Millisecond + ".xls'");
            return PartialView("_ReportList", reportDetails);
        }
        #endregion
    }
}