using Microsoft.Extensions.Options;

using OscarWeb.Services;
using OscarWeb.Models;

namespace OscarWeb.Pages.Administration.Company
{
    public class CompanyAdminModel : PageModelBase<CompanyAdminPageModelService>
    {
        public string Message { get; set; }

        public void OnGet()
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";
        }

        public CompanyAdminModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}