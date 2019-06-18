using BEL.CommonDataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BEL.MaterialTrackingWorkflow.Models.Role
{
    [DataContract, Serializable]
    public class RoleActivities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleActivitySecurity"/> class.
        /// </summary>
        public RoleActivities()
        {
            RoleList = new List<NameValueData>();
            MenuList = new List<MenuDetails>();
            RoleActivityData = new List<RoleScreenMappingDetails>();
        }

        [DataMember]
        public List<NameValueData> RoleList { get; set; }

        /// <summary>
        /// Gets or sets the menu list.
        /// </summary>
        /// <value>
        /// The menu list.
        /// </value>
        [DataMember]
        public List<MenuDetails> MenuList { get; set; }

        /// <summary>
        /// Gets or sets the role activity data.
        /// </summary>
        /// <value>
        /// The role activity data.
        /// </value>
        [DataMember]
        public List<RoleScreenMappingDetails> RoleActivityData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is all menu authorized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is all menu authorized; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsAllMenuAuthorized { get; set; }
    }
}