namespace BEL.MaterialTrackingWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Common;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.OutwardRequest;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using BEL.MaterialTrackingWorkflow.Models.InwardRequest;
    using System.Text.RegularExpressions;
    using System.Globalization;

    public class OutwardRequestController : OutwardRequestBaseController
    {
        // GET: INWARD
        public ActionResult Index(int id = 0)
        {
            OutwardAssmblContract contract = null;
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("FormName", FormNameConstants.OUTWARDASSMBLREQUESTFORM);
            objDict.Add("ItemId", id.ToString());
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
            ViewBag.UserID = this.CurrentUser.UserId;
            ViewBag.UserName = this.CurrentUser.FullName;
            objDict.Add(Parameter.CREATOREMAIL, this.CurrentUser.UserEmail);
            contract = this.GetOutwardAssembledDetails(objDict);
            contract.UserDetails = this.CurrentUser;

            if (contract.Forms != null && contract.Forms.Count > 0)
            {
                TesterSection testerSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.TESTERSECTION)) as TesterSection;

                if (testerSection != null && string.IsNullOrWhiteSpace(testerSection.ProposedByName))
                {
                    Logger.Error("Proposed By Alias is null so not authorised");
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
                if (id > 0)
                {
                    HODSection hodSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSECTION)) as HODSection;

                    if (hodSection.IsActive == true)
                    {
                        ActivityLogSection activityLog = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.ACTIVITYLOG)) as ActivityLogSection;

                        bool isReworkDoneOnce = activityLog.ActivityLogs.Any(f => f.Activity.Trim() == "Rework" && f.SectionName == OutwardSectionName.HODSECTION);

                        hodSection.IsReworkRequired = !isReworkDoneOnce;
                    }
                }
                return this.View("~/Views/OutwardRequest/Assembled/Index.cshtml", contract);
            }
            else
            {
                return View("~/Views/OutwardRequest/Assembled/Index.cshtml", contract);
            }



        }

        // GET: INWARD
        public ActionResult SingleForm(int id = 0)
        {
            OutwardSingleContract contract = null;
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("FormName", FormNameConstants.OUTWARDSINGLEREQUESTFORM);
            objDict.Add("ItemId", id.ToString());
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
            objDict.Add(Parameter.CREATOREMAIL, this.CurrentUser.UserEmail);
            ViewBag.UserID = this.CurrentUser.UserId;
            ViewBag.UserName = this.CurrentUser.FullName;
            contract = this.GetOutwardSingleDetails(objDict);
            contract.UserDetails = this.CurrentUser;
            if (contract.Forms != null && contract.Forms.Count > 0)
            {
                TesterSingleSection testerSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.TESTERSINGLESECTION)) as TesterSingleSection;

                if (testerSection != null && string.IsNullOrWhiteSpace(testerSection.ProposedByName))
                {
                    Logger.Error("Proposed By Alias is null so not authorised");
                    return this.RedirectToAction("NotAuthorize", "Master");
                }

                if (id > 0)
                {
                    HODSingleSection hodSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSECTION)) as HODSingleSection;

                    if (hodSection.IsActive == true)
                    {
                        ActivityLogSection activityLog = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.ACTIVITYLOG)) as ActivityLogSection;

                        bool isReworkDoneOnce = activityLog.ActivityLogs.Any(f => f.Activity.Trim() == "Rework" && f.SectionName == OutwardSectionName.HODSINGLESECTION);

                        hodSection.IsReworkRequired = !isReworkDoneOnce;
                    }
                }
                return this.View("~/Views/OutwardRequest/Single/SingleForm.cshtml", contract);
            }
            else
            {
                return View("~/Views/OutwardRequest/Single/SingleForm.cshtml", contract);
            }


        }

        #region "MT Assembled Tester Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveAssembleTesterSection(TesterSection model)
        {

            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.SendForward)
                    {
                        objDict[Parameter.SENDTOLEVEL] = "2";
                        objDict[Parameter.SENDTOROLE] = OUTWARDRoles.RECIPIENT1;
                    }
                    model.ApproversList.ForEach(p =>
                    {
                        if (p.Role == OUTWARDRoles.HOD)
                        {
                            p.Approver = model.HOD;

                        }
                    });
                    model.OutwardDate = String.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.OutwardDate));
                    model.RequestDate = string.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.RequestDate));
                    model.CurrentApprover.Comments = model.Comment;
                    if (model.ProposedBy == null)
                    {
                        model.ProposedBy = this.CurrentUser.UserId;
                    }
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.TesterAttachment));
                    model.TesterAttachment = string.Join(",", FileListHelper.GetFileNames(model.TesterAttachment));

                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Assembled HOD Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveAssembleHODSection(HODSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                if (model.ActionStatus == ButtonActionStatus.Hold || model.ActionStatus == ButtonActionStatus.Resume)
                {
                    ModelState.Remove("Action");
                }
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.BackToCreator)
                    {
                        if (model.Action == "Approve")
                        {
                            model.ActionStatus = ButtonActionStatus.BackToCreator;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                            objDict[Parameter.SENDTOROLE] = OUTWARDRoles.CREATOR;
                        }
                        else if (model.Action == "Rework")
                        {
                            model.ActionStatus = ButtonActionStatus.SendBack;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                        }
                    }

                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Assembled Recipient Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveAssembleRecipientSection(RecipientSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    ////file Operation
                    model.OutwardDate = String.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.OutwardDate));

                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.RecipientAttachment));
                    model.RecipientAttachment = string.Join(",", FileListHelper.GetFileNames(model.RecipientAttachment));
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Single Tester Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveSingleTesterSection(TesterSingleSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.SendForward)
                    {
                        objDict[Parameter.SENDTOLEVEL] = "2";
                        objDict[Parameter.SENDTOROLE] = OUTWARDRoles.RECIPIENT1;
                    }
                    model.ApproversList.ForEach(p =>
                              {
                                  if (p.Role == OUTWARDRoles.HOD)
                                  {
                                      p.Approver = model.HOD;

                                  }
                              });
                    model.OutwardDate = String.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.OutwardDate));
                    model.RequestDate = string.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.RequestDate));
                    model.CurrentApprover.Comments = model.Comment;
                    if (model.ProposedBy == null)
                    {
                        model.ProposedBy = this.CurrentUser.UserId;
                    }
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.TesterSingleAttachment));
                    model.TesterSingleAttachment = string.Join(",", FileListHelper.GetFileNames(model.TesterSingleAttachment));
                    //Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                    status = this.SaveBySingleSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Single HOD Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveSingleHODSection(HODSingleSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                if (model.ActionStatus == ButtonActionStatus.Hold || model.ActionStatus == ButtonActionStatus.Resume)
                {
                    ModelState.Remove("Action");
                }
                if (this.ValidateModelState(model))
                {
                    if (model.ActionStatus == ButtonActionStatus.BackToCreator)
                    {
                        if (model.Action == "Approve")
                        {
                            model.ActionStatus = ButtonActionStatus.BackToCreator;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                            objDict[Parameter.SENDTOROLE] = OUTWARDRoles.CREATOR;
                        }
                        else if (model.Action == "Rework")
                        {
                            model.ActionStatus = ButtonActionStatus.SendBack;
                            objDict[Parameter.SENDTOLEVEL] = "0";
                        }
                    }

                    //if (model.ActionStatus == ButtonActionStatus.NextApproval)
                    //{
                    //    if (model.Action == "Approve")
                    //    {
                    //        model.ActionStatus = ButtonActionStatus.NextApproval;
                    //    }
                    //    else if (model.Action == "Rework")
                    //    {
                    //        model.ActionStatus = ButtonActionStatus.SendBack;
                    //        objDict[Parameter.SENDTOLEVEL] = "0";
                    //    }
                    //}


                    status = this.SaveBySingleSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Single Recipient Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveSingleRecipientSection(RecipientSingleSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (this.ValidateModelState(model))
                {
                    ////file Operation
                    model.OutwardDate = String.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(model.OutwardDate));
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.RecipientSingleAttachment));
                    model.RecipientSingleAttachment = string.Join(",", FileListHelper.GetFileNames(model.RecipientSingleAttachment));
                    Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                    status = this.SaveBySingleSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.OutwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "Get data of inward for Single "
        /// <summary>
        /// Gets the inward requests.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="selectedInwardId">The selected inward identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInwardRequests(string projectName, string projectCode, string itemId, string selectedInwardId, string currentUserAlise)
        {
            List<InwardDetail> list = null;
            if (projectName != null && projectCode != null)
            {
                list = this.RetrieveAllInwardId(projectName, projectCode, currentUserAlise);
            }

            if (itemId != null && selectedInwardId != string.Empty)
            {
                var inwardDetail = new InwardDetail { InwardID = selectedInwardId };
                if (list != null) list.Add(inwardDetail);
            }
            return this.Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the inward request details.
        /// </summary>
        /// <param name="inwardId">The inward identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInwardRequestDetails(string inwardId)
        {
            List<InwardDetail> list = null;
            if (inwardId != null)
            {
                list = this.RetrieveInwardDetails(inwardId);
            }
            return this.Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
        #endregion