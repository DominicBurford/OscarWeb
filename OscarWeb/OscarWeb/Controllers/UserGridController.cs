using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using OscarWeb.Services;
using OscarWeb.Constants;
using OscarWeb.Extensions;

using Common.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace OscarWeb.Controllers
{
    /// <summary>
    /// The controller that is responsible for data operations on the Kendo grid on the User Admin form.
    ///  </summary>
    public class UserGridController : BaseGridController
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
            var user = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);

            var result = new List<UserModel>();

            var users = await new UserAdminPageModelService().GetUsersByCompanyId(email, rootUrl, encodedId, user.CompanyId);
            if (users?.Users != null && users.Users.Any())
            {
                result = users.Users;
            }
            return Json(result.ToDataSourceResult(request));
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Create([DataSourceRequest] DataSourceRequest request, UserModel user)
        {
            if (user != null && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                var sendUser = this.SupplyDefaultValues(user, true);

                await new UserAdminPageModelService().CreateUser(sendUser, encodedId, rootUrl);
            }

            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Update([DataSourceRequest] DataSourceRequest request, UserModel user)
        {
            if (user != null && ModelState.IsValid)
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                var sendUser = this.SupplyDefaultValues(user, false);

                await new UserAdminPageModelService().UpdateUserDetails(sendUser, encodedId, rootUrl);
            }

            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [AcceptVerbs("Post")]
        public async Task<ActionResult> EditingPopup_Destroy([DataSourceRequest] DataSourceRequest request, UserModel user)
        {
            if (!string.IsNullOrEmpty(user?.Email))
            {
                string rootUrl = base.GetSession().Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = base.GetSession().Get<string>(SessionConstants.EncodedUserId);

                await new UserAdminPageModelService().DeleteUser(user.Email, encodedId, rootUrl);
            }

            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        private UserModel SupplyDefaultValues(UserModel model, bool newUser)
        {
            if (model == null) return null;
            var currentUser = base.GetSession().Get<UserModel>(SessionConstants.CurrentUser);
            model.CompanyId = currentUser.CompanyId;
            model.Active = true;
            if (newUser)
            {
                model.Id = 0;
            }
            return model;
        }
    }
}