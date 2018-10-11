using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages.Administration.Company
{
    public class CompanyAddressAdminModel : PageModelBase<CompanyAdminPageModelService>
    {
        public string Message { get; set; }

        public void OnGet(int companyId)
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";
            HttpContext.Session.Set<int>(SessionConstants.CompanyAddressId, companyId);
        }

        public CompanyAddressAdminModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}