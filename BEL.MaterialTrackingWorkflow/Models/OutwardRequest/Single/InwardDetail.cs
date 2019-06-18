namespace BEL.MaterialTrackingWorkflow.Models.OutwardRequest
{
    using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using BEL.MaterialTrackingWorkflow.Models.Master;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// DCR No 
    /// </summary>
    [DataContract, Serializable]
    public class InwardDetail
    {
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false)]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string InwardID { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string ProjectCode { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string InwardItemID { get; set; }

        /// <summary>
        /// Gets or sets the Type of Material.
        /// </summary>
        /// <value>
        /// The Type of Material.
        /// </value>
        [DataMember]
        public string TypeofMaterial { get; set; }

        /// <summary>
        /// Gets or sets the Sender Name.
        /// </summary>
        /// <value>
        /// The Sender Name.
        /// </value>
        [DataMember]
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        /// <value>
        /// The Location.
        /// </value>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Particulars.
        /// </summary>
        /// <value>
        /// The Particulars.
        /// </value>
        [DataMember]
        public string Particulars { get; set; }

        /// <summary>
        /// Gets or sets the Particulars.
        /// </summary>
        /// <value>
        /// The Particulars.
        /// </value>
        [DataMember]
        public string Recipient2Particulars { get; set; }

        /// <summary>
        /// Gets or sets the inward date.
        /// </summary>
        /// <value>
        /// The inward date.
        /// </value>
        [DataMember]
        public string InwardDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the recipient2.
        /// </summary>
        /// <value>
        /// The name of the recipient2.
        /// </value>
        [DataMember]
        public string Recipient2Alise { get; set; }

        /// <summary>
        /// Gets or sets the material category.
        /// </summary>
        /// <value>
        /// The material category.
        /// </value>
        [DataMember]
        public string MaterialCategory { get; set; }

        /// <summary>
        /// Gets or sets the Material Location.
        /// </summary>
        /// <value>
        /// The Material Location.
        /// </value>
        [DataMember]
        public string MaterialLocation { get; set; }

        /// <summary>
        /// Gets or sets the Material Handed over to.
        /// </summary>
        /// <value>
        /// The Material Handed over to.
        /// </value>
        [DataMember]
        public string MaterialHandedoverto { get; set; }

        /// <summary>
        /// Gets or sets the name of the bu.
        /// </summary>
        /// <value>
        /// The name of the bu.
        /// </value>
        [DataMember]
        public string BUName { get; set; }

        /// <summary>
        /// Gets or sets the product category.
        /// </summary>
        /// <value>
        /// The product category.
        /// </value>
        [DataMember]
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the proposed by alise.
        /// </summary>
        /// <value>
        /// The proposed by alise.
        /// </value>
        [DataMember]
        public string ProposedByAlise { get; set; }

    }
}