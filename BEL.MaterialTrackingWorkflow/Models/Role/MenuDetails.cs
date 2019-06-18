namespace BEL.MaterialTrackingWorkflow.Models.Role
{
    using System;
    using System.Runtime.Serialization;

    [DataContract, Serializable]
    public class MenuDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the parent menu.
        /// </summary>
        /// <value>
        /// The parent menu.
        /// </value>
        [DataMember]
        public string ParentMenu { get; set; }

        /// <summary>
        /// Gets or sets the child menu.
        /// </summary>
        /// <value>
        /// The child menu.
        /// </value>
        [DataMember]
        public string childMenu { get; set; }
    }
}