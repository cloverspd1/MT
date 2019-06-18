using BEL.CommonDataContract;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using BEL.MaterialTrackingWorkflow.Models.Role;
using System;

namespace BEL.MaterialTrackingWorkflow.Controllers.Role
{
    public class RoleActivityController : RoleActivityBaseController
    {
        // GET: RoleActivity
        public ActionResult Index()
        {
            RoleActivities role = new RoleActivities();
            role.RoleList = this.GetAllRoles();
            return View("RoleActivity", role);
        }

        /// <summary>
        /// Gets the role activity by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>        
        public ActionResult GetRoleActivityByRoleId(string roleName)
        {
            List<RoleScreenMappingDetails> RoleActivities = this.GetRoleActivityDetailsByRoleId(roleName);
            RoleActivities roleActivitySecurity = new RoleActivities();
            roleActivitySecurity.RoleActivityData = RoleActivities;
            roleActivitySecurity.IsAllMenuAuthorized = roleActivitySecurity.RoleActivityData.FindAll(m => m.IsAuthorized == false).Count() > 0 ? false : true;
            return View("_RoleActivity", roleActivitySecurity);
        }

        /// <summary>
        /// Updates the role activity.
        /// </summary>
        /// <param name="lstRoleActivity">The LST role activity.</param>
        /// <returns></returns>
        public JsonResult UpdateRoleActivity(List<RoleScreenMappingDetails> lstRoleActivity)
        {
            ActionStatus status = this.UpdateRoleWiseActivity(lstRoleActivity);
            return Json(status);
        }

        /// <summary>
        /// Adds the new role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public JsonResult AddNewRole(string roleName)
        {
            ActionStatus status = this.AddRole(roleName);
            return Json(status);
        }

        /// <summary>
        /// Deletes the role by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteRoleById(int id)
        {
            ActionStatus status = this.DeleteRole(id);
            return Json(status);
        }
    }
}