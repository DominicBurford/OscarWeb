using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Models;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Pages.DocumentManager
{
    public class DocumentEditModel : PageModelBase<DocumentManagerPageModelService>
    {
        public string ErrorMessage { get; set; }

        public List<SelectListItem> DocumentCategories { get; set; }

        public string DocumentCategory { get; set; }

        /// <summary>
        /// A list of all company users
        /// </summary>
        public UserModels Users { get; set; }

        /// <summary>
        /// A list of subscribers to the document
        /// </summary>
        public UserModels Subscribers { get; set; }

        /// <summary>
        /// A list of users who are not subscribed to the document
        /// (and are therefore availableto be added as subscribers)
        /// </summary>
        public UserModels Available { get; set; }

        public string DocumentDescription { get; set; }

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

            await this.PopulateDataSources();
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

                //save
                string email = HttpContext.Session.Get<string>(SessionConstants.EmailClaim);
                string rootUrl = HttpContext.Session.Get<string>(SessionConstants.WebServicesUrl);
                string encodedId = HttpContext.Session.Get<string>(SessionConstants.EncodedUserId);
                this.CurrentDocumentId = HttpContext.Session.Get<int>(SessionConstants.CurrentDocumentId);

                var document =
                    await new DocumentManagerPageModelService().GetDocumentById(email, rootUrl, encodedId,
                        this.CurrentDocumentId);

                string description = Request.Form["document_details_description_edit"];
                string category = Request.Form["document_details_category_edit"];

                document.Active = true;
                document.Description = description;
                document.Category = Convert.ToInt32(category);
                //add any subscribers to the document
                var subscribers = this.GetDocumentSubscribers();
                document.Subscribers = subscribers;
                document.IsPrivate = !(subscribers != null && subscribers.Any());

                bool success = await new DocumentManagerPageModelService().UpdateDocument(email, rootUrl, encodedId, document);

                if (!success)
                {
                    this.ErrorMessage = StringConstants.DocumentEditError;
                    await this.PopulateDataSources();
                }
                else
                {
                    this.Users = new UserModels();
                    this.Available = new UserModels();
                    this.Subscribers = new UserModels();
                    this.DocumentCategories = new List<SelectListItem>();

                    //force the document manager page to re-load the document manager treeview
                    HttpContext.Session.Set<ToolbarModel>(SessionConstants.DocumentManagerToolbar, null);
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
            this.Users = new UserModels();
            this.Available = new UserModels();
            this.Subscribers = new UserModels();
            this.DocumentCategories = new List<SelectListItem>();
            Response.Redirect("/DocumentManager/DocumentManager");
        }

        public DocumentEditModel(IOptions<WebServicesModel> webServicesUrl) : base(webServicesUrl)
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

            var user = HttpContext.Session.Get<UserModel>(SessionConstants.CurrentUser);

            var response = await new DocumentManagerPageModelService().GetDocumentById(email, rootUrl, encodedId, this.CurrentDocumentId);

            this.DocumentDescription = response.Description;
            this.DocumentCategory = response.Category.ToString();

            this.Users = new UserModels();
            this.Available = new UserModels();
            this.Subscribers = new UserModels();
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

            this.PopulateSubscribers(response.Subscribers);
        }

        private void PopulateSubscribers(List<DocumentSubscriberModel> subscribers)
        {
            //if there are currently no subscribers then all users are available as document subscribers
            this.Available = this.Users;
            this.Subscribers = new UserModels();
            if (subscribers == null || subscribers.Count <= 0) return;

            foreach (var subscriber in subscribers)
            {
                var user = this.Users.Users.Find(u => u.Id == subscriber.UserId);
                this.Subscribers.Users.Add(user);
                this.Available.Users.Remove(user);
            }
        }

        private List<DocumentSubscriberModel> GetDocumentSubscribers()
        {
            this.Users = HttpContext.Session.Get<UserModels>(SessionConstants.CompanyUsers);
            var subscribers = Request.Form["document_details_subscribers_edit"];
            return this.Service.ToDocumentSubscribers(subscribers, this.Users);
        }

        private bool ValidatePost()
        {
            //ensure all information is correctly filled in before proceeding
            bool isValid = true;

            string description = Request.Form["document_details_description_edit"];
            
            if (string.IsNullOrEmpty(description))
            {
                this.ErrorMessage = StringConstants.DocumentNoDescription;
                isValid = false;
            }
            return isValid;
        }
    }
}