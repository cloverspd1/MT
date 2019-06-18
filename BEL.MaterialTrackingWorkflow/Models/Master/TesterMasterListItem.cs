namespace BEL.MaterialTrackingWorkflow.Models.Master
{
    using BEL.CommonDataContract;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Department Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class TesterMasterListItem 
    {
       
        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        /// <value>
        /// The Role.
        /// </value>

        [DataMember]
        public string AliasNames { get; set; }

        /// <summary>
        /// Gets or sets the Role.
        /// </summary>
        /// <value>
        /// The Role.
        /// </value>

        [DataMember]
        public string UserID { get; set; }

       
    }
}