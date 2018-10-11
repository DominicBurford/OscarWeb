using Microsoft.AspNetCore.Mvc.RazorPages;

using OscarWeb.Constants;
using OscarWeb.Extensions;

namespace OscarWeb.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }
        public string UserEmail { get; set; }

        public void OnGet()
        {
            this.Message = "Your contact page.";
            this.UserEmail = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
        }
    }
}
