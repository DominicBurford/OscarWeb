using Microsoft.Extensions.Options;

using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages.Administration.Role
{
    public class RoleAdminModel : PageModelBase<RoleAdminPageModelService>
    {
        public string Message { get; set; }

        public void OnGet()
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";
        }

        public RoleAdminModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}