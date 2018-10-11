using Microsoft.Extensions.Options;

using OscarWeb.Services;
using OscarWeb.Models;

namespace OscarWeb.Pages.Administration
{
    public class AdministrationModel : PageModelBase<AdministrationPageModelService>
    {
        public string Message { get; set; }

        public void OnGet()
        {
            this.Message = $"Welcome to the {this.Service.ModuleName} page.";
        }

        public AdministrationModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}