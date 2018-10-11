using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;

using OscarWeb.Services;
using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.ViewComponents;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.ViewComponents
{
    [TestClass]
    public class DocumentsViewComponentTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";
        private const string UserEmail = "unittest@grosvenor-leasing.co.uk";
        private static UserModel _user = null;

        private static string _endpoint;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(DocumentsViewComponentTests._endpoint))
                {
                    DocumentsViewComponentTests._endpoint = DocumentsViewComponentTests.Rooturl;
                }
                return DocumentsViewComponentTests._endpoint;
            }
            set { DocumentsViewComponentTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            DocumentsViewComponentTests.Endpoint = DocumentsViewComponentTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {DocumentsViewComponentTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    DocumentsViewComponentTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {DocumentsViewComponentTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(DocumentsViewComponentTests.UserEmail,
                    DocumentsViewComponentTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentsViewComponentTests.UserEmail, user.Email);

            DocumentsViewComponentTests._user = user;
        }

        [TestMethod]
        public async Task InvokeAsyncTest()
        {
            DocumentsViewComponent component = new DocumentsViewComponent();
            var mockContext = DocumentsViewComponentTests.MockHttpContext();

            string email = mockContext.Get<string>(SessionConstants.EmailClaim);
            string url = mockContext.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = mockContext.Get<string>(SessionConstants.EncodedUserId);
            UserModel user = mockContext.Get<UserModel>(SessionConstants.CurrentUser);

            Assert.AreEqual(DocumentsViewComponentTests.UserEmail, email);
            Assert.AreEqual(DocumentsViewComponentTests.Endpoint, url);
            Assert.IsNotNull(encodedId);
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentsViewComponentTests.UserEmail, user.Email);

            var result = await component.InvokeAsync(new List<TreeViewItemModel>(), mockContext);

            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, DocumentsViewComponentTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, DocumentsViewComponentTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, DocumentsViewComponentTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, DocumentsViewComponentTests._user);
            return httpcontext;
        }
    }
}
