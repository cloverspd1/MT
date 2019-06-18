using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BEL.MaterialTrackingWorkflow.Models.Role
{
    [DataContract, Serializable]
    public class RoleScreenMappingDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        [DataMember]
        public int? RoleId { get; set; }

        /// <summary>
        /// Get or sets the RoleId
        /// </summary>
        /// <value>
        /// The RoleId.
        /// </value>
        [DataMember]
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the name of the screen.
        /// </summary>
        /// <value>
        /// The name of the screen.
        /// </value>
        [DataMember]
        public string ParentMenuName { get; set; }

        /// <summary>
        /// Gets or sets the name of the screen.
        /// </summary>
        /// <value>
        /// The name of the screen.
        /// </value>
        [DataMember]
        public string ChildMenuName { get; set; }

        /// <summary>
        /// Get or sets the IsAuthorized
        /// </summary>
        /// <value>
        /// The IsAuthorized.
        /// </value>
        [DataMember]
        public bool IsAuthorized { get; set; }
    }  
}