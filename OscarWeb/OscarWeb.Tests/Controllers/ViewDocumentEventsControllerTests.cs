using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Constants;
using OscarWeb.Controllers;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.Controllers
{
    [TestClass]
    public class ViewDocumentEventsControllerTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";
        private const string UserEmail = "unittest@grosvenor-leasing.co.uk";
        private static UserModel _user = null;
        private static string _endpoint;
        private const int DocumentId = 4;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(ViewDocumentEventsControllerTests._endpoint))
                {
                    ViewDocumentEventsControllerTests._endpoint = ViewDocumentEventsControllerTests.Rooturl;
                }
                return ViewDocumentEventsControllerTests._endpoint;
            }
            set { ViewDocumentEventsControllerTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            ViewDocumentEventsControllerTests.Endpoint = ViewDocumentEventsControllerTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {ViewDocumentEventsControllerTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    ViewDocumentEventsControllerTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {ViewDocumentEventsControllerTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(ViewDocumentEventsControllerTests.UserEmail,
                    ViewDocumentEventsControllerTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(ViewDocumentEventsControllerTests.UserEmail, user.Email);

            ViewDocumentEventsControllerTests._user = user;
        }

        [TestMethod]
        public async Task EditingPopup_ReadTests()
        {
            //Arrange
            var mockSession = ViewDocumentEventsControllerTests.MockHttpContext();

            //Assert - the session object has been created correctly
            Assert.AreEqual(ViewDocumentEventsControllerTests.UserEmail, mockSession.Get<string>(SessionConstants.EmailClaim));
            Assert.AreEqual(ViewDocumentEventsControllerTests.Rooturl, mockSession.Get<string>(SessionConstants.WebServicesUrl));
            Assert.AreEqual(ViewDocumentEventsControllerTests._user.Id.ToString().Base64Encode(), mockSession.Get<string>(SessionConstants.EncodedUserId));
            Assert.AreEqual(ViewDocumentEventsControllerTests.DocumentId, mockSession.Get<int>(SessionConstants.CurrentDocumentId));

            var controller = new ViewDocumentEventsController();

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
            httpcontext.Set<string>(SessionConstants.EmailClaim, ViewDocumentEventsControllerTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, ViewDocumentEventsControllerTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, ViewDocumentEventsControllerTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, ViewDocumentEventsControllerTests._user);
            httpcontext.Set<int>(SessionConstants.CurrentDocumentId, ViewDocumentEventsControllerTests.DocumentId);
            return httpcontext;
        }
    }
}
