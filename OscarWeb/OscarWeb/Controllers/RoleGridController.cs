using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace OscarWeb.Controllers
{
    public class RoleGridController : BaseGridController
    {
        /// <summary>
        /// Populate the grid with data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditingPopup_Read([DataSourceRequest] DataSourceRequest request)
        {
            string email = base.GetSession().Get<string>(SessionConstants.EmailClaim);
            string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);
            UserModel user = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);

            var result = new List<RoleModel>();

            var roles = await new RoleAdminPageModelService().GetRolesByCompanyId(email, rootUrl, encodedId, user.CompanyId);
            if (roles != null && roles.Roles.Any())
            {
                result = roles.Roles;
            }
            return Json(result.ToDataSourceResult(request));
        }

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Create([DataSourceRequest] DataSourceRequest request, RoleModel role)
        {
            if (!string.IsNullOrEmpty(role?.Name) && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);
                UserModel user = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);

                role.Active = true;
                role.CompanyId = user.CompanyId;
                
                await new RoleAdminPageModelService().CreateRole(role, encodedId, rootUrl);
            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Update([DataSourceRequest] DataSourceRequest request, RoleModel role)
        {
            if (!string.IsNullOrEmpty(role?.Name) && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);
                
                await new RoleAdminPageModelService().UpdateRole(role, encodedId, rootUrl);
            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Destroy([DataSourceRequest] DataSourceRequest request, RoleModel role)
        {
            if (!string.IsNullOrEmpty(role?.Name))
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                await new RoleAdminPageModelService().DeleteRole(role, encodedId, rootUrl);
            }

            return Json(new[] { role }.ToDataSourceResult(request, ModelState));
        }
    }
}
