using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;
using OscarWeb.ViewComponents;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.ViewComponents
{
    [TestClass]
    public class KendoMenuViewComponentTests
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
                if (string.IsNullOrEmpty(KendoMenuViewComponentTests._endpoint))
                {
                    KendoMenuViewComponentTests._endpoint = KendoMenuViewComponentTests.Rooturl;
                }
                return KendoMenuViewComponentTests._endpoint;
            }
            set { KendoMenuViewComponentTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            KendoMenuViewComponentTests.Endpoint = KendoMenuViewComponentTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {KendoMenuViewComponentTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    KendoMenuViewComponentTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {KendoMenuViewComponentTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(KendoMenuViewComponentTests.UserEmail,
                    KendoMenuViewComponentTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(KendoMenuViewComponentTests.UserEmail, user.Email);

            KendoMenuViewComponentTests._user = user;
        }

        [TestMethod]
        public async Task InvokeAsyncTest()
        {
            KendoMenuViewComponent component = new KendoMenuViewComponent();
            var mockContext = KendoMenuViewComponentTests.MockHttpContext();

            string email = mockContext.Get<string>(SessionConstants.EmailClaim);
            string url = mockContext.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = mockContext.Get<string>(SessionConstants.EncodedUserId);
            var menuitems = mockContext.Get<List<MenuItem>>(SessionConstants.KendoMainMenu);
            UserModel user = mockContext.Get<UserModel>(SessionConstants.CurrentUser);

            Assert.AreEqual(KendoMenuViewComponentTests.UserEmail, email);
            Assert.AreEqual(KendoMenuViewComponentTests.Endpoint, url);
            Assert.IsNotNull(encodedId);
            Assert.IsNull(menuitems);
            Assert.IsNotNull(user);
            Assert.AreEqual(KendoMenuViewComponentTests.UserEmail, user.Email);

            var result = await component.InvokeAsync(new List<MenuItem>(), mockContext);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task InvokeAsyncNoItemsTest()
        {
            KendoMenuViewComponent component = new KendoMenuViewComponent();
            var mockContext = KendoMenuViewComponentTests.MockHttpContext();

            //if the user is null then a "No items to display" menu will be returned
            string email = null;
            string url = mockContext.Get<string>(SessionConstants.WebServicesUrl);
            string encodedId = mockContext.Get<string>(SessionConstants.EncodedUserId);
            var menuitems = mockContext.Get<List<MenuItem>>(SessionConstants.KendoMainMenu);
            UserModel user = mockContext.Get<UserModel>(SessionConstants.CurrentUser);

            Assert.IsNull(email);
            Assert.AreEqual(KendoMenuViewComponentTests.Endpoint, url);
            Assert.IsNotNull(encodedId);
            Assert.IsNull(menuitems);
            Assert.IsNotNull(user);
            Assert.AreEqual(KendoMenuViewComponentTests.UserEmail, user.Email);

            var result = await component.InvokeAsync(new List<MenuItem>(), mockContext);

            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, KendoMenuViewComponentTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, KendoMenuViewComponentTests.Endpoint);
            httpcontext.Set<List<MenuItem>>(SessionConstants.KendoMainMenu, null);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, KendoMenuViewComponentTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, KendoMenuViewComponentTests._user);
            return httpcontext;
        }
    }
}
