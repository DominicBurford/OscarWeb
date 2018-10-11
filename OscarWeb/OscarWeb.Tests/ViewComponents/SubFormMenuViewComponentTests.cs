using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Common.Models;
using Microsoft.AspNetCore.Http;
using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;
using OscarWeb.ViewComponents;

namespace OscarWeb.Tests.ViewComponents
{
    [TestClass]
    public class SubFormMenuViewComponentTests
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
                if (string.IsNullOrEmpty(SubFormMenuViewComponentTests._endpoint))
                {
                    SubFormMenuViewComponentTests._endpoint = SubFormMenuViewComponentTests.Rooturl;
                }
                return SubFormMenuViewComponentTests._endpoint;
            }
            set { SubFormMenuViewComponentTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            SubFormMenuViewComponentTests.Endpoint = SubFormMenuViewComponentTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {SubFormMenuViewComponentTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    SubFormMenuViewComponentTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {SubFormMenuViewComponentTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(SubFormMenuViewComponentTests.UserEmail,
                    SubFormMenuViewComponentTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(SubFormMenuViewComponentTests.UserEmail, user.Email);

            SubFormMenuViewComponentTests._user = user;
        }

        [TestMethod]
        public async Task InvokeAsyncTest()
        {
            SubFormMenuViewComponent component = new SubFormMenuViewComponent();
            var mockContext = SubFormMenuViewComponentTests.MockHttpContext();
            const int parentId = 17;

            string email = mockContext.Get<string>(SessionConstants.EmailClaim);
            string url = mockContext.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = mockContext.Get<string>(SessionConstants.EncodedUserId);
            MainMenuModels menuitems = mockContext.Get<MainMenuModels>(SessionConstants.CompanyAdministrationMenuItems);
            UserModel user = mockContext.Get<UserModel>(SessionConstants.CurrentUser);

            Assert.AreEqual(SubFormMenuViewComponentTests.UserEmail, email);
            Assert.AreEqual(SubFormMenuViewComponentTests.Endpoint, url);
            Assert.IsNotNull(encodedId);
            Assert.IsNull(menuitems);
            Assert.IsNotNull(user);
            Assert.AreEqual(SubFormMenuViewComponentTests.UserEmail, user.Email);

            var result = await component.InvokeAsync(parentId, SessionConstants.CompanyAdministrationMenuItems, new MainMenuModels(), mockContext);

            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, SubFormMenuViewComponentTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, SubFormMenuViewComponentTests.Endpoint);
            httpcontext.Set<MainMenuModels>(SessionConstants.CompanyAdministrationMenuItems, null);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, SubFormMenuViewComponentTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, SubFormMenuViewComponentTests._user);
            return httpcontext;
        }
    }
}
