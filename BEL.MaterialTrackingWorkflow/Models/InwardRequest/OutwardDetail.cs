namespace BEL.MaterialTrackingWorkflow.Models.InwardRequest
{
    using BEL.CommonDataContract;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// DCR No 
    /// </summary>
    [DataContract, Serializable]
    public class OutwardDetail
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
        public string OutwardId { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string OutwardItemID { get; set; }

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
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets the Courier Details.
        /// </summary>
        /// <value>
        /// The Courier Details.
        /// </value>
        [DataMember]
        public string CourierDetails { get; set; }

        /// <summary>
        /// Gets or sets the AWD No.
        /// </summary>
        /// <value>
        /// The AWD No.
        /// </value>
        [DataMember]
        public string AWDNo { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the inward identifier.
        /// </summary>
        /// <value>
        /// The inward identifier.
        /// </value>
        [DataMember]
        public string InwardId { get; set; }

    }
}