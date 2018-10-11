using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Pages.DocumentManager
{
    public class DocumentUploadModel : PageModelBase<DocumentManagerPageModelService>
    {
        public string ErrorMessage { get; set; }

        public List<SelectListItem> DocumentTypes { get; set; }

        public List<SelectListItem> DocumentCategories { get; set; }

        public UserModels Users { get; set; }

        private int CurrentDocumentId { get; set; }

        public async Task OnGet()
        {
            this.ErrorMessage = "";

            //retrieve the document ID passed on the querystring
            var documentStringValues = Request.Query["documentid"];
            if (!string.IsNullOrEmpty(documentStringValues) && documentStringValues.Count > 0)
            {
                this.CurrentDocumentId = Convert.ToInt32(documentStringValues[0]);
                HttpContext.Session.Set<int>(SessionConstants.CurrentDocumentId, this.CurrentDocumentId);
            }

            await this.PopulateDataSources();
        }

        public async Task OnPost(IEnumerable<IFormFile> files)
        {

            if (files.Any())
            {
                await OnPostUpload(files);
            }
            else
            {
                OnPostCancel();
            }

        }

        public async Task OnPostUpload(IEnumerable<IFormFile> files)
        {
            this.ErrorMessage = "";

            //ensure all information is correctly filled in before proceeding
            bool isValid = this.ValidateUpload(files);
            
            //retrieve the document ID passed on the querystring - retrieved in the OnGet()
            this.CurrentDocumentId = HttpContext.Session.Get<int>(SessionConstants.CurrentDocumentId);

            if (isValid)
            {
                string description = Request.Form["document_details_description_upload"];
                string type = Request.Form["document_details_type_upload"];
                string category = Request.Form["document_details_category_upload"];
                var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);

                var fileToUpload = files.First();
                string filename = fileToUpload.FileName;
                long fileLength = fileToUpload.Length;
                byte[] buffer = new byte[fileLength];

                //read the image into a byte array
                using (var stream = fileToUpload.OpenReadStream())
                {
                    stream.Read(buffer, 0, (int)fileLength);
                }

                var currentUser = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);

                DocumentImageModel document = new DocumentImageModel
                {
                    UploadedBy = HttpContext.Session.Get<int>(SessionConstants.CurrentUserId)
                };
                document.Document = new DocumentsModel
                {
                    Description = description,
                    CompanyId = currentUser.CompanyId,
                    CompanyName = currentUser.CompanyName,
                    Category = Convert.ToInt32(category),
                    Name = filename,
                    Active = true,
                    IsDocument = true,
                    IsPrivate = true,
                    LastViewed = DateTime.MinValue,
                    UploadedBy = document.UploadedBy,
                    DocumentType = Convert.ToInt32(type),
                    ParentId = this.CurrentDocumentId,
                    Subscribers = null,
                    UploadedByUsername = user.UserName
                };

                var documentType =
                    new DocumentManagerPageModelService().GetDocumentTypeForDocument(Convert.ToInt32(type), HttpContext.Session.Get<DocumentTypeModels>(SessionConstants.DocumentTypes));

                //possibly do a check to see if the DocumentModel.MimeType and the FileToUpload.ContentType are the same
                //fileToUpload.ContentType == documentType.MimeType

                document.Image = new ImageStreamModel
                {
                    MimeType = documentType.MimeType,
                    ImagesList = new Dictionary<string, byte[]>
                    {
                        { filename, buffer }
                    }
                };

                //add any subscribers to the document
                var subscribers = this.GetDocumentSubscribers();
                if (subscribers != null && subscribers.Any())
                {
                    document.Document.Subscribers = subscribers;
                    document.Document.IsPrivate = false;
                }

                string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
                string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

                var success = await new DocumentManagerPageModelService().UploadDocumentImage(email, rootUrl, encodedId, document);

                if (!success)
                {
                    this.ErrorMessage = StringConstants.DocumentUploadError;
                    await this.PopulateDataSources();
                }
                else
                {
                    var users = HttpContext.Session.Get<UserModels>(SessionConstants.CompanyUsers);
                    document.Document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(document.Document, users);
                    await new DocumentManagerPageModelService().NotifySubscribers(document.Document, rootUrl);

                    this.DocumentCategories = new List<SelectListItem>();
                    this.DocumentTypes = new List<SelectListItem>();

                    //force the document manager page to re-load the document manager treeview
                    HttpContext.Session.Set<ToolbarModel>(SessionConstants.DocumentManagerToolbar, null);

                    //navigate back to the document manager page
                    Response.Redirect("/DocumentManager/DocumentManager");
                }
            }
            else
            {
                await this.PopulateDataSources();
            }
        }

        public void OnPostCancel()
        {
            //cancel
            this.DocumentCategories = new List<SelectListItem>();
            this.DocumentTypes = new List<SelectListItem>();
            this.Users = new UserModels();
            Response.Redirect("/DocumentManager/DocumentManager");
        }

        public DocumentUploadModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
        {
        }

        private async Task PopulateDataSources()
        {
            string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
            string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);

            var documentTypes = await this.Service.GetDocumentTypes(email, rootUrl, encodedId);
            var documentCategories = await this.Service.GetDocumentCategories(email, rootUrl, encodedId);

            HttpContext.Session.Set<DocumentTypeModels>(SessionConstants.DocumentTypes, documentTypes);
            this.DocumentTypes = documentTypes.ToSelectListItems();
            this.DocumentCategories = documentCategories.ToSelectListItems();

            var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);

            this.Users = new UserModels();
            if (HttpContext.Session.Get<UserModels>(SessionConstants.CompanyUsers) == null)
            {
                var users = await new UserAdminPageModelService().GetUsersByCompanyId(email, rootUrl, encodedId, user.CompanyId);
                users.Users.RemoveAll(u => u.Email == email);
                if (users?.Users != null && users.Users.Any())
                {
                    HttpContext.Session.Set<UserModels>(SessionConstants.CompanyUsers, users);
                    this.Users = users;
                }
            }
            else
            {
                this.Users = HttpContext.Session.Get<UserModels>(SessionConstants.CompanyUsers);
            }
        }

        private bool ValidateUpload(IEnumerable<IFormFile> files)
        {
            //ensure all information is correctly filled in before proceeding
            bool isValid = true;

            if (files == null || !files.Any())
            {
                this.ErrorMessage = StringConstants.DocumentNoFileSelectedError;
                isValid = false;
            }

            if (isValid && files.Count() != 1)
            {
                this.ErrorMessage = StringConstants.DocumentFileSelectedError;
                isValid = false;
            }

            string description = Request.Form["document_details_description_upload"];

            if (isValid && string.IsNullOrEmpty(description))
            {
                this.ErrorMessage = StringConstants.DocumentNoDescription;
                isValid = false;
            }

            string type = Request.Form["document_details_type_upload"];

            if (isValid && string.IsNullOrEmpty(type))
            {
                this.ErrorMessage = StringConstants.DocumentNoTypeSelected;
                isValid = false;
            }
            string category = Request.Form["document_details_category_upload"];

            if (isValid && string.IsNullOrEmpty(category))
            {
                this.ErrorMessage = StringConstants.DocumentNoCategorySelected;
                isValid = false;
            }

            string fileExtension = "";
            DocumentTypeModels documentTypes = null;

            if (isValid)
            {
                var selectedfile = files.First();
                FileInfo fi = new FileInfo(selectedfile.FileName);
                //remove the period from the file extension
                fileExtension = fi.Extension.Substring(1);
                documentTypes = HttpContext.Session.Get<DocumentTypeModels>(SessionConstants.DocumentTypes);
            }

            //determine if the selected document for upload has a supported file extension
            if (isValid)
            {
                bool validFileType =
                    new DocumentManagerPageModelService().IsFileTypeSupported(fileExtension, documentTypes);

                if (!validFileType)
                {
                    this.ErrorMessage = StringConstants.DocumentUnsupportedFileType;
                    isValid = false;
                }
            }

            //determine if the selected document type matches the document's file extension
            if (isValid)
            {
                var documentType =
                    new DocumentManagerPageModelService().GetDocumentTypeForDocument(Convert.ToInt32(type), documentTypes);

                if (string.Compare(fileExtension, documentType.Extension, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    this.ErrorMessage = StringConstants.DocumentMismatchedFileType;
                    isValid = false;
                }
            }
            return isValid;
        }

        private List<DocumentSubscriberModel> GetDocumentSubscribers()
        {
            this.Users = HttpContext.Session.Get<UserModels>(SessionConstants.CompanyUsers);
            var subscribers = Request.Form["document_details_subscribers"];
            return this.Service.ToDocumentSubscribers(subscribers, this.Users);
        }
    }
}