using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Pages.Administration.UserRoles
{
    public class UserRolesAdminModel : PageModelBase<UserRoleAdminPageModelService>
    {
        public string Message { get; set; }

        public RoleModels AllCompanyRoles { get; set; }

        public RoleModels SelectedRoles { get; set; }

        public async Task OnGet()
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";

            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }

            await this.PopulateDataSources();
        }

        /// <summary>
        /// Razor handler that is invoked from the AJAX method when the user selects a user from the grid.
        /// Returns a JSON object that represents the selected user. 
        /// </summary>
        /// <param name="useremail">The email of the selected user</param>
        /// <returns>An instance of <see cref="UserModel"/></returns>
        public async Task<JsonResult> OnGetSelectedUser(string useremail)
        {
            JsonResult result = null;
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            var allRoles = HttpContext.Session.Get<RoleModels>(SessionConstants.AllUserRoles);
            var user = await new UserServices().GetUserDetails(useremail, rootUrl);
            HttpContext.Session.Set<UserModel>(SessionConstants.CurrentlySelectedUserRole, user);
            var displayObject = new UserRoleAdminPageModelService().GetRolesForDisplay(user, allRoles);
            result = new JsonResult(displayObject);
            return result;
        }

        public async Task OnPost()
        {
            if (Request.Form.Keys.Contains("submitCancel"))
            {
                OnPostCancel();
            }
            else if (Request.Form.Keys.Contains("submitSave"))
            {
                await OnPostSave();
            }
        }

        public async Task OnPostSave()
        {
            await this.UpdateUserRoles();
            this.CompleteHandler();
        }

        public void OnPostCancel()
        {
            this.CompleteHandler();
        }

        public UserRolesAdminModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }

        private async Task PopulateDataSources()
        {
            this.AllCompanyRoles = new RoleModels();
            this.SelectedRoles = new RoleModels();

            var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);
            string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

            if (HttpContext.Session.Get<RoleModels>(SessionConstants.AllUserRoles) == null)
            {
                var allroles = await new RoleAdminPageModelService().GetRolesByCompanyId(email, rootUrl, encodedId, user.CompanyId);
                if (allroles?.Roles != null && allroles.Roles.Any())
                {
                    this.AllCompanyRoles = allroles;
                    HttpContext.Session.Set<RoleModels>(SessionConstants.AllUserRoles, allroles);
                }
            }
        }

        private void CompleteHandler()
        {
            HttpContext.Session.Set<RoleModels>(SessionConstants.AllUserRoles, null);
            this.AllCompanyRoles = new RoleModels();
            this.SelectedRoles = new RoleModels();
            Response.Redirect("/Administration/Administration");
        }

        private async Task UpdateUserRoles()
        {
            var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentlySelectedUserRole);
            var userRoles = Request.Form["selectedroles"];
            if (!string.IsNullOrEmpty(userRoles))
            {
                var allRoleModels = HttpContext.Session.Get<RoleModels>(SessionConstants.AllUserRoles);
                var roleModels = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(userRoles, allRoleModels);
                user.Roles = roleModels;
            }
            else
            {
                user.Roles = null;
            }
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);
            await new UserAdminPageModelService().UpdateUserDetails(user, encodedId, rootUrl);
        }
    }
}