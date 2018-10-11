using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

namespace OscarWeb.Pages
{
    public class IndexModel : PageModelBase<IndexPageModelService>
    {
        public string Message { get; set; }

        public string ErrorMessage { get; set; }

        public async Task OnGet()
        {
            this.ErrorMessage = "";
            await base.SetSession();
        }

        /// <summary>
        /// The page handler for the KeepSessionalive() heartbeat.
        /// The heartbeat is initially invoked in javascript in _Layout.cshtml.
        /// This then invokes the javascript function KeepSessionalive() defined in site.js.
        /// This is an AJAX call to this page handler.
        /// </summary>
        public void OnGetSession()
        {
            HttpContext.Session.Set<DateTime>(SessionConstants.KeepSessionAlive, DateTime.Now);
        }

        /// <summary>
        /// Redirect the user to the DocumentManager page for the selected document.
        /// Invoked from the Details button on the Index.cshtml button. 
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public void OnPostDetails(int currentDocumentId)
        {
            this.ErrorMessage = "";
            if (currentDocumentId > 0)
            {
                Response.Redirect($"/DocumentManager/DocumentManager?documentid={currentDocumentId}");
            }
        }

        /// <summary>
        /// Download the specified document
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public async Task<FileContentResult> OnPostDownload(int currentDocumentId)
        {
            this.ErrorMessage = "";
            this.Message = $"Selected handler is Download for document ID {currentDocumentId}";
            FileContentResult result = null;
            if (currentDocumentId > 0)
            {
                //ensure we have session state before proceeding
                if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
                {
                    await base.SetSession();
                }

                string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

                var response = await new DocumentManagerPageModelService().GetDocumentDownload(rootUrl, encodedId, currentDocumentId);

                if (response?.ImageBytes != null && response.BlobInformation != null && response.ImageBytes.Length > 0)
                {
                    //download the document to the client
                    string filename = response.BlobInformation.BlobName;
                    string mimetype = response.BlobInformation.BlobContentType;
                    var image = response.ImageBytes;
                    result = File(image, mimetype, filename);
                }
                else
                {
                    this.ErrorMessage = StringConstants.DocumentDownloadError;
                }
            }
            return result;
        }

        public IndexModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }
    }
}
