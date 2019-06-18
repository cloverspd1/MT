namespace BEL.MaterialTrackingWorkflow.Models.InwardRequest
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class Recipient1Section : ISection
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        public Recipient1Section()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public Recipient1Section(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(MaterialTrackingListNames.INWARDLIST, true) };
                this.SectionName = INWARDSectionName.RECIPIENT1SECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                ////Add All Master Object which required for this Section.
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new TypeofMaterialMaster());
                this.MasterData.Add(new CourierTypeMaster());
              
                this.MasterData.Add(new EmployeeDetailsMaster());
            }
        }

        /// <summary>
        /// Gets or sets the master data.
        /// </summary>
        /// <value>
        /// The master data.
        /// </value>
        [DataMember, IsListColumn(false), ContainsMasterData(true)]
        public List<IMaster> MasterData { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the Inward ID.
        /// </summary>
        /// <value>
        /// The Inward ID.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Inward ID.
        /// </summary>
        /// <value>
        /// The Inward ID.
        /// </value>
        [DataMember]
        public string InwardID { get; set; }

        /// <summary>
        /// Gets or sets the Inward ID.
        /// </summary>
        /// <value>
        /// The Inward ID.
        /// </value>
        [DataMember,Required, IsListColumn(false)]
        public bool NewInward { get; set; }

        /// <summary>
        /// Gets or sets the Inward Id.
        /// </summary>
        /// <value>
        /// The Inward Id.
        /// </value>
        [DataMember,Required, RequiredOnDraft]
        public string OutwardId { get; set; }

        /// <summary>
        /// Gets or sets the Inward Id.
        /// </summary>
        /// <value>
        /// The Inward Id.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string OutwardIdOld { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("ProposedBy"), IsViewer]
        public string ProposedByName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true, false), FieldColumnName("Recipient2User"),IsViewer]
        public string Recipient2 { get; set; }

        ///// <summary>
        ///// Gets or sets the request date.
        ///// </summary>
        ///// <value>
        ///// The request date.
        ///// </value>
        //[DataMember, IsPerson(true, true, true), FieldColumnName("Recipient2User"), IsViewer]
        //public string Recipient2UserName { get; set; }

        /// <summary>
        /// Gets or sets the Recipient2Alise.
        /// </summary>
        /// <value>
        /// The Recipient2Alise.
        /// </value>
        [DataMember]
        public string Recipient2Alise { get; set; }

        /// <summary>
        /// Gets or sets the Recipient2Alise.
        /// </summary>
        /// <value>
        /// The Recipient2Alise.
        /// </value>
        [DataMember]
        public string ProposedByAlise { get; set; }
        

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Request Date.
        /// </summary>
        /// <value>
        /// The Request Date.
        /// </value>
        [DataMember]
        public string RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the Type of Material.
        /// </summary>
        /// <value>
        /// The Type of Material.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string TypeofMaterial { get; set; }

        /// <summary>
        /// Gets or sets the Sender Name.
        /// </summary>
        /// <value>
        /// The Sender Name.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        /// <value>
        /// The Location.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Courier Name.
        /// </summary>
        /// <value>
        /// The Courier Name.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string CourierNameHandDelivery { get; set; }

        /// <summary>
        /// Gets or sets the AWD No.
        /// </summary>
        /// <value>
        /// The AWD No.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string AWBNameofdeliveryperson { get; set; }

        /// <summary>
        /// Gets or sets the Particulars.
        /// </summary>
        /// <value>
        /// The Particulars.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string Particulars { get; set; }

        /// <summary>
        /// Gets or sets the Recipient 1 Attachment.
        /// </summary>
        /// <value>
        /// The Recipient 1 Attachment.
        /// </value>
        [DataMember,Required]
        public string Recipient1Attachment { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string WorkflowStatus { get; set; }


        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string NextApproverAlias { get; set; }
        


        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ApplicationBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember, IsListColumn(false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        [DataMember, IsFile(true)]
        public List<FileDetails> Files { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FileNameList { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ButtonActionStatus ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverDetails(true, MaterialTrackingListNames.INWARDAPPAPPROVALMATRIX)]
        public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, MaterialTrackingListNames.INWARDAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }
    }
}