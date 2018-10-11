using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Pages.DocumentManager
{
    public class DocumentCreateFolderModel : PageModelBase<DocumentManagerPageModelService>
    {
        public ToolbarModel Toolbar { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }

        public List<SelectListItem> DocumentCategories { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; }

        public string DocumentFolderName { get; set; }

        public string DocumentFolderDescription { get; set; }

        private int CurrentDocumentId { get; set; }

        public async Task OnGet()
        {
            //ensure we have session state before proceeding
            if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
            {
                await base.SetSession();
            }

            this.ErrorMessage = "";

            //retrieve the document ID passed on the querystring
            var documentStringValues = Request.Query["documentid"];
            if (!string.IsNullOrEmpty(documentStringValues) && documentStringValues.Count > 0)
            {
                this.CurrentDocumentId = Convert.ToInt32(documentStringValues[0]);
                HttpContext.Session.Set<int>(SessionConstants.CurrentDocumentId, this.CurrentDocumentId);
            }
        }

        public async Task OnPost()
        {
            if (Request.Form.Keys.Contains("submitCancel"))
            {
                OnPostCancel();
            }
            else if (Request.Form.Keys.Contains("submitSave"))
            {
                await OnPostSave();
            }
        }

        public async Task OnPostSave()
        {
            this.ErrorMessage = "";

            bool isValid = this.ValidatePost();

            if (isValid)
            {
                //ensure we have session state before proceeding
                if (HttpContext.Session.Get<int>(SessionConstants.CurrentUserId) == 0)
                {
                    await base.SetSession();
                }

                await this.PopulateDataSources();

                this.CurrentDocumentId = HttpContext.Session.Get<int>(SessionConstants.CurrentDocumentId);

                string name = Request.Form["document_details_name_createfolder"];
                string description = Request.Form["document_details_description_createfolder"];

                var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);

                var documentType = this.DocumentTypes.Find(t => t.Text.Contains("Folder"));
                var documentCategory = this.DocumentCategories.Find(c => c.Text == "Folder");

                DocumentsModel folder = new DocumentsModel
                {
                    Name = name,
                    ParentId = this.CurrentDocumentId,
                    Category = Convert.ToInt32(documentCategory.Value),
                    CompanyId = user.CompanyId,
                    IsDocument = false,
                    IsPrivate = false,
                    Active = true,
                    DocumentType = Convert.ToInt32(documentType.Value),
                    UploadedBy = user.Id,
                    Description = description
                };

                string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
                string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

                bool success = await new DocumentManagerPageModelService().UpdateDocument(email, rootUrl, encodedId, folder);

                if (!success)
                {
                    this.ErrorMessage = StringConstants.DocumentFolderCreateError;
                }
                else
                {
                    //force the document manager page to re-load the document manager treeview
                    HttpContext.Session.Set<ToolbarModel>(SessionConstants.DocumentManagerToolbar, null);
                    Response.Redirect("/DocumentManager/DocumentManager");
                }
            }
        }

        public void OnPostCancel()
        {
            //cancel
            Response.Redirect("/DocumentManager/DocumentManager");
        }

        public DocumentCreateFolderModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }

        private async Task PopulateDataSources()
        {
            string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);
            this.CurrentDocumentId = HttpContext.Session.Get<int>(SessionConstants.CurrentDocumentId);

            var documentCategories = await this.Service.GetDocumentCategories(email, rootUrl, encodedId);
            this.DocumentCategories = documentCategories.ToSelectListItems();

            var documentTypes = await this.Service.GetDocumentTypes(email, rootUrl, encodedId);
            this.DocumentTypes = documentTypes.ToSelectListItems();
        }

        private bool ValidatePost()
        {
            //ensure all information is correctly filled in before proceeding
            bool isValid = true;

            string name = Request.Form["document_details_name_createfolder"];
            string description = Request.Form["document_details_description_createfolder"];

            this.DocumentFolderName = name;
            this.DocumentFolderDescription = description;

            if (string.IsNullOrEmpty(name))
            {
                this.ErrorMessage = StringConstants.DocumentFolderNoName;
                isValid = false;
            }
            if (isValid && string.IsNullOrEmpty(description))
            {
                this.ErrorMessage = StringConstants.DocumentFolderNoDescription;
                isValid = false;
            }
            return isValid;
        }
    }
}