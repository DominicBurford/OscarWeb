using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;

using OscarWeb.Constants;
using OscarWeb.Controllers;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.Controllers
{
    [TestClass]
    public class ViewUnreadDocumentsControllerTests
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
                if (string.IsNullOrEmpty(ViewUnreadDocumentsControllerTests._endpoint))
                {
                    ViewUnreadDocumentsControllerTests._endpoint = ViewUnreadDocumentsControllerTests.Rooturl;
                }
                return ViewUnreadDocumentsControllerTests._endpoint;
            }
            set { ViewUnreadDocumentsControllerTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            ViewUnreadDocumentsControllerTests.Endpoint = ViewUnreadDocumentsControllerTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {ViewUnreadDocumentsControllerTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    ViewUnreadDocumentsControllerTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {ViewUnreadDocumentsControllerTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(ViewUnreadDocumentsControllerTests.UserEmail,
                    ViewUnreadDocumentsControllerTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(ViewUnreadDocumentsControllerTests.UserEmail, user.Email);

            ViewUnreadDocumentsControllerTests._user = user;
        }

        [TestMethod]
        public async Task EditingPopup_ReadTests()
        {
            //Arrange
            var mockSession = ViewUnreadDocumentsControllerTests.MockHttpContext();

            //Assert - the session object has been created correctly
            Assert.AreEqual(ViewUnreadDocumentsControllerTests.UserEmail, mockSession.Get<string>(SessionConstants.EmailClaim));
            Assert.AreEqual(ViewUnreadDocumentsControllerTests.Rooturl, mockSession.Get<string>(SessionConstants.WebServicesUrl));
            Assert.AreEqual(ViewUnreadDocumentsControllerTests._user.Id.ToString().Base64Encode(), mockSession.Get<string>(SessionConstants.EncodedUserId));
            
            var controller = new ViewUnreadDocumentsController();

            //Assert
            Assert.IsNotNull(controller);

            controller.MockSession = mockSession;

            DataSourceRequest request = new DataSourceRequest();

            //Act - fetch the document events
            var result = await controller.EditingPopup_Read(request);

            //Assert
            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, ViewUnreadDocumentsControllerTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, ViewUnreadDocumentsControllerTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, ViewUnreadDocumentsControllerTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, ViewUnreadDocumentsControllerTests._user);
            return httpcontext;
        }
    }
}
