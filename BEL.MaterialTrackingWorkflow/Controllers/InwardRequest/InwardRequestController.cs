using System.Linq;
using BEL.MaterialTrackingWorkflow.Models.Common;

namespace BEL.MaterialTrackingWorkflow.Controllers
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Common;
    using BEL.MaterialTrackingWorkflow.Models.InwardRequest;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Web.Mvc;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using BEL.MaterialTrackingWorkflow.BusinessLayer;
    using System.Drawing.Text;
    using System.Drawing.Imaging;

    public class InwardRequestController : InwardRequestBaseController
    {
        #region " Global Section & Save "
        // GET: INWARD
        public ActionResult Index(int id = 0)
        {
            InwardContract contract = null;
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add("FormName", FormNameConstants.INWARDREQUESTFORM);
            objDict.Add("ItemId", id.ToString());
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
            objDict.Add(Parameter.CREATOREMAIL, this.CurrentUser.UserEmail);

            ViewBag.UserID = this.CurrentUser.UserId;
            contract = this.GetInwardRequestDetails(objDict);
            contract.UserDetails = this.CurrentUser;
            Recipient1Section recipient1Section = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(INWARDSectionName.RECIPIENT1SECTION)) as Recipient1Section;

            if (recipient1Section != null && string.IsNullOrWhiteSpace(recipient1Section.ProposedByName))
            {
                Logger.Error("Proposed By Alias is null so not authorised");
                return this.RedirectToAction("NotAuthorize", "Master");
            }

            if (recipient1Section != null && recipient1Section.Status == "Submitted")
            {
                Recipient2Section recipient2Section = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(INWARDSectionName.RECIPIENT2SECTION)) as Recipient2Section;
                if (!string.IsNullOrWhiteSpace(recipient1Section.OutwardId))
                {
                    OutwardDetail detail = InwardRequestBusinessLayer.Instance.GetOutwardId(recipient1Section.OutwardId);
                    if (detail != null)
                    {

                        recipient2Section.ProjectName = detail.ProjectName;
                        if (string.IsNullOrWhiteSpace(recipient2Section.TypeofMaterial))
                        {
                            recipient2Section.TypeofMaterial = detail.TypeofMaterial;
                        }
                        recipient2Section.OutwardIdType = detail.ListName;


                        if (!string.IsNullOrWhiteSpace(detail.InwardId))
                        {
                            InwardDetails inwardDetails = InwardRequestBusinessLayer.Instance.GetInwardDeatils(detail.InwardId);
                            if (inwardDetails != null)
                            {
                                recipient2Section.BUName = inwardDetails.BUName;
                                recipient2Section.MaterialCategory = inwardDetails.MaterialCategory;
                                if (string.IsNullOrWhiteSpace(recipient2Section.MaterialHandedoverto))
                                {
                                    recipient2Section.MaterialHandedoverto = inwardDetails.MaterialHandedoverto;
                                }
                                recipient2Section.MaterialLocation = inwardDetails.MaterialLocation;
                                recipient2Section.ProductCategory = inwardDetails.ProductCategory;
                                recipient2Section.ProjectName = inwardDetails.ProjectName;
                                if (string.IsNullOrWhiteSpace(recipient2Section.TypeofMaterial))
                                {
                                    recipient2Section.TypeofMaterial = inwardDetails.TypeofMaterial;
                                }
                                recipient2Section.Particulars = inwardDetails.Particulars;
                                if (string.IsNullOrWhiteSpace(recipient2Section.Recipient2Particulars))
                                {
                                    recipient2Section.Recipient2Particulars = inwardDetails.Recipient2Particulars;
                                }
                            }
                        }
                    }
                }
            }

            if (recipient1Section.Status == "Completed")
            {
                Recipient2Section recipient2Section = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName.Equals(INWARDSectionName.RECIPIENT2SECTION)) as Recipient2Section;

                //recipient2Section.BarCodeImage = BarCodeHelper.GenerateBarcodeImage(recipient2Section.SerialNo, "r", 181, 75);
                //PrivateFontCollection pfc = new PrivateFontCollection();
                //pfc.AddFontFile(Server.MapPath("~/fonts/FREE3OF9.TTF"));
                //FontFamily family = new FontFamily("Free 3 of 9", pfc);
                FontFamily family = new FontFamily("Arial");
                Font font = new Font(family, 30, FontStyle.Regular, GraphicsUnit.Point);

                //// Create a temporary bitmap...
                //Bitmap tmpBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                //Graphics objGraphics = Graphics.FromImage(tmpBitmap);
                //// measure the barcode size...
                //SizeF barCodeSize = objGraphics.MeasureString(recipient2Section.SerialNo, font);
                //objGraphics.DrawString(recipient2Section.SerialNo, font, new SolidBrush(Color.Black), 0, 0);
                recipient2Section.BarCodeImage = BarCodeHelper.DrawTextImage(recipient2Section.SerialNo, font, Color.Black, Color.White, new Size(192, 96));

            }

            return View(contract);
        }



        #region "MT Recipient1 Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]//ValidateAntiForgeryTokenOnAllPosts
        public ActionResult SaveRecipient1DetailSection(Recipient1Section model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null)
            {
                if (model.ProposedBy == null)
                {
                    model.ProposedBy = this.CurrentUser.UserId;
                }
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                if (model.NewInward == true)
                {
                    ModelState.Remove("OutwardId");
                }
                if (this.ValidateModelState(model))
                {
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Recipient1Attachment));
                    model.Recipient1Attachment = string.Join(",", FileListHelper.GetFileNames(model.Recipient1Attachment));
                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.InwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.InwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #region "MT Recipient2 Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveRecipient2DetailSection(Recipient2Section model)
        {
            ActionStatus status = new ActionStatus();


            if (model != null)
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                objDict.Add(Parameter.CREATOREMAIL, this.CurrentUser.UserEmail);
                if (this.ValidateModelState(model))
                {
                    model.MaterialHandedoverto = model.MaterialHandedovertoAliasNames;
                    model.Files = new List<FileDetails>();
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Recipient2Attachment));
                    model.Recipient2Attachment = string.Join(",", FileListHelper.GetFileNames(model.Recipient2Attachment));

                    status = this.SaveSection(model, objDict);
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.InwardRequest);
                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.InwardRequest);
                }
            }

            return this.Json(status);
        }
        #endregion

        #endregion
        /// <summary>
        /// Gets the Vendor.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns>json object</returns>
        [HttpGet]
        public JsonResult GetTesters(string projectCode)
        {
            List<TesterMasterListItem> list = null;
            if (!string.IsNullOrWhiteSpace(projectCode))
            {
                list = InwardRequestBusinessLayer.Instance.GetTesters(projectCode);
            }
            return this.Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the Outward ID.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns>json object</returns>
        [HttpGet]
        public JsonResult GetOutwardRequests()
        {
            List<OutwardDetail> list = null;


            list = InwardRequestBusinessLayer.Instance.GetOutwardID();

            return this.Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllProjects()
        {
            List<ProjectNameMasterListItem> list = null;
            list = InwardRequestBusinessLayer.Instance.GetAllProjects();
            return this.Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}