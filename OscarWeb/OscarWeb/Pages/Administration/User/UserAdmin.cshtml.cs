using Microsoft.Extensions.Options;

using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages.Administration.User
{
    public class UserAdminModel : PageModelBase<UserAdminPageModelService>
    {
        public string Message { get; set; }

        public void OnGet()
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";
        }

        public UserAdminModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}