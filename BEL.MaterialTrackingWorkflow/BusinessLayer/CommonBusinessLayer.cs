namespace BEL.MaterialTrackingWorkflow.BusinessLayer
{

    using BEL.CommonDataContract;
    using BEL.DataAccessLayer;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.Reports;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class CommonBusinessLayer
    {
        /// <summary>
        ///  Lazy Instance
        /// </summary>
        private static readonly Lazy<CommonBusinessLayer> lazy = new Lazy<CommonBusinessLayer>(() => new CommonBusinessLayer());

        /// <summary>
        /// Instance
        /// </summary>
        public static CommonBusinessLayer Instance
        {
            get
            {
                return lazy.Value;
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

        private CommonBusinessLayer()
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            string siteURL = helper.GetSiteURL(SiteURLs.MTSITEURL);
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
        /// Download File
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="applicationName">application Name</param>
        /// <returns>Byte Array</returns>
        public byte[] DownloadFile(string url, string applicationName)
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            ////string siteURL = helper.GetSiteURL(applicationName);
            ////context = helper.CreateClientContext(siteURL);
            return helper.GetFileBytesByUrl(this.context, url);
        }

        /// <summary>
        /// Validates the users.
        /// </summary>
        /// <param name="emailList">The email list.</param>
        /// <returns>list of invalid users</returns>
        public List<string> ValidateUsers(List<string> emailList)
        {
            BELDataAccessLayer helper = new BELDataAccessLayer();
            return helper.GetInvalidUsers(emailList);
        }

        /// <summary>
        /// Removes the cache keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        public void RemoveCacheKeys(List<string> keys)
        {
            if (keys != null && keys.Count != 0)
            {
                foreach (string key in keys)
                {
                    GlobalCachingProvider.Instance.RemoveItem(key);
                }
            }
        }

        /// <summary>
        /// Gets the cache keys.
        /// </summary>
        /// <returns>list of string</returns>
        public List<string> GetCacheKeys()
        {
            return GlobalCachingProvider.Instance.GetAllKeys();
        }

        /// <summary>
        /// Gets the file name list.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>ISection Detail</returns>
        public ISection GetFileNameList(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic dysectionDetails = Convert.ChangeType(sectionDetails, type);
            dysectionDetails.FileNameList = string.Empty;
            if (dysectionDetails.Files != null && dysectionDetails.Files.Count > 0)
            {
                dysectionDetails.FileNameList = JsonConvert.SerializeObject(dysectionDetails.Files);
            }
            return dysectionDetails;
        }


        /// <summary>
        /// Gets the file name list from current approver.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>I Section</returns>
        public ISection GetFileNameListFromCurrentApprover(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic dysectionDetails = Convert.ChangeType(sectionDetails, type);
            dysectionDetails.FileNameList = string.Empty;
            if (dysectionDetails.CurrentApprover != null && dysectionDetails.CurrentApprover.Files != null && dysectionDetails.CurrentApprover.Files.Count > 0)
            {
                dysectionDetails.FileNameList = JsonConvert.SerializeObject(dysectionDetails.CurrentApprover.Files);
            }
            return dysectionDetails;
        }

        public UserDetails GetLoginUserDetail(string id)
        {

            MasterDataHelper masterHelper = new MasterDataHelper();
            List<UserDetails> userInfoList = masterHelper.GetAllEmployee(context, web);
            UserDetails detail = userInfoList.FirstOrDefault(p => p.UserId == id);
            return detail;


        }

        public User getCurrentUser(string userid)
        {
            return BELDataAccessLayer.EnsureUser(this.context, this.web, userid);
        }

        /// <summary>
        /// Gets the report details.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>list of Reportdetails</returns>
        public List<ReportDetails> GetReportDetails(string fieldName, string fieldValue)
        {
            try
            {
                List<ReportDetails> reportList = new List<ReportDetails>();

                #region Inward List

                List inwardList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDLIST);
                if (inwardList != null)
                {
                    string[] arrFilter = !string.IsNullOrWhiteSpace(fieldValue) ? fieldValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
                    string appendWhereClause = string.Empty;
                    if (!string.IsNullOrWhiteSpace(fieldValue) && !fieldName.ToLower().Contains("date"))
                    {
                        //appendWhereClause = @"<Where>
                        //                          <Eq>
                        //                             <FieldRef Name='" + fieldName + @"' />
                        //                             <Value Type='Text'>" + fieldValue + @"</Value>
                        //                          </Eq>
                        //                       </Where>";

                        string paraValue = string.Empty;

                        foreach (string item in arrFilter)
                        {
                            paraValue += "<Value Type='Text'>" + item + @"</Value>";
                        }

                        if (!string.IsNullOrWhiteSpace(paraValue))
                        {
                            appendWhereClause = @"<Where>
                                        <In>
                                            <FieldRef Name='" + fieldName + @"' />
                                            <Values>
                                            " + paraValue + @"
                                            </Values>
                                        </In>
                                    </Where>";
                        }

                    }
                    else if (!string.IsNullOrWhiteSpace(fieldValue) && fieldName.ToLower().Contains("date"))
                    {
                        appendWhereClause = @"<Where>
                                                 <Or>
                                                     <Eq>
                                                        <FieldRef Name='RequestDate' />
                                                        <Value Type='Text'>" + fieldValue + @"</Value>
                                                     </Eq>
                                                     <Eq>
                                                        <FieldRef Name='OutwardDate' />
                                                        <Value Type='Text'>" + fieldValue + @"</Value>
                                                     </Eq>
                                                  </Or>   
                                               </Where>";
                    }
                    CamlQuery inwardQuery = new CamlQuery();
                    inwardQuery.ViewXml = @"<View>
                                           <Query>
                                             " + appendWhereClause + @"
                                               <OrderBy>
                                                  <FieldRef Name='InwardID' Ascending='True' />
                                               </OrderBy>
                                        </Query>
                                    </View>";
                    ListItemCollection items = inwardList.GetItems(inwardQuery);
                    this.context.Load(items);
                    this.context.ExecuteQuery();

                    if (items != null && items.Count > 0)
                    {
                        foreach (ListItem item in items)
                        {
                            ReportDetails reportDetails = new ReportDetails();
                            reportDetails.ProjectName = Convert.ToString(item["ProjectName"]);
                            reportDetails.MaterialCategory = Convert.ToString(item["MaterialCategory"]);
                            reportDetails.BUName = Convert.ToString(item["BUName"]);
                            reportDetails.ProductCategory = Convert.ToString(item["ProductCategory"]);
                            reportDetails.InwardId = Convert.ToString(item["InwardID"]);
                            reportDetails.SerialNumber = Convert.ToString(item["SerialNo"]);
                            reportDetails.MaterialType = Convert.ToString(item["TypeofMaterial"]);
                            reportDetails.MaterialLocation = Convert.ToString(item["MaterialLocation"]);
                            reportDetails.Tester = Convert.ToString(item["MaterialHandedoverto"]);
                            reportDetails.TesterAssignedDate = !string.IsNullOrWhiteSpace(Convert.ToString(item["TesterAssignedDate"])) ? Convert.ToDateTime(item["TesterAssignedDate"]).ToShortDateString() : "-";
                            reportDetails.IsOutwardDone = Convert.ToString(item["IsOutwardGenerated"]);
                            reportDetails.InwardDate = Convert.ToString(item["RequestDate"]);
                            reportDetails.OutwardDate = Convert.ToString(item["OutwardDate"]);

                            reportDetails.OutwardId = Convert.ToString(item["OutwardNoForReport"]);
                            reportList.Add(reportDetails);
                        }
                    }
                }

                #endregion

                #region Outward Assembled

                List outwardList = this.web.Lists.GetByTitle(MaterialTrackingListNames.OUTWARDREQUESTSLIST);
                if (outwardList != null)
                {
                    CamlQuery query = new CamlQuery();
                    string appendWhereClause = string.Empty;

                    if ((!string.IsNullOrWhiteSpace(fieldName)) && fieldName.ToLower().Contains("date"))
                    {
                        if (!string.IsNullOrWhiteSpace(fieldValue))
                        {
                            appendWhereClause = @"<Where>
                                                      <Eq>
                                                         <FieldRef Name='OutwardDate' />
                                                         <Value Type='Text'>" + fieldValue + @"</Value>
                                                      </Eq>
                                                   </Where>";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fieldValue) && fieldName.ToLower().Contains("date"))
                    {
                        query.ViewXml = @"<View>
                                                <Query>
                                                   " + appendWhereClause + @" 
                                                   <OrderBy>
                                                      <FieldRef Name='OutwardId' Ascending='True' />
                                                   </OrderBy>
                                                </Query>
                                           </View>";
                    }
                    else if (string.IsNullOrWhiteSpace(fieldValue))
                    {
                        query.ViewXml = @"<View>
                                                <Query>
                                                   " + appendWhereClause + @" 
                                                   <OrderBy>
                                                      <FieldRef Name='OutwardId' Ascending='True' />
                                                   </OrderBy>
                                                </Query>
                                           </View>";
                    }
                    if (query.ViewXml != null)
                    {
                        ListItemCollection items1 = outwardList.GetItems(query);
                        this.context.Load(items1);
                        this.context.ExecuteQuery();
                        if (items1 != null && items1.Count > 0)
                        {
                            foreach (ListItem item in items1)
                            {
                                ReportDetails reportDetails = new ReportDetails();

                                reportDetails.MaterialType = Convert.ToString(item["TypeofMaterial"]);
                                reportDetails.Tester = Convert.ToString(item["ProposedByAlise"]);
                                reportDetails.TesterAssignedDate = Convert.ToString(item["RequestDate"]);
                                reportDetails.OutwardId = Convert.ToString(item["OutwardId"]);
                                reportDetails.OutwardDate = Convert.ToString(item["OutwardDate"]);
                                reportList.Add(reportDetails);
                            }
                        }
                    }
                }


                #endregion

                return reportList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Gets the material location list.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<NameValueData> GetMaterialLocationList()
        {
            List<NameValueData> materialLocations = new List<NameValueData>();
            List materialLocationList = this.web.Lists.GetByTitle(MaterialTrackingListNames.MATERIALLOCATION);
            if (materialLocationList != null)
            {
                CamlQuery materialLocationQuery = new CamlQuery();
                materialLocationQuery.ViewXml = @"<View>
                                                     <Query>
                                                       <OrderBy>
                                                          <FieldRef Name='Title' Ascending='True' />
                                                       </OrderBy>
                                                     </Query>                                      
                                                   </View>";
                ListItemCollection items = materialLocationList.GetItems(materialLocationQuery);
                this.context.Load(items);
                this.context.ExecuteQuery();

                if (items != null && items.Count > 0)
                {
                    foreach (ListItem item in items)
                    {
                        NameValueData materialLocation = new NameValueData();
                        materialLocation.Name = Convert.ToString(item["ID"]);
                        materialLocation.Value = Convert.ToString(item["Title"]);
                        materialLocations.Add(materialLocation);
                    }
                }
            }
            return materialLocations;
        }

        /// <summary>
        /// Gets the bu list.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<NameValueData> GetBUList()
        {
            List<NameValueData> buNames = new List<NameValueData>();
            List buNameList = this.web.Lists.GetByTitle(MaterialTrackingListNames.BUSINESSUNITMASTER);
            if (buNameList != null)
            {
                CamlQuery buQuery = new CamlQuery();
                buQuery.ViewXml = @"<View>
                                        <Query>                                           
                                            <OrderBy>
                                                <FieldRef Name='Title' Ascending='True' />
                                            </OrderBy>
                                        </Query>                                      
                                    </View>";
                ListItemCollection items = buNameList.GetItems(buQuery);
                this.context.Load(items);
                this.context.ExecuteQuery();

                if (items != null && items.Count > 0)
                {
                    foreach (ListItem item in items)
                    {
                        NameValueData buName = new NameValueData();
                        buName.Name = Convert.ToString(item["ID"]);
                        buName.Value = Convert.ToString(item["Title"]);
                        buNames.Add(buName);
                    }
                }
            }
            return buNames;
        }

        /// <summary>
        /// Gets the serial numbers.
        /// </summary>
        /// <param name="serialNumber">The serial number.</param>
        /// <returns></returns>
        public string GetSerialNumbers(string strSerialNumber)
        {
            List<NameValueData> serialNumbers = new List<NameValueData>();
            List spList = this.web.Lists.GetByTitle(MaterialTrackingListNames.INWARDLIST);
            CamlQuery query = new CamlQuery();
            query.ViewXml = @"<View>
                                  <Query>
                                        <Where>                                            
                                            <Contains>
                                                <FieldRef Name='SerialNo' />
                                                <Value Type='Text'>" + strSerialNumber + @"</Value>
                                            </Contains>                                         
                                        </Where>
                                        <FieldRef Name='SerialNo' />
                                        <OrderBy>
                                            <FieldRef Name='SerialNo' Ascending='True' />
                                        </OrderBy>                                      
                                   </Query> <RowLimit>15</RowLimit>      
                             </View>";
            ListItemCollection items = spList.GetItems(query);
            this.context.Load(items);
            this.context.ExecuteQuery();
            if (items != null && items.Count != 0)
            {
                foreach (ListItem item in items)
                {
                    NameValueData serialNumber = new NameValueData();
                    serialNumber.Name = Convert.ToString(item["SerialNo"]);
                    serialNumber.Value = Convert.ToString(item["SerialNo"]);
                    serialNumbers.Add(serialNumber);
                }
            }

            return JSONHelper.Serialize<List<NameValueData>>(serialNumbers);
        }

    }
}