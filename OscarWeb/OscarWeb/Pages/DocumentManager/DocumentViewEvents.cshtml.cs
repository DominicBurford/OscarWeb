using System;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages.DocumentManager
{
    public class DocumentViewEventsModel : PageModelBase<DocumentManagerPageModelService>
    {
        private int CurrentDocumentId { get; set; }

        public void OnGet()
        {
            //retrieve the document ID passed on the querystring
            var documentStringValues = Request.Query["documentid"];
            if (!string.IsNullOrEmpty(documentStringValues) && documentStringValues.Count > 0)
            {
                this.CurrentDocumentId = Convert.ToInt32(documentStringValues[0]);
                HttpContext.Session.Set<int>(SessionConstants.CurrentDocumentId, this.CurrentDocumentId);
            }
        }

        public DocumentViewEventsModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}