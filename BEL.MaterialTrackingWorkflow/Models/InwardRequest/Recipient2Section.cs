namespace BEL.MaterialTrackingWorkflow.Models.InwardRequest
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class Recipient2Section : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCRInchargeNavigatorSection"/> class.
        /// </summary>
        public Recipient2Section() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCRInchargeNavigatorSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public Recipient2Section(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(MaterialTrackingListNames.INWARDLIST, true) };
                this.SectionName = INWARDSectionName.RECIPIENT2SECTION;
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                ////Add All Master Object which required for this Section.
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new TypeofMaterialMaster());
                this.MasterData.Add(new BusinessUnitMaster());
                this.MasterData.Add(new ProductCategoryMaster());
                this.MasterData.Add(new ProjectNameMaster());
                this.MasterData.Add(new MaterialCategoryMaster());
                this.MasterData.Add(new MaterialLocationMaster());

                this.TesterList = new List<TesterMasterListItem>();
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
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the BU Name.
        /// </summary>
        /// <value>
        /// The BU Name.
        /// </value>
        [DataMember, Required]
        public string BUName { get; set; }

        /// <summary>
        /// Gets or sets the Recipient2 Particulars.
        /// </summary>
        /// <value>
        /// The Recipient2 Particulars.
        /// </value>
        [DataMember, Required]
        public string Recipient2Particulars { get; set; }

        /// <summary>
        /// Gets or sets the Product Category.
        /// </summary>
        /// <value>
        /// The Product Category.
        /// </value>
        [DataMember, Required]
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the Type of Material.
        /// </summary>
        /// <value>
        /// The Type of Material.
        /// </value>
        [DataMember, Required]
        public string TypeofMaterial { get; set; }

        /// <summary>
        /// Gets or sets the Material Category.
        /// </summary>
        /// <value>
        /// The Material Category.
        /// </value>
        [DataMember, Required]
        public string MaterialCategory { get; set; }

        /// <summary>
        /// Gets or sets the Material Location.
        /// </summary>
        /// <value>
        /// The Material Location.
        /// </value>
        [DataMember, Required]
        public string MaterialLocation { get; set; }

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        /// <value>
        /// The Project Name.
        /// </value>
        [DataMember, Required]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the Material Handed over to.
        /// </summary>
        /// <value>
        /// The Material Handed over to.
        /// </value>
        [DataMember, Required]
        public string MaterialHandedoverto { get; set; }

        /// <summary>
        /// Gets or sets the Courier Name.
        /// </summary>
        /// <value>
        /// The Courier Name.
        /// </value>
        [DataMember]
        public string SerialNo { get; set; }

        /// <summary>
        /// Gets or sets the Particulars.
        /// </summary>
        /// <value>
        /// The Particulars.
        /// </value>
        [DataMember, Required]
        public string Particulars { get; set; }

        /// <summary>
        /// Gets or sets the Recipient 1 Attachment.
        /// </summary>
        /// <value>
        /// The Recipient 1 Attachment.
        /// </value>
        [DataMember]
        public string Recipient2Attachment { get; set; }

        /// <summary>
        /// Gets or sets the Material Handed over to.
        /// </summary>
        /// <value>
        /// The Material Handed over to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string MaterialHandedovertoUserID { get; set; }

         [DataMember, IsListColumn(false)]
        public string MaterialHandedovertoAliasNames { get; set; } 

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
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<TesterMasterListItem> TesterList { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }

        /// <summary>
        /// Gets or sets the bar code image.
        /// </summary>
        /// <value>
        /// The bar code image.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string BarCodeImage { get; set; }

        /// <summary>
        /// Gets or sets the inward identifier.
        /// </summary>
        /// <value>
        /// The inward identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string InwardId { get; set; }

        /// <summary>
        /// Gets or sets the tester assigned date.
        /// </summary>
        /// <value>
        /// The tester assigned date.
        /// </value>
        [DataMember, DataType(DataType.Date)]
        public DateTime? TesterAssignedDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the outward identifier.
        /// </summary>
        /// <value>
        /// The type of the outward identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string OutwardIdType { get; set; }
    }
}