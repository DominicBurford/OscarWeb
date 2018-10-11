using Microsoft.Extensions.Options;

using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages.Administration.DocumentEvents
{
    public class DocumentEventsModel : PageModelBase<DocumentManagerPageModelService>
    {
        public void OnGet()
        {
        }

        public DocumentEventsModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}