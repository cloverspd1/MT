namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is Approver Details Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsApproverDetailsAttribute : Attribute
    {
        /// <summary>
        /// The is approver details field
        /// </summary>
        private bool isApproverDetailsField = true;

        /// <summary>
        /// The approver matrix list name
        /// </summary>
        private string approverMatrixListName;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsApproverDetailsAttribute"/> class.
        /// </summary>
        /// <param name="isApproverDetailsField">if set to <c>true</c> [is approver details field].</param>
        /// <param name="approverMatrixListName">Name of the approver matrix list.</param>
        public IsApproverDetailsAttribute(bool isApproverDetailsField, string approverMatrixListName)
        {
            this.isApproverDetailsField = isApproverDetailsField;
            this.approverMatrixListName = approverMatrixListName;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is approver details field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is approver details field; otherwise, <c>false</c>.
        /// </value>
        public bool IsApproverDetailsField
        {
            get
            {
                return isApproverDetailsField;
            }
        }

        /// <summary>
        /// Gets the name of the approver matrix list.
        /// </summary>
        /// <value>
        /// The name of the approver matrix list.
        /// </value>
        public string ApproverMatrixListName
        {
            get
            {
                return approverMatrixListName;
            }
        }
    }
}