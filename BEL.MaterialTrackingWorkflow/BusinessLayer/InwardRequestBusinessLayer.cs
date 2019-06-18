namespace BEL.MaterialTrackingWorkflow.BusinessLayer
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.InwardRequest;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BEL.DataAccessLayer;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using Newtonsoft.Json;
    using System.Linq;

    using BEL.MaterialTrackingWorkflow.Models;
    using Microsoft.SharePoint.Client.UserProfiles;

    public class InwardRequestBusinessLayer
    {
        private static readonly Lazy<InwardRequestBusinessLayer> lazy =
           new Lazy<InwardRequestBusinessLayer>(() => new InwardRequestBusinessLayer());

        public static InwardRequestBusinessLayer Instance { get { return lazy.Value; } }

        /// <summary>
        /// The padlock
        /// </summary>
        private static readonly object Padlock = new object();

        private InwardRequestBusinessLayer()
        {
            string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.MTSITEURL);
            if (!string.IsNullOrEmpty(siteURL))
            {
                if (this.context == null)
                {
                    this.context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
                    //string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(siteURL));
                    //string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(siteURL).Authority, realm).AccessToken;

                    //using (ClientContext cc = TokenHelper.GetClientContextWithAccessToken(siteURL, accessToken))
                    //{
                    //    cc.Load(cc.Web, p => p.Title);
                    //    cc.ExecuteQuery();
                    //    this.context = cc;
                    //}
                }
                if (this.web == null)
                {
                    this.web = BELDataAccessLayer.Instance.CreateWeb(this.context);
                }
            }
        }


        /// <summary>
        /// The context
        /// </summary>
        private ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        private Web web = null;

        public OutwardDetail GetOutwardId(string outwardId)
        {
            OutwardDetail outwardDetails = new OutwardDetail();

            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDSINGLEREQUESTSLIST);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='OutwardId' /><Value Type='Text'>" + outwardId + "</Value></Eq></Where>  </Query> </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    outwardDetails.ID = Convert.ToInt32(item["ID"]);
                    outwardDetails.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    outwardDetails.ProjectName = Convert.ToString(item["ProjectName"]);
                    outwardDetails.InwardId = Convert.ToString(item["InwardId"]);
                    FieldUserValue userValue = item["ProposedBy"] as FieldUserValue;

                    if (userValue != null) outwardDetails.ProposedBy = userValue.LookupId.ToString();
                }
                outwardDetails.ListName = MaterialTrackingListNames.OUTWARDSINGLEREQUESTSLIST;
            }
            List spListoutward = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTSLIST);
            CamlQuery queryoutward = new CamlQuery();
            queryoutward.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='OutwardId' /><Value Type='Text'>" + outwardId + "</Value></Eq></Where>  </Query> </View>";
            ListItemCollection itemsoutward = spListoutward.GetItems(query);
            this.context.Load(itemsoutward);
            this.context.ExecuteQuery();
            if (itemsoutward != null && itemsoutward.Count != 0)
            {
                foreach (ListItem item in itemsoutward)
                {
                    outwardDetails.ID = Convert.ToInt32(item["ID"]);
                    outwardDetails.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    outwardDetails.ProjectName = Convert.ToString(item["ProjectName"]);
                    FieldUserValue userValue = item["ProposedBy"] as FieldUserValue;

                    if (userValue != null) outwardDetails.ProposedBy = userValue.LookupId.ToString();
                }
                outwardDetails.ListName = MaterialTrackingListNames.OUTWARDREQUESTSLIST;
            }
            return outwardDetails;

        }


        #region "GET DATA"
        /// <summary>
        /// Gets the DCR details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public InwardContract GetInwardRequestDetails(IDictionary<string, string> objDict)
        {
            InwardContract contract = new InwardContract();

            // BELDataAccessLayer helper = new BELDataAccessLayer(); // KKSoni Change Helper class name to BELDataAccessLayer
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string strUserName = null;
                string strUserID = null;
                string stralias = null;
                MasterDataHelper masterHelper = new MasterDataHelper();
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                string userEmail = objDict[Parameter.CREATOREMAIL];
                //contract.UserDetails = BELDataAccessLayer.Instance.GetUserInformation(userEmail);
                IForm inwardForm = new InwardRequestForm(true);
                inwardForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.MATERIALTRACKINGAPP, formName, itemId, userID, inwardForm);
                if (inwardForm != null && inwardForm.SectionsList != null && inwardForm.SectionsList.Count > 0)
                {
                    var sectionDetails = inwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(INWARDSectionName.RECIPIENT1SECTION)) as Recipient1Section;
                    sectionDetails.Recipient1Attachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.Recipient1Attachment) && sectionDetails.Recipient1Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                    if (sectionDetails != null)
                    {
                        if (itemId == 0)
                        {

                            List<IMasterItem> approvers = sectionDetails.MasterData.FirstOrDefault(p => p.NameOfMaster == MaterialTrackingListNames.EMPLOYEEMASTER).Items;
                            List<EmployeeDetailsMasterListItem> approverList = approvers.ConvertAll(p => (EmployeeDetailsMasterListItem)p);

                            var recipient1 = approverList.Find(e => e.UserID.Equals(userID) && e.Role.Equals(INWARDRoles.RECIPIENT1) && e.UserSelection);
                            if (recipient1 != null)
                            {
                                sectionDetails.ProposedByName = masterHelper.GetEmployeeAliasByEmail(this.context, this.web, userEmail);

                                foreach (EmployeeDetailsMasterListItem item in approverList.Where(m => m.UserSelection))
                                {
                                    if (item.Role == INWARDRoles.RECIPIENT2)
                                    {
                                        if (strUserName != null)
                                        {
                                            strUserName = strUserName + ", " + item.UserName;
                                            strUserID = strUserID + ", " + item.UserID;
                                            stralias = stralias + ", " + item.AliasNames;
                                        }
                                        else
                                        {
                                            strUserName = item.UserName;
                                            strUserID = item.UserID;
                                            stralias = item.AliasNames;
                                        }
                                    }

                                }

                                sectionDetails.ApproversList.ForEach(p =>
                                {

                                    p.Approver = strUserID;
                                    p.ApproverName = stralias;

                                    sectionDetails.Recipient2 = strUserID;
                                    sectionDetails.Recipient2Alise = stralias;

                                });
                                sectionDetails.RequestDate = DateTime.Now.ToShortDateString();
                            }
                        }
                        else
                        {
                            sectionDetails.OutwardIdOld = sectionDetails.OutwardId;
                            sectionDetails.RequestDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionDetails.RequestDate));
                            sectionDetails.ApproversList.Remove(sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                        }
                    }

                    var sectionRecipient2SectionDetails = inwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(INWARDSectionName.RECIPIENT2SECTION)) as Recipient2Section;
                    sectionRecipient2SectionDetails.Recipient2Attachment = sectionRecipient2SectionDetails.Files != null && sectionRecipient2SectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionRecipient2SectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionRecipient2SectionDetails.Recipient2Attachment) && sectionRecipient2SectionDetails.Recipient2Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                    if (sectionRecipient2SectionDetails != null)
                    {
                        //List<IMasterItem> approvers = sectionDetails.MasterData.FirstOrDefault(p => p.NameOfMaster == MaterialTrackingListNames.EMPLOYEEMASTER).Items;
                        //List<EmployeeDetailsMasterListItem> approverList = approvers.ConvertAll(p => (EmployeeDetailsMasterListItem)p);
                        //List<TesterMasterListItem> tester = new List<TesterMasterListItem>();
                        //foreach (EmployeeDetailsMasterListItem item in approverList)
                        //{
                        //    if (item.Role == INWARDRoles.TESTER)
                        //    {
                        //        TesterMasterListItem testerMasterListItem =
                        //            new TesterMasterListItem { AliasNames = item.AliasNames };
                        //        tester.Add(testerMasterListItem);
                        //    }
                        //}
                        //sectionRecipient2SectionDetails.TesterList = tester;

                        // List<IMasterItem> projectList = sectionRecipient2SectionDetails.MasterData.FirstOrDefault(p => p.NameOfMaster == MaterialTrackingListNames.PROJECTNAMEMASTER).Items;
                        //List<ProjectNameMasterListItem> activeProjectList = projectList.ConvertAll(p => (ProjectNameMasterListItem)p);
                        //List<ProjectNameMasterListItem> listActiveProject = new List<ProjectNameMasterListItem>();
                        //foreach (var item in activeProjectList)
                        //{
                        //    if (item.IsActive != null && item.IsActive == true)
                        //    {
                        //        listActiveProject.Add(item);
                        //    }
                        //}
                        //List<IMasterItem> projectNewList = listActiveProject.ConvertAll(p => (IMasterItem)p);

                        //sectionRecipient2SectionDetails.MasterData.Remove(new ProjectNameMaster());
                        //sectionRecipient2SectionDetails.MasterData.AddRange(projectNewList.);

                        sectionRecipient2SectionDetails.InwardId = sectionDetails.InwardID;


                    }
                    contract.Forms.Add(inwardForm);
                }
            }
            return contract;
        }
        #endregion


        #region "SAVE DATA"

        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="sectionDetails"></param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public ActionStatus SaveBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();
                InwardNoCount currentInwardNo = null;
                SerialNoCount currentSerialNo = null;

                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = MaterialTrackingListNames.INWARDACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.MATERIALTRACKINGAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.INWARDREQUESTFORM;


                    Recipient1Section section = null;
                    Recipient2Section sectionRecipient2 = null;
                    if (sectionDetails.SectionName == INWARDSectionName.RECIPIENT1SECTION)
                    {
                        section = sectionDetails as Recipient1Section;
                        section.RequestDate = string.Format("{0:dd-MM-yyyy}", DateTime.Now);
                        if (string.IsNullOrEmpty(section.InwardID) && sectionDetails.ActionStatus == ButtonActionStatus.SaveAsDraft)
                        {
                            section.Title = section.InwardID = "View";
                        }
                        else if (sectionDetails.ActionStatus == ButtonActionStatus.NextApproval && (string.IsNullOrEmpty(section.InwardID) || section.InwardID == "View"))
                        {
                            currentInwardNo = GetInwardRequestNo();
                            section.RequestDate = string.Format("{0:dd-MM-yyyy}", DateTime.Now);
                            if (currentInwardNo != null)
                            {
                                currentInwardNo.CurrentValue = currentInwardNo.CurrentValue + 1;
                                Logger.Info("Inward Request Current Value + 1 = " + currentInwardNo.CurrentValue);
                                string hourMinute = DateTime.Now.ToString("HH:mm");
                                section.InwardID = string.Format("INW-{0} {1}{2}-{3}-{4}", DateTime.Today.Year, DateTime.Now.Month.ToString("d2"), DateTime.Today.Day.ToString("d2"), hourMinute, string.Format("{0:0000}", currentInwardNo.CurrentValue));
                                section.Title = section.InwardID;
                                Logger.Info("Inward Request No is " + section.InwardID);
                                status.ExtraData = section.InwardID;
                            }
                        }
                    }


                    if (sectionDetails.SectionName == INWARDSectionName.RECIPIENT2SECTION)
                    {
                        sectionRecipient2 = sectionDetails as Recipient2Section;
                        if (sectionDetails.ActionStatus == ButtonActionStatus.Complete && string.IsNullOrEmpty(sectionRecipient2.SerialNo))
                        {
                            currentSerialNo = GetSerialNo();
                            sectionRecipient2.TesterAssignedDate = DateTime.Now;
                            if (currentSerialNo != null)
                            {
                                currentSerialNo.CurrentValue = currentSerialNo.CurrentValue + 1;
                                Logger.Info("Inward Request Current Value + 1 = " + currentSerialNo.CurrentValue);
                                var month = currentSerialNo.Month;
                                var start = string.Empty;
                                switch (month)
                                {
                                    case 1:
                                        start = "A";
                                        break;
                                    case 2:
                                        start = "B";
                                        break;
                                    case 3:
                                        start = "C";
                                        break;
                                    case 4:
                                        start = "D";
                                        break;
                                    case 5:
                                        start = "E";
                                        break;
                                    case 6:
                                        start = "F";
                                        break;
                                    case 7:
                                        start = "G";
                                        break;
                                    case 8:
                                        start = "H";
                                        break;
                                    case 9:
                                        start = "J";
                                        break;
                                    case 10:
                                        start = "K";
                                        break;
                                    case 11:
                                        start = "L";
                                        break;
                                    case 12:
                                        start = "M";
                                        break;
                                }
                                sectionRecipient2.SerialNo = string.Format("{0}{1}{2}", start, (DateTime.Today.Year % 100), string.Format("{0:0000}", currentSerialNo.CurrentValue));

                                Logger.Info("Inward Request No is " + sectionRecipient2.SerialNo);
                                status.ExtraData = sectionRecipient2.InwardId;
                                // status.ExtraData = sectionRecipient2.SerialNo;
                            }
                        }
                    }
                    if (section != null && section.SectionName == INWARDSectionName.RECIPIENT1SECTION)
                    {
                        OutwardDetail Outwarddetail = this.GetOutwardId(section.OutwardId);
                        Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                        if (section.ActionStatus == ButtonActionStatus.NextApproval)
                        {
                            values = new Dictionary<string, dynamic> { { "IsInwardGenerated", true } };
                        }

                        if (Outwarddetail.ListName != null)
                        {
                            BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, Outwarddetail.ListName, Outwarddetail.ID, values);
                        }
                        if (section.OutwardId != section.OutwardIdOld && section.OutwardIdOld != null)
                        {
                            var outwardOldDetail = this.GetOutwardId(section.OutwardIdOld);
                            if (outwardOldDetail.ID > 0)
                            {
                                values = new Dictionary<string, dynamic> { { "IsInwardGenerated", false } };
                                BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web,
                                    Outwarddetail.ListName, outwardOldDetail.ID, values);
                            }
                        }
                    }

                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.Where(p => p.ListName.Equals(MaterialTrackingListNames.INWARDLIST)).FirstOrDefault<ListItemDetail>();
                    if (sectionDetails.SectionName == INWARDSectionName.RECIPIENT1SECTION)
                    {
                        if (itemDetails != null && (itemDetails.ItemId > 0 && currentInwardNo != null))
                        {
                            UpdateInwardRequestNoCount(currentInwardNo);
                            Logger.Info("Update Inward No " + section.InwardID);
                        }
                    }
                    if (sectionDetails.SectionName == INWARDSectionName.RECIPIENT2SECTION)
                    {
                        if (itemDetails != null && (itemDetails.ItemId > 0 && currentSerialNo != null))
                        {
                            UpdateSerialNoCount(currentSerialNo);
                            Logger.Info("Update Serial No " + sectionRecipient2.SerialNo);
                        }
                    }
                    if (sectionDetails.ActionStatus == ButtonActionStatus.Complete)
                    {
                        if (sectionRecipient2.MaterialHandedoverto != null)
                        {
                            EmailHelper eHelper = new EmailHelper();
                            int currLevel;
                            Dictionary<string, string> mailCustomValues = null; List<FileDetails> emailAttachments = null;
                            Dictionary<string, string> email = new Dictionary<string, string>();
                            List<ListItemDetail> itemdetail = new List<ListItemDetail>();
                            string applicationName = ApplicationNameConstants.MATERIALTRACKINGAPP; string formName = FormNameConstants.INWARDREQUESTFORM;
                            string Role = "Recipient2";

                            string from = string.Empty, to = string.Empty, cc = string.Empty, role = string.Empty, tmplName = string.Empty, strAllusers = string.Empty, nextApproverIds = string.Empty;
                            from = objDict[Parameter.USEREID];
                            from = BELDataAccessLayer.GetEmailUsingUserID(context, web, from);
                            to = sectionRecipient2.MaterialHandedovertoUserID;
                            to = BELDataAccessLayer.GetEmailUsingUserID(context, web, to);

                            cc = BELDataAccessLayer.GetEmailUsingUserID(context, web, cc);
                            role = Role;
                            tmplName = EmailTemplateName.MATERIALHANDEDOVERTO;
                            itemdetail.Add(new ListItemDetail() { ItemId = itemDetails.ItemId, IsMainList = true, ListName = MaterialTrackingListNames.INWARDLIST });
                            if (mailCustomValues == null)
                            {
                                mailCustomValues = new Dictionary<string, string>();
                            }
                            mailCustomValues[Parameter.CURRENTAPPROVERNAME] = BELDataAccessLayer.GetNameUsingUserID(context, web, objDict[Parameter.USEREID]);

                            email = eHelper.GetEmailBody(context, web, tmplName, itemdetail, mailCustomValues, role, applicationName, formName);

                            email["Body"] = email["Body"].Replace("[[INWARD:Recipient2Particulars​]]", sectionRecipient2.Recipient2Particulars);
                            eHelper.SendMail(applicationName, formName, tmplName, email["Subject"], email["Body"], from, to, cc, false, emailAttachments);

                        }
                    }
                    if (itemDetails != null && itemDetails.ItemId > 0)
                    {
                        status.IsSucceed = true;
                        status.ItemID = itemDetails.ItemId;
                        switch (sectionDetails.ActionStatus)
                        {
                            case ButtonActionStatus.SaveAsDraft:
                                status.Messages.Add("Text_SaveDraftSuccess");
                                break;
                            case ButtonActionStatus.SaveAndNoStatusUpdate:
                                status.Messages.Add("Text_SaveSuccess");
                                break;
                            case ButtonActionStatus.NextApproval:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                            case ButtonActionStatus.Delegate:
                                status.Messages.Add("Text_DelegatedSuccess");
                                break;
                            case ButtonActionStatus.Complete:
                                status.Messages.Add("Text_CompleteSuccess");
                                break;
                            case ButtonActionStatus.Rejected:
                                status.Messages.Add("Text_RejectedSuccess");
                                break;
                            default:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                        }
                    }
                    else
                    {
                        status.IsSucceed = false;
                        status.Messages.Add(ApplicationConstants.ERRORMESSAGE);
                    }

                }
                return status;
            }
        }
        #endregion

        public UserDetails getUSerDetail(int id)
        {
            UserDetails detail = new UserDetails();
            var peopleManager = new PeopleManager(context);
            User usr = context.Web.GetUserById(id);
            PersonProperties personProperties = peopleManager.GetMyProperties();
            context.Load(usr, p => p.Id, p => p.LoginName, p => p.Email);
            context.Load(personProperties);
            context.ExecuteQuery();
            detail.UserId = usr.Id.ToString();
            detail.LoginName = usr.LoginName;
            detail.Department = personProperties.UserProfileProperties["Department"];
            detail.FullName = personProperties.DisplayName;
            detail.UserEmail = usr.Email; //personProperties.Email;
            return detail;
        }

        /// <summary>
        /// Get DCR No Logic
        /// </summary>
        /// <param name="businessunit">Business Unit</param>
        /// <returns>DCR No Count</returns>
        public InwardNoCount GetInwardRequestNo()
        {
            try
            {
                List<InwardNoCount> lstdcrCount = new List<InwardNoCount>();
                List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDREQUESTNOCOUNT);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View><ViewFields><FieldRef Name='Title' /><FieldRef Name='Year' /><FieldRef Name='CurrentValue' /></ViewFields>   </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    foreach (ListItem item in items)
                    {
                        InwardNoCount obj = new InwardNoCount();
                        obj.ID = item.Id;

                        obj.Year = Convert.ToInt32(item["Year"]);
                        obj.CurrentValue = Convert.ToInt32(item["CurrentValue"]);
                        if (obj.Year != DateTime.Today.Year)
                        {
                            obj.Year = DateTime.Today.Year;
                            obj.CurrentValue = 0;
                        }

                        lstdcrCount.Add(obj);
                    }
                }

                if (lstdcrCount != null)
                {
                    return lstdcrCount.FirstOrDefault(p => p.Year == DateTime.Today.Year);
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Update DCR No Count
        /// </summary>
        /// <param name="currentValue">Current Value</param>
        public void UpdateInwardRequestNoCount(InwardNoCount currentValue)
        {
            if (currentValue != null && currentValue.ID != 0)
            {
                try
                {

                    Logger.Info("Aync update DCR Current value currentValue : " + currentValue.CurrentValue);
                    List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDREQUESTNOCOUNT);
                    ListItem item = spList.GetItemById(currentValue.ID);

                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                    item["CurrentValue"] = currentValue.CurrentValue;
                    item["Year"] = currentValue.Year;
                    item.Update();
                    //Version conflict error
                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    Logger.Error("Error while Update DCR no Current Value");
                    Logger.Error(ex);
                }
            }
        }


        /// <summary>
        /// Get DCR No Logic
        /// </summary>
        /// <param name="businessunit">Business Unit</param>
        /// <returns>DCR No Count</returns>
        public SerialNoCount GetSerialNo()
        {
            try
            {
                List<SerialNoCount> lstSerialCount = new List<SerialNoCount>();
                List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.SERIALNOCOUNTCOUNT);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View><ViewFields><FieldRef Name='Title' /><FieldRef Name='Year' /><FieldRef Name='CurrentValue' /></ViewFields>   </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    foreach (ListItem item in items)
                    {
                        SerialNoCount obj = new SerialNoCount();
                        obj.ID = item.Id;

                        obj.Year = Convert.ToInt32(item["Year"]);
                        obj.CurrentValue = Convert.ToInt32(item["CurrentValue"]);
                        obj.Month = DateTime.Today.Month;
                        if (obj.Year != DateTime.Today.Year)
                        {
                            obj.Year = DateTime.Today.Year;

                            obj.CurrentValue = 0;
                        }

                        lstSerialCount.Add(obj);
                    }
                }

                if (lstSerialCount != null)
                {
                    return lstSerialCount.FirstOrDefault(p => p.Year == DateTime.Today.Year);
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Update DCR No Count
        /// </summary>
        /// <param name="currentValue">Current Value</param>
        public void UpdateSerialNoCount(SerialNoCount currentValue)
        {
            if (currentValue != null && currentValue.ID != 0)
            {
                try
                {

                    Logger.Info("Aync update DCR Current value currentValue : " + currentValue.CurrentValue);
                    List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.SERIALNOCOUNTCOUNT);
                    ListItem item = spList.GetItemById(currentValue.ID);

                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                    item["CurrentValue"] = currentValue.CurrentValue;
                    item["Year"] = currentValue.Year;
                    item.Update();
                    //Version conflict error
                    item.RefreshLoad(); // Pooja Atkotiya - Added for Version Conflict!
                    context.Load(item);
                    context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    Logger.Error("Error while Update DCR no Current Value");
                    Logger.Error(ex);
                }
            }
        }

        public string GetProjectMasterData(string title)
        {
            MasterDataHelper masterHelper = new MasterDataHelper();
            IMaster projectMaster = masterHelper.GetMasterData(context, web, new List<IMaster>() { new ProjectNameMaster() }).FirstOrDefault();
            string projectJson = JSONHelper.ToJSON<IMaster>(projectMaster);
            var filterdProjectMaster = JSONHelper.ToObject<IMaster>(projectJson);
            filterdProjectMaster.Items = filterdProjectMaster.Items.Where(x => ((x.Value).ToLower() == "yes")).ToList();
            filterdProjectMaster.Items = filterdProjectMaster.Items.Where(x => (x.Title ?? string.Empty).ToLower().Trim().Contains((title ?? string.Empty).ToLower().Trim())).ToList();
            return JSONHelper.ToJSON<IMaster>(filterdProjectMaster);
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        public List<ProjectNameMasterListItem> GetAllProjects()
        {
            MasterDataHelper masterHelper = new MasterDataHelper();
            List<IMasterItem> projectNameMasterList = masterHelper.GetMasterData(context, web, new List<IMaster>() { new ProjectNameMaster() }).FirstOrDefault(p => p.NameOfMaster == MaterialTrackingListNames.PROJECTNAMEMASTER).Items;
            List<ProjectNameMasterListItem> projectNameList = projectNameMasterList.ConvertAll(p => (ProjectNameMasterListItem)p);
            projectNameList = projectNameList.Where(x => ((x.Value).ToLower() == "yes") && !string.IsNullOrWhiteSpace(x.Title)).ToList();
            return projectNameList;
        }

        //        public List<OutwardDetail> RetrieveOutwardDetails(string OutwardId)
        //        {
        //            List<OutwardDetail> Outward = new List<OutwardDetail>();
        //            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTSLIST);
        //            CamlQuery query = new CamlQuery();
        //            query.ViewXml = @"<View>
        //                                    <Query>
        //                                            <Where>
        //                                                    <And>
        //                                                        <And>
        //                                                            <And>
        //                                                            <Eq>
        //                                                                <FieldRef Name='Status' />
        //                                                                <Value Type='Text'>Completed</Value>
        //                                                            </Eq>
        //                                                            <Eq>
        //                                                                <FieldRef Name='TypeofMaterial' />
        //                                                                <Value Type='Text'>Returnable</Value>
        //                                                            </Eq>
        //                                                            </And>
        //                                                            <Eq>
        //                                                                <FieldRef Name='IsInwardGenerated' />
        //                                                                <Value Type='Boolean'>0</Value>
        //                                                            </Eq>
        //                                                        </And>
        //                                                        <Contains>
        //                                                             <FieldRef Name='OutwardId' />
        //                                                             <Value Type='Text'>" + OutwardId + @"</Value>
        //                                                        </Contains>
        //                                                   </And>
        //                                            </Where>
        //                                    </Query> 
        //                            </View>";
        //            ListItemCollection items = spList.GetItems(query);
        //            this.context.Load(items);
        //            this.context.ExecuteQuery();
        //            if (items != null && items.Count != 0)
        //            {
        //                foreach (ListItem item in items)
        //                {
        //                    OutwardDetail OutwardDetail = new OutwardDetail();
        //                    OutwardDetail.OutwardId = Convert.ToString(item["OutwardId"]);
        //                    OutwardDetail.Particulars = Convert.ToString(item["Particulars"]);
        //                    OutwardDetail.Location = Convert.ToString(item["LocationAddress"]);
        //                    OutwardDetail.SenderName = Convert.ToString(item["RecipientName"]);
        //                    OutwardDetail.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
        //                    OutwardDetail.CourierDetails = Convert.ToString(item["CourierDetails"]);
        //                    OutwardDetail.AWDNo = Convert.ToString(item["AWDNo"]);
        //                    Outward.Add(OutwardDetail);
        //                }
        //            }
        //            List spList1 = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDSINGLEREQUESTSLIST);
        //            CamlQuery query1 = new CamlQuery();
        //            query1.ViewXml = @"<View>
        //                                    <Query>
        //                                            <Where>
        //                                                    <And>
        //                                                        <And>
        //                                                            <And>
        //                                                            <Eq>
        //                                                                <FieldRef Name='Status' />
        //                                                                <Value Type='Text'>Completed</Value>
        //                                                            </Eq>
        //                                                            <Eq>
        //                                                                <FieldRef Name='TypeofMaterial' />
        //                                                                <Value Type='Text'>Returnable</Value>
        //                                                            </Eq>
        //                                                            </And>
        //                                                            <Eq>
        //                                                                <FieldRef Name='IsInwardGenerated' />
        //                                                                <Value Type='Boolean'>0</Value>
        //                                                            </Eq>
        //                                                        </And>
        //                                                        <Contains>
        //                                                             <FieldRef Name='OutwardId' />
        //                                                             <Value Type='Text'>" + OutwardId + @"</Value>
        //                                                        </Contains>
        //                                                   </And>
        //                                            </Where>
        //                                    </Query> 
        //                            </View>";
        //            ListItemCollection items1 = spList1.GetItems(query1);
        //            this.context.Load(items1);
        //            this.context.ExecuteQuery();
        //            if (items1 != null && items1.Count != 0)
        //            {
        //                foreach (ListItem item in items1)
        //                {
        //                    OutwardDetail OutwardDetail = new OutwardDetail();
        //                    OutwardDetail.OutwardId = Convert.ToString(item["OutwardId"]);
        //                    OutwardDetail.Particulars = Convert.ToString(item["Particulars"]);
        //                    OutwardDetail.Location = Convert.ToString(item["LocationAddress"]);
        //                    OutwardDetail.SenderName = Convert.ToString(item["RecipientName"]);
        //                    OutwardDetail.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
        //                    OutwardDetail.CourierDetails = Convert.ToString(item["CourierDetails"]);
        //                    OutwardDetail.AWDNo = Convert.ToString(item["AWDNo"]);

        //                    Outward.Add(OutwardDetail);
        //                }
        //            }
        //            if (Outward == null)
        //            {
        //                OutwardDetail OutwardDetail = new OutwardDetail();
        //                OutwardDetail.OutwardId.Replace(" ", "");
        //            }
        //            return Outward;

        //        }

        public List<TesterMasterListItem> GetTesters(string projectCode)
        {
            List<TesterMasterListItem> testers = new List<TesterMasterListItem>();
            List<TesterMasterListItem> testersAlise = new List<TesterMasterListItem>();
            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.PROJECTNAMEMASTER);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='ProjectCode' /><Value Type='Text'>" +
                            projectCode +
                            "</Value></Eq></Where> </Query> </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);


            List spListEmployeeMaster = this.web.Lists.GetByTitle(MaterialTrackingListNames.EMPLOYEEMASTER);
            CamlQuery queryemp = new CamlQuery();
            queryemp.ViewXml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='Role' /><Value Type='Text'>Tester</Value> </Eq><Eq><FieldRef Name='UserSelection' /><Value Type='Boolean'>1</Value></Eq> </And></Where> 
            </Query> 
      </View>";
            ListItemCollection itemsEmployeeMaster = spListEmployeeMaster.GetItems(queryemp);
            this.context.Load(itemsEmployeeMaster);
            this.context.ExecuteQuery();

            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    if (item["Testers"] != null)
                    {
                        foreach (FieldUserValue userValue in item["Testers"] as FieldUserValue[])
                        {
                            TesterMasterListItem tester = new TesterMasterListItem();
                            tester.UserID = userValue.LookupId.ToString();
                            testers.Add(tester);
                        }
                    }
                }
            }
            if (testers.Count() > 0)
            {
                foreach (var a in testers)
                {
                    foreach (ListItem item in itemsEmployeeMaster)
                    {
                        var userValue = item["UserName"] as FieldUserValue;
                        if (Convert.ToString(userValue.LookupId) == a.UserID)
                        {
                            TesterMasterListItem tester = new TesterMasterListItem();
                            tester.AliasNames = Convert.ToString(item["Alias"]);
                            tester.UserID = Convert.ToString(Convert.ToString(userValue.LookupId));
                            testersAlise.Add(tester);
                        }
                    }

                }
            }
            return testersAlise;
        }

        public List<OutwardDetail> GetOutwardID()
        {
            List<OutwardDetail> Outward = new List<OutwardDetail>();
            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTSLIST);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View>
                                    <Query>
                                            <Where>
                                                        <And>
                                                            <And>
                                                            <Eq>
                                                                <FieldRef Name='WorkflowStatus' />
                                                                <Value Type='Text'>Completed</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='TypeofMaterial' />
                                                                <Value Type='Text'>Returnable</Value>
                                                            </Eq>
                                                            </And>
                                                            <Eq>
                                                                <FieldRef Name='IsInwardGenerated' />
                                                                <Value Type='Boolean'>0</Value>
                                                            </Eq>
                                                        </And>
                                            </Where>
                                    </Query> 
                            </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    OutwardDetail OutwardDetail = new OutwardDetail();
                    OutwardDetail.OutwardId = Convert.ToString(item["OutwardId"]);
                    OutwardDetail.Particulars = Convert.ToString(item["Particulars"]);
                    OutwardDetail.Location = Convert.ToString(item["LocationAddress"]);
                    OutwardDetail.SenderName = Convert.ToString(item["RecipientName"]);
                    OutwardDetail.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    OutwardDetail.CourierDetails = Convert.ToString(item["CourierDetails"]);
                    OutwardDetail.AWDNo = Convert.ToString(item["AWDNo"]);
                    OutwardDetail.ProjectName = Convert.ToString(item["ProjectName"]);
                    Outward.Add(OutwardDetail);
                }
            }
            List spList1 = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDSINGLEREQUESTSLIST);
            CamlQuery query1 = new CamlQuery();
            query1.ViewXml = @"<View>
                                    <Query>
                                            <Where>
                                                        <And>
                                                            <And>
                                                            <Eq>
                                                                <FieldRef Name='WorkflowStatus' />
                                                                <Value Type='Text'>Completed</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='TypeofMaterial' />
                                                                <Value Type='Text'>Returnable</Value>
                                                            </Eq>
                                                            </And>
                                                            <Eq>
                                                                <FieldRef Name='IsInwardGenerated' />
                                                                <Value Type='Boolean'>0</Value>
                                                            </Eq>
                                                        </And>
                                            </Where>
                                    </Query> 
                            </View>";
            ListItemCollection items1 = spList1.GetItems(query1);
            this.context.Load(items1);
            this.context.ExecuteQuery();
            if (items1 != null && items1.Count != 0)
            {
                foreach (ListItem item in items1)
                {
                    OutwardDetail OutwardDetail = new OutwardDetail();
                    OutwardDetail.OutwardId = Convert.ToString(item["OutwardId"]);
                    OutwardDetail.Particulars = Convert.ToString(item["Particulars"]);
                    OutwardDetail.Location = Convert.ToString(item["LocationAddress"]);
                    OutwardDetail.SenderName = Convert.ToString(item["RecipientName"]);
                    OutwardDetail.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    OutwardDetail.CourierDetails = Convert.ToString(item["CourierDetails"]);
                    OutwardDetail.AWDNo = Convert.ToString(item["AWDNo"]);
                    OutwardDetail.ProjectName = Convert.ToString(item["ProjectName"]);
                    Outward.Add(OutwardDetail);
                }
            }
            if (Outward == null)
            {
                OutwardDetail OutwardDetail = new OutwardDetail();
                OutwardDetail.OutwardId.Replace(" ", "");
            }

            return Outward;
        }

        /// <summary>
        /// Gets the inward identifier.
        /// </summary>
        /// <param name="inwardId">The inward identifier.</param>
        /// <returns></returns>
        public InwardDetails GetInwardDeatils(string inwardId)
        {
            InwardDetails inwardDetails = new InwardDetails();

            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDLIST);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='InwardID' /><Value Type='Text'>" + inwardId + "</Value></Eq></Where>  </Query> </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    inwardDetails.ProjectName = Convert.ToString(item["ProjectName"]);
                    inwardDetails.BUName = Convert.ToString(item["BUName"]);
                    inwardDetails.MaterialCategory = Convert.ToString(item["MaterialCategory"]);
                    inwardDetails.MaterialHandedoverto = Convert.ToString(item["MaterialHandedoverto"]);
                    inwardDetails.MaterialLocation = Convert.ToString(item["MaterialLocation"]);
                    inwardDetails.ProductCategory = Convert.ToString(item["ProductCategory"]);
                    inwardDetails.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    inwardDetails.Particulars = Convert.ToString(item["Particulars"]);
                    inwardDetails.Recipient2Particulars = Convert.ToString(item["Recipient2Particulars"]);
                }
            }
            return inwardDetails;

        }
    }
}