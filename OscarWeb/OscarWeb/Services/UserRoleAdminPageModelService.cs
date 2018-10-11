using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Extensions.Primitives;

using OscarWeb.Constants;

using Common.Models;

namespace OscarWeb.Services
{
    public class UserRoleAdminPageModelService : PageModelService
    {
        public override string ModuleName { get; }

        public UserRoleAdminPageModelService()
        {
            ModuleName = ModuleNameConstants.UserRoleAdmin;
        }

        /// <summary>
        /// Returns an object containing the user-roles for display on the page
        /// </summary>
        /// <param name="user"></param>
        /// <param name="allRoles"></param>
        /// <returns></returns>
        public ExpandoObject GetRolesForDisplay(UserModel user, RoleModels allRoles)
        {
            dynamic userroles = new ExpandoObject();
            if (user?.Roles != null && user.Roles.Any() && allRoles != null)
            {
                allRoles = UserRoleAdminPageModelService.RemoveRoleModels(user.Roles, allRoles);
            }
            userroles.UserRoles = UserRoleAdminPageModelService.ConvertUserRolesToList(user);
            userroles.AllRoles = UserRoleAdminPageModelService.ConvertAllRolesToList(allRoles);
            return userroles;
        }

        /// <summary>
        /// Create a list of roles that can be used to update the user
        /// </summary>
        /// <param name="selectedRoles"></param>
        /// <param name="allRoleModels"></param>
        public List<UserRolesModel> ProcessSelectedUserRoles(StringValues selectedRoles, RoleModels allRoleModels)
        {
            if (string.IsNullOrEmpty(selectedRoles) || allRoleModels?.Roles == null || !allRoleModels.Roles.Any()) return null;

            return selectedRoles.Select(role => allRoleModels.Roles.Find(r => r.Name == role))
                .Select(selectedRole => new UserRolesModel
                {
                    RoleId = selectedRole.Id,
                    RoleName = selectedRole.Name
                })
                .ToList();
        }

        private static List<string> ConvertAllRolesToList(RoleModels allRoles)
        {
            var result = new List<string>();
            if (allRoles?.Roles == null || !allRoles.Roles.Any()) return result;
            result.AddRange(allRoles.Roles.Select(role => role.Name));
            return result;
        }

        private static List<string> ConvertUserRolesToList(UserModel user)
        {
            var result = new List<string>();
            if (user?.Roles == null || !user.Roles.Any()) return result;
            return user.Roles.Select(role => role.RoleName).ToList();
        }

        private static RoleModels RemoveRoleModels(IReadOnlyCollection<UserRolesModel> userRoles, RoleModels allRoles)
        {
            if (userRoles == null || !userRoles.Any()) return allRoles;
            foreach (var userRole in userRoles)
            {
                if (allRoles.Roles.Exists(r => r.Id == userRole.RoleId))
                {
                    allRoles.Roles.RemoveAll(u => u.Id == userRole.RoleId);
                }
            }
            return allRoles;
        }
    }
}
