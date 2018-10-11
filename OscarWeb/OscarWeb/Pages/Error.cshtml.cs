using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OscarWeb.Pages
{
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }
        public IExceptionHandlerPathFeature ExceptionFeature { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet()
        {
            this.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            this.ExceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        }
    }
}
