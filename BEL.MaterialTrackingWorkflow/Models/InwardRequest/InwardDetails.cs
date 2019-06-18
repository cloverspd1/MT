using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BEL.MaterialTrackingWorkflow.Models.InwardRequest
{
    /// <summary>
    /// Inward Details
    /// </summary>

    [DataContract, Serializable]
    public class InwardDetails
    {      
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the Type of Material.
        /// </summary>
        /// <value>
        /// The Type of Material.
        /// </value>
        [DataMember]
        public string TypeofMaterial { get; set; }
    
    
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
        /// Gets or sets the Recipient 1 Particulars.
        /// </summary>
        /// <value>
        /// The Recipient 1 Particulars.
        /// </value>
        [DataMember]
        public string Particulars { get; set; }

        /// <summary>
        /// Gets or sets the Recipient 2 Particulars.
        /// </summary>
        /// <value>
        /// The Recipient 2 Particulars.
        /// </value>
        [DataMember]
        public string Recipient2Particulars { get; set; }

    }

}