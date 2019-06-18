namespace BEL.MaterialTrackingWorkflow.Models.Master
{
    using BEL.CommonDataContract;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Department Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class DeclarationTextMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("Declaration")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("Declaration")]
        public string Value { get; set; }
    }
}