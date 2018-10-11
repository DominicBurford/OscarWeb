using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;
using OscarWeb.Models;

using Common.Models;

namespace OscarWeb.Pages.DocumentManager
{
    public class DocumentManagerModel : PageModelBase<DocumentManagerPageModelService>
    {
        public string Message { get; set; }

        public string ErrorMessage { get; set; }

        public ToolbarModel Toolbar { get; set; }

        public string DeleteConfirmation = StringConstants.DeleteConfirmation;

        public List<SelectListItem> DocumentCategories { get; set; }
        public List<SelectListItem> DocumentTypes { get; set; }

        private const string ToolbarName = "Document Manager";

        private int CurrentDocumentId { get; set; }

        public async Task OnGet()
        {
            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }

            this.ErrorMessage = "";

            this.Message = $"Welcome to the {this.Service.ModuleName}.";

            if (HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar) == null)
            {
                var toolbar = await this.GetToolbar();
                this.Toolbar = toolbar;
                HttpContext.Session.Set<ToolbarModel>(SessionConstants.DocumentManagerToolbar, toolbar);
            }
            else
            {
                this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            }
        }

        /// <summary>
        /// Page handler that is invoked from an AJAX call defined in 
        /// Pages\Components\Documents\Default.cshtml. 
        ///  </summary>
        /// <param name="id">The ID of the selected document</param>
        /// <returns>The selected document as a JSON object</returns>
        public async Task<JsonResult> OnGetDocument(string id)
        {
            JsonResult result = null;
            if (string.IsNullOrEmpty(id)) return null;

            this.Message = $"Selected document node {id}";

            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }

            int docId;
            if (int.TryParse(id, out docId))
            {
                string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
                string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

                this.CurrentDocumentId = docId;
                HttpContext.Session.Set<int>(SessionConstants.CurrentDocumentId, this.CurrentDocumentId);

                var response = await new DocumentManagerPageModelService().GetDocumentById(email, rootUrl, encodedId, docId);
                
                if (docId == response.Id)
                {
                    //format the response for display on the screen
                    dynamic document = this.Service.GetDocumentForDisplay(response, HttpContext.Session.Get<int>(SessionConstants.CurrentUserId));
                    result = new JsonResult(document);
                }
                else  
                {
                    this.ErrorMessage = StringConstants.DocumentFindError;
                }
            }
            return result;
        }

        /// <summary>
        /// Page handler for downloading a document
        /// </summary>
        /// <param name="currentDocumentId"></param> 
        public async Task<FileContentResult> OnPostDownload(int currentDocumentId)
        {
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

                var response = await this.Service.GetDocumentDownload(rootUrl, encodedId, currentDocumentId);

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
            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            return result;
        }

        /// <summary>
        /// Page handler for editing a document
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public void OnPostEdit(int currentDocumentId)
        {
            this.Message = $"Selected handler is Edit for document ID {currentDocumentId}";
            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            Response.Redirect($"/DocumentManager/DocumentEdit?documentid={currentDocumentId}");
        }

        /// <summary>
        /// Page handler for deleting a document
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public async Task OnPostDelete(int currentDocumentId)
        {
            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }

            this.Message = $"Selected handler is Delete for document ID {currentDocumentId}";
            string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

            var success =
                await new DocumentManagerPageModelService().DeleteDocument(email, rootUrl, encodedId,
                    currentDocumentId);

            if (!success)
            {
                this.ErrorMessage = StringConstants.DocumentDeleteError;
            }

            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
        }

        /// <summary>
        /// Page handler for uploading a document
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public void OnPostUpload(int currentDocumentId)
        {
            this.Message = $"Selected handler is Upload for document ID {currentDocumentId}";
            Response.Redirect($"/DocumentManager/DocumentUpload?documentid={currentDocumentId}");
        }

        /// <summary>
        /// Page handler for adding a new folder
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public void OnPostCreateFolder(int currentDocumentId)
        {
            this.Message = $"Selected handler is Create Folder for document ID {currentDocumentId}";
            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            Response.Redirect($"/DocumentManager/DocumentCreateFolder?documentid={currentDocumentId}");
        }

        /// <summary>
        /// Page handler for viewing document events
        /// </summary>
        /// <param name="currentDocumentId"></param>
        public void OnPostViewEvents(int currentDocumentId)
        {
            this.Message = $"Selected handler is ViewEvents for document ID {currentDocumentId}";
            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            Response.Redirect($"/DocumentManager/DocumentViewEvents?documentid={currentDocumentId}");
        }

        /// <summary>
        ///  Page handler for drag and drop of folders/documents in Document Manager Tree View
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="destinationid"></param>
        public async Task OnGetDrop(int sourceid, int destinationid)
        {
            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);
            this.Message = $"Selected handler is drag & drop for document ID {sourceid}";
            this.Toolbar = HttpContext.Session.Get<ToolbarModel>(SessionConstants.DocumentManagerToolbar);
            await new DocumentManagerPageModelService().UpdateDocumentTreeParent(destinationid, sourceid, rootUrl, encodedId);
        }

        public DocumentManagerModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }

        private async Task<ToolbarModel> GetToolbar()
        {
            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }
            string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);
            return await this.Service.GetToolbarForUser(email, rootUrl, encodedId, DocumentManagerModel.ToolbarName);
        }
    }
}