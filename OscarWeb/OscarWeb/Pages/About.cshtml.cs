using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;

namespace OscarWeb.Pages
{
    public class AboutModel : PageModel
    {
        private IOptions<WebServicesModel> WebServicesUrl { get; set; }

        public string Message { get; set; }
        public string UserEmail { get; set; }
        public string WebServicesMessage { get; set; }

        public AboutModel(IOptions<WebServicesModel> webServicesUrl)
        {
            this.WebServicesUrl = webServicesUrl;
        }

        public void OnGet()
        {
            this.Message = "Your application description page.";
            this.UserEmail = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            this.WebServicesMessage = $"You are consuming services from {this.WebServicesUrl.Value.Endpoint}";
        }
    }
}
