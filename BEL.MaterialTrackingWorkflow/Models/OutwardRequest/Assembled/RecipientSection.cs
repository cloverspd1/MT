namespace BEL.MaterialTrackingWorkflow.Models.OutwardRequest
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class RecipientSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        public RecipientSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public RecipientSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(MaterialTrackingListNames.OUTWARDREQUESTSLIST, true) };
                this.SectionName = OutwardSectionName.RECIPIENTSECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                ////Add All Master Object which required for this Section.
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new CourierTypeMaster());
                this.MasterData.Add(new DeclarationTextMaster());

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
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Outward Id.
        /// </summary>
        /// <value>
        /// The Outward Id.
        /// </value>
        [DataMember]
        public string OutwardId { get; set; }

        /// <summary>
        /// Gets or sets the Outward Date.
        /// </summary>
        /// <value>
        /// The Outward Date.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string OutwardDate { get; set; }
        /// <summary>
        /// Gets or sets the Location/Address.
        /// </summary>
        /// <value>
        /// The Location/Address.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string LocationAddress { get; set; }

        /// <summary>
        /// Gets or sets the Courier Details.
        /// </summary>
        /// <value>
        /// The Courier Details.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string CourierDetails { get; set; }

        /// <summary>
        /// Gets or sets the AWD No.
        /// </summary>
        /// <value>
        /// The AWD No.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string AWDNo { get; set; }

        /// <summary>
        /// Gets or sets the Recipient Attachment.
        /// </summary>
        /// <value>
        /// The Recipient Attachment.
        /// </value>
        [DataMember]
        public string RecipientAttachment { get; set; }

        /// <summary>
        /// Gets or sets the Request Type.
        /// </summary>
        /// <value>
        /// The Request Type.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string RequestType { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string WorkflowStatus { get; set; }


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
        [DataMember, IsListColumn(false), IsApproverDetails(true, MaterialTrackingListNames.OUTWARDREQUESTSAPPAPPROVALMATRIX)]
        public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, MaterialTrackingListNames.OUTWARDREQUESTSAPPAPPROVALMATRIX)]
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