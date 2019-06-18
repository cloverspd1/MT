namespace BEL.MaterialTrackingWorkflow.Models.Reports
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract, Serializable]
    public class ReportDetails
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the material category.
        /// </summary>
        /// <value>
        /// The material category.
        /// </value>
        [DataMember]
        public string MaterialCategory { get; set; }

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
        /// Gets or sets the inward identifier.
        /// </summary>
        /// <value>
        /// The inward identifier.
        /// </value>
        [DataMember]
        public string InwardId { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the material.
        /// </summary>
        /// <value>
        /// The type of the material.
        /// </value>
        [DataMember]
        public string MaterialType { get; set; }

        /// <summary>
        /// Gets or sets the material location.
        /// </summary>
        /// <value>
        /// The material location.
        /// </value>
        [DataMember]
        public string MaterialLocation { get; set; }

        /// <summary>
        /// Gets or sets the tester.
        /// </summary>
        /// <value>
        /// The tester.
        /// </value>
        [DataMember]
        public string Tester { get; set; }

        /// <summary>
        /// Gets or sets the tester assigned date.
        /// </summary>
        /// <value>
        /// The tester assigned date.
        /// </value>
        [DataMember]
        public string TesterAssignedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is outward done.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is outward done; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public string IsOutwardDone { get; set; }

        /// <summary>
        /// Gets or sets the outward identifier.
        /// </summary>
        /// <value>
        /// The outward identifier.
        /// </value>
        [DataMember]
        public string OutwardId { get; set; }

        /// <summary>
        /// Gets or sets the inward date.
        /// </summary>
        /// <value>
        /// The inward date.
        /// </value>
        [DataMember]
        public string InwardDate { get; set; }

        /// <summary>
        /// Gets or sets the outward date.
        /// </summary>
        /// <value>
        /// The outward date.
        /// </value>
        [DataMember]
        public string OutwardDate { get; set; }



    }
}