namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Field Column Name Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class FieldColumnNameAttribute : Attribute
    {
        /// <summary>
        /// The _fields information
        /// </summary>
        private string fieldsInformationstr;

        /// <summary>
        /// The _is lookup
        /// </summary>
        private bool isLookup;

        /// <summary>
        /// The lookup field name
        /// </summary>
        private string lookupFieldName = "Title";

        /// <summary>
        /// The lookup field name for task
        /// </summary>
        private string lookupFieldNameForTask = "ID";

        /// <summary>
        /// The lookup field name for trans
        /// </summary>
        private string lookupFieldNameForTrans = "ID";

        /// <summary>
        /// The is multiple lookup
        /// </summary>
        private bool isMultipleLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldColumnNameAttribute"/> class.
        /// </summary>
        /// <param name="fieldsInformation">The fields information.</param>
        public FieldColumnNameAttribute(string fieldsInformation)
        {
            fieldsInformationstr = fieldsInformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldColumnNameAttribute" /> class.
        /// </summary>
        /// <param name="fieldsInformation">The fields information.</param>
        /// <param name="isLookup">if set to <c>true</c> [is lookup].</param>
        /// <param name="isMultipleLookup">if set to <c>true</c> [is multiple lookup].</param>
        /// <param name="lookupFieldName">Name of the lookup field.</param>
        /// <param name="lookupFieldNameForTask">The lookup field name for task.</param>
        /// <param name="lookupFieldNameForTrans">The lookup field name for trans.</param>
        public FieldColumnNameAttribute(string fieldsInformation, bool isLookup, bool isMultipleLookup = false, string lookupFieldName = "Title", string lookupFieldNameForTask = "ID", string lookupFieldNameForTrans = "ID")
        {
            fieldsInformationstr = fieldsInformation;
            this.isLookup = isLookup;
            this.isMultipleLookup = isMultipleLookup;
            this.lookupFieldName = lookupFieldName;
            this.lookupFieldNameForTask = lookupFieldNameForTask;
            this.lookupFieldNameForTrans = lookupFieldNameForTrans;
        }

        /// <summary>
        /// Gets the fields information.
        /// </summary>
        /// <value>
        /// The fields information.
        /// </value>
        public string FieldsInformation
        {
            get
            {
                return fieldsInformationstr;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is lookup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is lookup; otherwise, <c>false</c>.
        /// </value>
        public bool IsLookup
        {
            get
            {
                return isLookup;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is multiple lookup.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multiple lookup; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultipleLookup
        {
            get
            {
                return isMultipleLookup;
            }
        }

        /// <summary>
        /// Gets the name of the lookup field.
        /// </summary>
        /// <value>
        /// The name of the lookup field.
        /// </value>
        public string LookupFieldName
        {
            get
            {
                return lookupFieldName;
            }
        }

        /// <summary>
        /// Gets the lookup field name for task.
        /// </summary>
        /// <value>
        /// The lookup field name for task.
        /// </value>
        public string LookupFieldNameForTask
        {
            get
            {
                return lookupFieldNameForTask;
            }
        }

        /// <summary>
        /// Gets the lookup field name for trans.
        /// </summary>
        /// <value>
        /// The lookup field name for trans.
        /// </value>
        public string LookupFieldNameForTrans
        {
            get
            {
                return lookupFieldNameForTrans;
            }
        }
    }
}
