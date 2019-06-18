namespace BEL.MaterialTrackingWorkflow.BusinessLayer
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.OutwardRequest;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BEL.DataAccessLayer;
    using Microsoft.SharePoint.Client.UserProfiles;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using Newtonsoft.Json;
    using BEL.MaterialTrackingWorkflow.Models;

    public class OutwardRequestBusinessLayer
    {
        private static readonly Lazy<OutwardRequestBusinessLayer> lazy =
           new Lazy<OutwardRequestBusinessLayer>(() => new OutwardRequestBusinessLayer());

        public static OutwardRequestBusinessLayer Instance { get { return lazy.Value; } }

        /// <summary>
        /// The padlock
        /// </summary>
        private static readonly object Padlock = new object();

        private OutwardRequestBusinessLayer()
        {
            string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.MTSITEURL);
            if (!string.IsNullOrEmpty(siteURL))
            {
                if (this.context == null)
                {
                    this.context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
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

        /// <summary>
        /// Retrieves all inward identifier.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        public List<InwardDetail> RetrieveAllInwardId(string projectName, string projectCode, string currentUserAlise)
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
                                           <Where>
                                                <And>
                                              <Eq>
                                                 <FieldRef Name='Role' />
                                                 <Value Type='Choice'>Tester</Value>
                                              </Eq>
                                               <Eq>
                                                   <FieldRef Name='UserSelection' />
                                                   <Value Type='Boolean'>1</Value>
                                               </Eq>
                                                </And>
                                           </Where>
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
                            testersAlise.Add(tester);
                        }
                    }

                }
            }

            bool alreadyExist = false;
            for (int i = 0; i < testersAlise.Count; i++)
            {
                if (testersAlise[i].AliasNames == currentUserAlise)
                {
                    alreadyExist = true;
                }
            }

            List<InwardDetail> Inward = new List<InwardDetail>();
            if (alreadyExist)
            {
                List spListInward = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDLIST);
                CamlQuery queryInward = new CamlQuery();

                queryInward.ViewXml = @"<View>
                                           <Query>
                                               <Where>
                                                  <And>
                                                     <Eq>
                                                        <FieldRef Name='ProjectName' />
                                                        <Value Type='Text'>" + projectName + @"</Value>
                                                     </Eq>
                                                     <And>
                                                        <Eq>
                                                           <FieldRef Name='WorkflowStatus' />
                                                           <Value Type='Text'>Completed</Value>
                                                        </Eq>
                                                        <And>
                                                           <Eq>
                                                              <FieldRef Name='IsOutwardGenerated' />
                                                              <Value Type='Choice'>No</Value>
                                                           </Eq>
                                                           <Eq>
                                                              <FieldRef Name='TypeofMaterial' />
                                                              <Value Type='Text'>Returnable</Value>
                                                           </Eq>
                                                        </And>
                                                     </And>
                                                  </And>
                                               </Where>
                                            </Query>
                                     </View>";
                ListItemCollection itemsInward = spListInward.GetItems(queryInward);
                this.context.Load(itemsInward);
                this.context.ExecuteQuery();
                if (itemsInward != null && itemsInward.Count != 0)
                {
                    foreach (ListItem item in itemsInward)
                    {
                        var inwardDetail = new InwardDetail
                        {
                            InwardID = Convert.ToString(item["InwardID"]),
                            InwardItemID = Convert.ToString(item["ID"])
                        };
                        Inward.Add(inwardDetail);
                    }
                }
            }
            return Inward;

        }

        /// <summary>
        /// Retrieves the inward details.
        /// </summary>
        /// <param name="inwardID">The inward identifier.</param>
        /// <returns></returns>
        public List<InwardDetail> RetrieveInwardDetails(string inwardID)
        {
            List<InwardDetail> Inward = new List<InwardDetail>();
            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDLIST);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='InwardID' /><Value Type='Text'>" + inwardID + "</Value></Eq></Where>  </Query> </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    InwardDetail InwardDetail = new InwardDetail();
                    InwardDetail.TypeofMaterial = Convert.ToString(item["TypeofMaterial"]);
                    InwardDetail.SenderName = Convert.ToString(item["SenderName"]);
                    InwardDetail.Location = Convert.ToString(item["Location"]);
                    InwardDetail.Particulars = Convert.ToString(item["Particulars"]);
                    InwardDetail.Recipient2Particulars = Convert.ToString(item["Recipient2Particulars"]);
                    InwardDetail.ProjectName = Convert.ToString(item["ProjectName"]);
                    InwardDetail.MaterialCategory = Convert.ToString(item["MaterialCategory"]);
                    InwardDetail.BUName = Convert.ToString(item["BUName"]);
                    InwardDetail.MaterialHandedoverto = Convert.ToString(item["MaterialHandedoverto"]);
                    InwardDetail.MaterialLocation = Convert.ToString(item["MaterialLocation"]);
                    InwardDetail.InwardDate = Convert.ToString(item["RequestDate"]);
                    InwardDetail.ProposedByAlise = Convert.ToString(item["ProposedByAlise"]);
                    InwardDetail.Recipient2Alise = Convert.ToString(item["Recipient2Alise"]);
                    InwardDetail.ProductCategory = Convert.ToString(item["ProductCategory"]);
                    Inward.Add(InwardDetail);
                }
            }
            return Inward;

        }

        /// <summary>
        /// Gets the inward identifier.
        /// </summary>
        /// <param name="inwardId">The inward identifier.</param>
        /// <returns></returns>
        public InwardDetail GetInwardId(string inwardId)
        {
            InwardDetail inwardDetails = new InwardDetail();

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
                    inwardDetails.ID = Convert.ToInt32(item["ID"]);
                    FieldUserValue userValue = item["ProposedBy"] as FieldUserValue;

                    if (userValue != null) inwardDetails.ProposedBy = userValue.LookupId.ToString();
                }
            }
            return inwardDetails;

        }

        #region "GET DATA"
        /// <summary>
        /// Gets the DCR details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public OutwardAssmblContract GetOutwardRequestDetails(IDictionary<string, string> objDict)
        {
            OutwardAssmblContract contract = new OutwardAssmblContract();

            // BELDataAccessLayer helper = new BELDataAccessLayer(); // KKSoni Change Helper class name to BELDataAccessLayer
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string strUserName = null;
                string strUserId = null;
                string stralias = null;
                string strRecepient1UserName = null;
                string strRecepient1UserId = null;
                string strRecepient1Alias = null;
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userId = objDict[Parameter.USEREID];
                string userEmail = objDict[Parameter.CREATOREMAIL];
                MasterDataHelper masterHelper = new MasterDataHelper();
                //contract.UserDetails = BELDataAccessLayer.Instance.GetUserInformation(userEmail);
                IForm outwardForm = new OutwardAssmblRequestForm(true);
                outwardForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.MATERIALTRACKINGAPP, formName, itemId, userId, outwardForm);
                if (outwardForm != null && outwardForm.SectionsList != null && outwardForm.SectionsList.Count > 0)
                {
                    var sectionDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.TESTERSECTION)) as TesterSection;
                    var sectionHODDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSECTION)) as HODSection;
                    if (sectionDetails != null)
                    {
                        sectionDetails.TesterAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0
                            ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x =>
                                !string.IsNullOrEmpty(sectionDetails.TesterAttachment) &&
                                sectionDetails.TesterAttachment.Split(',').Contains(x.FileName)).ToList())
                            : string.Empty;
                        var approvalMatrix = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(SectionNameConstant.APPLICATIONSTATUS)) as ApplicationStatusSection;
                        if (itemId == 0)
                        {
                            List<IMasterItem> approvers = sectionDetails.MasterData.FirstOrDefault(p => { return p.NameOfMaster != null && p.NameOfMaster == MaterialTrackingListNames.EMPLOYEEMASTER; }).Items;
                            List<EmployeeDetailsMasterListItem> approverList =
                                approvers.ConvertAll(p => (EmployeeDetailsMasterListItem)p);
                            var tester = approverList.Find(e => e.UserID.Equals(userId) && e.Role.Equals(OUTWARDRoles.TESTER) && e.UserSelection);
                            if (tester != null)
                            {
                                sectionDetails.ProposedByName = masterHelper.GetEmployeeAliasByEmail(this.context, this.web, userEmail);
                                sectionDetails.RequestDate = DateTime.Now.ToShortDateString();
                                //  sectionDetails.OutwardDate1 = DateTime.Parse(sectionDetails.OutwardDate);
                                sectionDetails.OutwardDate = DateTime.Now.ToShortDateString();

                                foreach (EmployeeDetailsMasterListItem item in approverList.Where(m => m.UserSelection))
                                {
                                    if (item.Role == OUTWARDRoles.HOD)
                                    {
                                        if (strUserName != null)
                                        {
                                            strUserName = strUserName + "," + item.UserName;
                                            strUserId = strUserId + "," + item.UserID;
                                            stralias = stralias + "," + item.AliasNames;
                                        }
                                        else
                                        {
                                            strUserName = item.UserName;
                                            strUserId = item.UserID;
                                            stralias = item.AliasNames;
                                        }
                                    }

                                    if (item.Role == OUTWARDRoles.RECIPIENT1)
                                    {
                                        if (strRecepient1UserName != null)
                                        {
                                            strRecepient1UserName = strRecepient1UserName + "," + item.UserName;
                                            strRecepient1UserId = strRecepient1UserId + "," + item.UserID;
                                            strRecepient1Alias = strRecepient1Alias + "," + item.AliasNames;
                                        }
                                        else
                                        {
                                            strRecepient1UserName = item.UserName;
                                            strRecepient1UserId = item.UserID;
                                            strRecepient1Alias = item.AliasNames;
                                        }
                                    }

                                }

                                sectionDetails.ApproversList.ForEach(p =>
                                {
                                    //if (p.Role == OUTWARDRoles.HOD)
                                    //{
                                    //    p.Approver = strUserId;
                                    //    p.ApproverName = stralias;
                                    //}

                                    if (p.Role == OUTWARDRoles.RECIPIENT1)
                                    {
                                        p.Approver = strRecepient1UserId;
                                        p.ApproverName = strRecepient1Alias;
                                    }
                                });
                            }

                        }
                        else
                        {
                            sectionDetails.ApproversList.Remove(
                                sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                            sectionDetails.OutwardDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionDetails.OutwardDate));
                            sectionDetails.RequestDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionDetails.RequestDate));
                            var hodSection = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSECTION)) as HODSection;
                            if ((sectionDetails.Status == FormStatus.SENTBACK) &&
                                (hodSection != null && hodSection.Action != null && hodSection.Action == "Approve") &&
                                (approvalMatrix.ApplicationStatusList.Any(p => (p.Role == OUTWARDRoles.CREATOR) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userId.Trim()))))
                            {
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.SaveAsDraft))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.SaveAsDraft).IsVisible = false;

                                }
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.Submit))
                                {

                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.Submit).IsVisible = false;
                                }
                            }
                            if ((sectionDetails.Status == FormStatus.SENTBACK || sectionDetails.Status == FormStatus.SUBMITTED || sectionDetails.Status == FormStatus.COMPLETED) &&
                              (sectionHODDetails != null && sectionHODDetails.Action != null && sectionHODDetails.Action == "Approve"))
                            {
                                for (int i = 0; i < approvalMatrix.ApplicationStatusList.Count; i++)
                                {
                                    if (approvalMatrix.ApplicationStatusList[i].Role == OUTWARDRoles.CREATOR && sectionHODDetails != null && sectionHODDetails.Action == "Approve" && approvalMatrix.ApplicationStatusList[i].Status == ApproverStatus.SENDFORWARD)
                                    {
                                        approvalMatrix.ApplicationStatusList[i].Status = ApproverStatus.APPROVED;
                                    }
                                    if (approvalMatrix.ApplicationStatusList[i].Role == OUTWARDRoles.HOD && sectionHODDetails.Action == "Approve")
                                    {
                                        if (approvalMatrix.ApplicationStatusList[i].Status == ApproverStatus.SENDBACK)
                                            approvalMatrix.ApplicationStatusList[i].Status = ApproverStatus.APPROVED;
                                    }
                                }
                            }
                            if ((sectionDetails.Status == FormStatus.SENTBACK) &&
                               (hodSection != null && hodSection.Action != null && hodSection.Action == "Rework") &&
                               (approvalMatrix.ApplicationStatusList.Any(p => (p.Role == OUTWARDRoles.CREATOR) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userId.Trim()))))
                            {
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.RELEASE))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.RELEASE).IsVisible = false;

                                }
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.PRINTDECLARATION))
                                {

                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.PRINTDECLARATION).IsVisible = false;
                                }
                            }
                        }
                    }
                    var sectionRecepientDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.RECIPIENTSECTION)) as RecipientSection;
                    if (sectionRecepientDetails != null)
                    {
                        sectionRecepientDetails.RecipientAttachment =
                            sectionRecepientDetails.Files != null && sectionRecepientDetails.Files.Count > 0
                                ? JsonConvert.SerializeObject(sectionRecepientDetails.Files.Where(x =>
                                        !string.IsNullOrEmpty(sectionRecepientDetails.RecipientAttachment) &&
                                        sectionRecepientDetails.RecipientAttachment.Split(',')
                                            .Contains(x.FileName))
                                    .ToList())
                                : string.Empty;
                        if (itemId == 0)
                            sectionRecepientDetails.OutwardDate = DateTime.Now.ToShortDateString();
                        else
                            sectionRecepientDetails.OutwardDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionRecepientDetails.OutwardDate));
                    }
                    contract.Forms.Add(outwardForm);
                }
            }
            return contract;
        }

        /// <summary>
        /// Gets the DCR details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public OutwardSingleContract GetOutwardSingleDetails(IDictionary<string, string> objDict)
        {
            OutwardSingleContract contract = new OutwardSingleContract();

            // BELDataAccessLayer helper = new BELDataAccessLayer(); // KKSoni Change Helper class name to BELDataAccessLayer
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string strUserName = null;
                string strUserId = null;
                string stralias = null;
                string strRecepient1UserName = null;
                string strRecepient1UserId = null;
                string strRecepient1Alias = null;
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userId = objDict[Parameter.USEREID];
                string userEmail = objDict[Parameter.CREATOREMAIL];
                MasterDataHelper masterHelper = new MasterDataHelper();
                IForm outwardForm = new OutwardSingleRequestForm(true);
                outwardForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.MATERIALTRACKINGAPP, formName, itemId, userId, outwardForm);
                if (outwardForm != null && outwardForm.SectionsList != null && outwardForm.SectionsList.Count > 0)
                {
                    var approvalMatrix = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(SectionNameConstant.APPLICATIONSTATUS)) as ApplicationStatusSection;
                    var sectionDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.TESTERSINGLESECTION)) as TesterSingleSection;
                    var sectionHODDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSINGLESECTION)) as HODSingleSection;

                    if (sectionDetails != null)
                    {

                        sectionDetails.TesterSingleAttachment =
                            sectionDetails.Files != null && sectionDetails.Files.Count > 0
                                ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x =>
                                    !string.IsNullOrEmpty(sectionDetails.TesterSingleAttachment) && sectionDetails
                                        .TesterSingleAttachment.Split(',').Contains(x.FileName)).ToList())
                                : string.Empty;

                        if (itemId == 0)
                        {
                            List<IMasterItem> approvers = sectionDetails.MasterData
                                .FirstOrDefault(p => p.NameOfMaster == MaterialTrackingListNames.EMPLOYEEMASTER).Items;
                            List<EmployeeDetailsMasterListItem> approverList =
                                approvers.ConvertAll(p => (EmployeeDetailsMasterListItem)p);
                            var tester = approverList.Find(e => e.UserID.Equals(userId) && e.Role.Equals(OUTWARDRoles.TESTER) && e.UserSelection);
                            if (tester != null)
                            {
                                sectionDetails.RequestDate = DateTime.Now.ToShortDateString();
                                sectionDetails.OutwardDate = DateTime.Now.ToShortDateString();
                                sectionDetails.ProposedByName = masterHelper.GetEmployeeAliasByEmail(this.context, this.web, userEmail);
                                foreach (EmployeeDetailsMasterListItem item in approverList.Where(m => m.UserSelection))
                                {
                                    if (item.Role == OUTWARDRoles.HOD)
                                    {
                                        if (strUserName != null)
                                        {
                                            strUserName = strUserName + "," + item.UserName;
                                            strUserId = strUserId + "," + item.UserID;
                                            stralias = stralias + "," + item.AliasNames;
                                        }
                                        else
                                        {
                                            strUserName = item.UserName;
                                            strUserId = item.UserID;
                                            stralias = item.AliasNames;
                                        }
                                    }

                                    if (item.Role == OUTWARDRoles.RECIPIENT1)
                                    {
                                        if (strRecepient1UserName != null)
                                        {
                                            strRecepient1UserName = strRecepient1UserName + "," + item.UserName;
                                            strRecepient1UserId = strRecepient1UserId + "," + item.UserID;
                                            strRecepient1Alias = strRecepient1Alias + "," + item.AliasNames;
                                        }
                                        else
                                        {
                                            strRecepient1UserName = item.UserName;
                                            strRecepient1UserId = item.UserID;
                                            strRecepient1Alias = item.AliasNames;
                                        }
                                    }

                                }



                                sectionDetails.ApproversList.ForEach(p =>
                                {
                                    //if (p.Role == sectionDetails.HOD)
                                    //{
                                    //    p.Approver = strUserId;
                                    //    p.ApproverName = stralias;
                                    //}

                                    if (p.Role == OUTWARDRoles.RECIPIENT1)
                                    {
                                        p.Approver = strRecepient1UserId;
                                        p.ApproverName = strRecepient1Alias;
                                    }
                                });
                            }
                        }
                        else
                        {
                            //if (sectionDetails.OutwardDate != null)
                            //{
                            //    sectionDetails.OutwardDate = sectionDetails.OutwardDate.Value.AddDays(1);//Need to resolved Time zone problem it is a Risky Patch 
                            //}
                            sectionDetails.InwardIdhidden = sectionDetails.InwardId;
                            sectionDetails.InwardIdOld = sectionDetails.InwardId;
                            sectionDetails.InwardDetails = RetrieveInwardDetails(sectionDetails.InwardId).FirstOrDefault();
                            sectionDetails.ApproversList.Remove(
                                sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));

                            sectionDetails.OutwardDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionDetails.OutwardDate));
                            sectionDetails.RequestDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionDetails.RequestDate));


                            //var hodSection = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.HODSECTION)) as HODSingleSection;
                            if ((sectionDetails.Status == FormStatus.SENTBACK) &&
                                (sectionHODDetails != null && sectionHODDetails.Action != null && sectionHODDetails.Action == "Approve") &&
                                (approvalMatrix.ApplicationStatusList.Any(p => (p.Role == OUTWARDRoles.CREATOR) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userId.Trim()))))
                            {
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.SaveAsDraft))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.SaveAsDraft).IsVisible = false;
                                }
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.Submit))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.Submit).IsVisible = false;
                                }
                            }

                            if ((sectionDetails.Status == FormStatus.SENTBACK || sectionDetails.Status == FormStatus.SUBMITTED || sectionDetails.Status == FormStatus.COMPLETED) &&
                               (sectionHODDetails != null && sectionHODDetails.Action != null && sectionHODDetails.Action == "Approve"))
                            {
                                for (int i = 0; i < approvalMatrix.ApplicationStatusList.Count; i++)
                                {
                                    if (approvalMatrix.ApplicationStatusList[i].Role == OUTWARDRoles.CREATOR && sectionHODDetails != null && sectionHODDetails.Action == "Approve" && approvalMatrix.ApplicationStatusList[i].Status == ApproverStatus.SENDFORWARD)
                                    {
                                        approvalMatrix.ApplicationStatusList[i].Status = ApproverStatus.APPROVED;
                                    }
                                    if (approvalMatrix.ApplicationStatusList[i].Role == OUTWARDRoles.HOD && sectionHODDetails.Action == "Approve")
                                    {
                                        if (approvalMatrix.ApplicationStatusList[i].Status == ApproverStatus.SENDBACK)
                                            approvalMatrix.ApplicationStatusList[i].Status = ApproverStatus.APPROVED;
                                    }
                                }
                            }


                            if ((sectionDetails.Status == FormStatus.SENTBACK) &&
                            (sectionHODDetails != null && sectionHODDetails.Action != null && sectionHODDetails.Action == "Rework") &&
                             (approvalMatrix.ApplicationStatusList.Any(p => (p.Role == OUTWARDRoles.CREATOR) && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userId.Trim()))))
                            {
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.RELEASE))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.RELEASE).IsVisible = false;
                                }
                                if (outwardForm.Buttons.Any(p => p.Name == ButtonCaption.PRINTDECLARATION))
                                {
                                    outwardForm.Buttons.FirstOrDefault(p => p.Name == ButtonCaption.PRINTDECLARATION).IsVisible = false;
                                }
                            }

                        }
                    }

                    var sectionRecepientDetails = outwardForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(OutwardSectionName.RECIPIENTSINGLESECTION)) as RecipientSingleSection;
                    if (sectionRecepientDetails != null)
                    {
                        sectionRecepientDetails.RecipientSingleAttachment =
                            sectionRecepientDetails.Files != null && sectionRecepientDetails.Files.Count > 0
                                ? JsonConvert.SerializeObject(sectionRecepientDetails.Files.Where(x =>
                                        !string.IsNullOrEmpty(sectionRecepientDetails.RecipientSingleAttachment) &&
                                        sectionRecepientDetails.RecipientSingleAttachment.Split(',')
                                            .Contains(x.FileName))
                                    .ToList())
                                : string.Empty;
                        if (itemId == 0)
                            sectionRecepientDetails.OutwardDate = DateTime.Now.ToShortDateString();
                        else
                            sectionRecepientDetails.OutwardDate = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(sectionRecepientDetails.OutwardDate));
                        //if (sectionRecepientDetails.OutwardDate != null)
                        //{
                        //    sectionRecepientDetails.OutwardDate = sectionRecepientDetails.OutwardDate.Value.AddDays(1);
                        //}
                    }

                    contract.Forms.Add(outwardForm);
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
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public ActionStatus SaveBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();
                OutwardNoCount currentOutwardNo = null;
                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = MaterialTrackingListNames.OUTWARDREQUESTSACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.MATERIALTRACKINGAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.OUTWARDASSMBLREQUESTFORM;

                    TesterSection section = null;
                    RecipientSection receipientSection = null;
                    if (sectionDetails.SectionName == OutwardSectionName.TESTERSECTION)
                    {
                        section = sectionDetails as TesterSection;
                        if (string.IsNullOrEmpty(section.OutwardId) && sectionDetails.ActionStatus == ButtonActionStatus.SaveAsDraft)
                        {
                            section.Title = section.OutwardId = "View";
                        }
                        else if (sectionDetails.ActionStatus == ButtonActionStatus.NextApproval && (string.IsNullOrEmpty(section.OutwardId) || section.OutwardId == "View"))
                        {
                            currentOutwardNo = GetOutwardRequestNo();
                            section.RequestDate = string.Format("{0:dd-MM-yyyy}", DateTime.Now);
                            if (currentOutwardNo != null)
                            {
                                currentOutwardNo.CurrentValue = currentOutwardNo.CurrentValue + 1;
                                Logger.Info("Inward Request Current Value + 1 = " + currentOutwardNo.CurrentValue);
                                section.OutwardId = string.Format("OUT-{0} {1}{2}-{3}:{4}-{5}", DateTime.Today.Year, DateTime.Now.Month.ToString("d2"), DateTime.Today.Day.ToString("d2"), DateTime.Now.Hour.ToString("d2"), DateTime.Now.Minute.ToString("d2"), string.Format("{0:0000}", currentOutwardNo.CurrentValue));
                                section.Title = section.OutwardId;
                                Logger.Info("Inward Request No is " + section.OutwardId);
                                status.ExtraData = section.OutwardId;
                            }

                        }
                    }

                    if (sectionDetails.SectionName == OutwardSectionName.RECIPIENTSECTION)
                    {
                        receipientSection = sectionDetails as RecipientSection;
                        if (sectionDetails.ActionStatus == ButtonActionStatus.Complete)
                        {
                            status.ExtraData = receipientSection.OutwardId;
                        }
                    }

                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.FirstOrDefault(p => p.ListName.Equals(MaterialTrackingListNames.OUTWARDREQUESTSLIST));
                    if (sectionDetails.SectionName == OutwardSectionName.TESTERSECTION)
                    {
                        if (itemDetails != null && (itemDetails.ItemId > 0 && currentOutwardNo != null))
                        {
                            UpdateOutwardRequestNoCount(currentOutwardNo);
                            Logger.Info("Update Inward No " + section.OutwardId);
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
                            case ButtonActionStatus.SendBack:
                                status.Messages.Add("Text_Rework");
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

        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public ActionStatus SaveBySingleSection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();
                OutwardNoCount currentOutwardNo = null;
                int inwardId = 0;
                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = MaterialTrackingListNames.OUTWARDSINGLEREQUESTSACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.MATERIALTRACKINGAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.OUTWARDSINGLEREQUESTFORM;

                    TesterSingleSection section = null;
                    RecipientSingleSection receipientSingleSection = null;
                    if (sectionDetails.SectionName == OutwardSectionName.TESTERSINGLESECTION)
                    {
                        section = sectionDetails as TesterSingleSection;
                        if (string.IsNullOrEmpty(section.OutwardId) && sectionDetails.ActionStatus == ButtonActionStatus.SaveAsDraft)
                        {
                            section.Title = section.OutwardId = "View";
                        }
                        else if (sectionDetails.ActionStatus == ButtonActionStatus.NextApproval && (string.IsNullOrEmpty(section.OutwardId) || section.OutwardId == "View"))
                        {
                            currentOutwardNo = GetOutwardRequestNo();
                            section.RequestDate = string.Format("{0:dd-MM-yyyy}", DateTime.Now);
                            if (currentOutwardNo != null)
                            {
                                currentOutwardNo.CurrentValue = currentOutwardNo.CurrentValue + 1;
                                Logger.Info("Inward Request Current Value + 1 = " + currentOutwardNo.CurrentValue);
                                section.OutwardId = string.Format("OUT-{0} {1}{2}-{3}:{4}-{5}", DateTime.Today.Year, DateTime.Now.Month.ToString("d2"), DateTime.Today.Day.ToString("d2"), DateTime.Now.Hour.ToString("d2"), DateTime.Now.Minute.ToString("d2"), string.Format("{0:0000}", currentOutwardNo.CurrentValue));
                                section.Title = section.OutwardId;
                                Logger.Info("Inward Request No is " + section.OutwardId);
                                status.ExtraData = section.OutwardId;
                            }
                        }

                        InwardDetail inwardDetail = this.GetInwardId(section.InwardId);
                        inwardId = inwardDetail.ID;
                        section.ApproversList.ForEach(p =>
                        {
                            if (p.Role == OUTWARDRoles.RECIPIENT1)
                            {
                                p.Approver = inwardDetail.ProposedBy;

                            }
                        });

                        Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                        if (section.ActionStatus == ButtonActionStatus.NextApproval)
                        {
                            values.Add("IsOutwardGenerated", "Yes");
                        }
                        else
                        {
                            values.Add("IsOutwardGenerated", "In Progress");
                        }
                        BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, MaterialTrackingListNames.INWARDLIST, inwardDetail.ID, values);
                        if (section.InwardId != section.InwardIdOld && section.InwardIdOld != null)
                        {
                            var inwardOldDetail = this.GetInwardId(section.InwardIdOld);
                            if (inwardOldDetail.ID > 0)
                            {
                                values = new Dictionary<string, dynamic> { { "IsOutwardGenerated", "No" } };
                                BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web,
                                    MaterialTrackingListNames.INWARDLIST, inwardOldDetail.ID, values);
                            }
                        }
                    }

                    //if (section != null && section.SectionName == OutwardSectionName.TESTERSINGLESECTION)
                    //{
                    //    InwardDetail inwardDetail = this.GetInwardId(section.InwardId);
                    //    inwardId = inwardDetail.ID;
                    //    section.ApproversList.ForEach(p =>
                    //    {
                    //        if (p.Role == OUTWARDRoles.RECIPIENT1)
                    //        {
                    //            p.Approver = inwardDetail.ProposedBy;

                    //        }
                    //    });

                    //    Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                    //    if (section.ActionStatus == ButtonActionStatus.NextApproval)
                    //    {
                    //        values.Add("IsOutwardGenerated", "Yes");
                    //    }
                    //    else
                    //    {
                    //        values.Add("IsOutwardGenerated", "In Progress");
                    //    }
                    //    BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, MaterialTrackingListNames.INWARDLIST, inwardDetail.ID, values);
                    //    if (section.InwardId != section.InwardIdOld && section.InwardIdOld != null)
                    //    {
                    //        var inwardOldDetail = this.GetInwardId(section.InwardIdOld);
                    //        if (inwardOldDetail.ID > 0)
                    //        {
                    //            values = new Dictionary<string, dynamic> { { "IsOutwardGenerated", "No" } };
                    //            BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web,
                    //                MaterialTrackingListNames.INWARDLIST, inwardOldDetail.ID, values);
                    //        }
                    //    }
                    //}

                    if (sectionDetails.SectionName == OutwardSectionName.RECIPIENTSINGLESECTION)
                    {
                        receipientSingleSection = sectionDetails as RecipientSingleSection;
                        InwardDetail inwardDetail = this.GetInwardId(receipientSingleSection.InwardId);
                        inwardId = inwardDetail.ID;
                        if (sectionDetails.ActionStatus == ButtonActionStatus.Complete)
                        {
                            status.ExtraData = receipientSingleSection.OutwardId;
                        }
                    }

                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.FirstOrDefault(p => p.ListName.Equals(MaterialTrackingListNames.OUTWARDSINGLEREQUESTSLIST));
                    if (sectionDetails.SectionName == OutwardSectionName.TESTERSINGLESECTION)
                    {
                        if (itemDetails != null && (itemDetails.ItemId > 0 && currentOutwardNo != null))
                        {
                            UpdateOutwardRequestNoCount(currentOutwardNo);
                            Logger.Info("Update Inward No " + section.OutwardId);
                            if (inwardId > 0)
                            {
                                Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                                values.Add("OutwardNoForReport", section.OutwardId);
                                values.Add("OutwardDate", section.OutwardDate);
                                BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, MaterialTrackingListNames.INWARDLIST, inwardId, values);
                            }
                        }
                    }

                    if (sectionDetails.SectionName == OutwardSectionName.RECIPIENTSINGLESECTION)
                    {
                        if (itemDetails != null && (itemDetails.ItemId > 0))
                        {
                            if (inwardId > 0)
                            {
                                Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                                values.Add("OutwardDate", receipientSingleSection.OutwardDate);
                                BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, MaterialTrackingListNames.INWARDLIST, inwardId, values);
                            }
                        }
                    }


                    if (itemDetails.ItemId > 0)
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
                            case ButtonActionStatus.SendBack:
                                status.Messages.Add("Text_Rework");
                                break;
                            case ButtonActionStatus.Hold:
                                status.Messages.Add("Text_HoldSuccess");
                                status.ItemID = itemDetails.ItemId;
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

        /// <summary>
        /// Gets the u ser detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
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
        public OutwardNoCount GetOutwardRequestNo()
        {
            try
            {
                List<OutwardNoCount> lstdcrCount = new List<OutwardNoCount>();
                List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTNOCOUNT);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View><ViewFields><FieldRef Name='Title' /><FieldRef Name='Year' /><FieldRef Name='CurrentValue' /></ViewFields>   </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    foreach (ListItem item in items)
                    {
                        OutwardNoCount obj = new OutwardNoCount();
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
        public void UpdateOutwardRequestNoCount(OutwardNoCount currentValue)
        {
            if (currentValue != null && currentValue.ID != 0)
            {
                try
                {

                    Logger.Info("Aync update Outward No Current value currentValue : " + currentValue.CurrentValue);
                    List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTNOCOUNT);
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
                    Logger.Error("Error while Update Outward no Current Value");
                    Logger.Error(ex);
                }
            }
        }

        public List<InwardDetail> RetrieveInwardProjectDetails(string project, string userId)
        {
            List<InwardDetail> inward = new List<InwardDetail>();
            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.PROJECTNAMEMASTER);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View> 
                                  <Query> 
                                       <Where>
                                            <And>
                                                 <Eq>
                                                      <FieldRef Name='Testers' LookupId='TRUE'/><Value Type='Integer'>" + userId + @"</Value>
                                                 </Eq>                                           
                                                 <Or>
                                                    <Contains>
                                                       <FieldRef Name='Title' />
                                                       <Value Type='Text'>" + project + @"</Value>
                                                    </Contains>
                                                    <Contains>
                                                       <FieldRef Name='ProjectCode' />
                                                       <Value Type='Text'>" + project + @"</Value>
                                                    </Contains>
                                                 </Or>
                                               </And>
                                      </Where> 
                                </Query> 
                          </View>";


            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count > 0)
            {
                foreach (ListItem item in items)
                {
                    InwardDetail inwardDetail = new InwardDetail();
                    inwardDetail.ProjectName = Convert.ToString(item["Title"]);
                    inwardDetail.ProjectCode = Convert.ToString(item["ProjectCode"]);
                    inward.Add(inwardDetail);
                }
            }
            return inward;

        }
    }
}