using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;
using OscarWeb.ViewComponents;

namespace OscarWeb.Tests.ViewComponents
{
    [TestClass]
    public class MenuItemsViewComponentTests
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
                if (string.IsNullOrEmpty(MenuItemsViewComponentTests._endpoint))
                {
                    MenuItemsViewComponentTests._endpoint = MenuItemsViewComponentTests.Rooturl;
                }
                return MenuItemsViewComponentTests._endpoint;
            }
            set { MenuItemsViewComponentTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            MenuItemsViewComponentTests.Endpoint = MenuItemsViewComponentTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {MenuItemsViewComponentTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    MenuItemsViewComponentTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {MenuItemsViewComponentTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(MenuItemsViewComponentTests.UserEmail,
                    MenuItemsViewComponentTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(MenuItemsViewComponentTests.UserEmail, user.Email);

            MenuItemsViewComponentTests._user = user;
        }
        
        [TestMethod]
        public async Task InvokeAsyncTest()
        {
            MenuItemsViewComponent component = new MenuItemsViewComponent();
            var mockContext = MenuItemsViewComponentTests.MockHttpContext();

            string email = mockContext.Get<string>(SessionConstants.EmailClaim);
            string url = mockContext.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = mockContext.Get<string>(SessionConstants.EncodedUserId);
            MainMenuModels menuitems = mockContext.Get<MainMenuModels>(SessionConstants.TopLevelMenuItems);
            UserModel user = mockContext.Get<UserModel>(SessionConstants.CurrentUser);

            Assert.AreEqual(MenuItemsViewComponentTests.UserEmail, email);
            Assert.AreEqual(MenuItemsViewComponentTests.Endpoint, url);
            Assert.IsNotNull(encodedId);
            Assert.IsNull(menuitems);
            Assert.IsNotNull(user);
            Assert.AreEqual(MenuItemsViewComponentTests.UserEmail, user.Email);
            
            var result = await component.InvokeAsync(0, mockContext);

            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, MenuItemsViewComponentTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, MenuItemsViewComponentTests.Endpoint);
            httpcontext.Set<MainMenuModels>(SessionConstants.TopLevelMenuItems, null);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, MenuItemsViewComponentTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, MenuItemsViewComponentTests._user);
            return httpcontext;
        }
    }
}
