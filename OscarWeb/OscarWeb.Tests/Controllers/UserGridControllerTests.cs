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
    public class UserGridControllerTests
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
                if (string.IsNullOrEmpty(UserGridControllerTests._endpoint))
                {
                    UserGridControllerTests._endpoint = UserGridControllerTests.Rooturl;
                }
                return UserGridControllerTests._endpoint;
            }
            set { UserGridControllerTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            UserGridControllerTests.Endpoint = UserGridControllerTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {UserGridControllerTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    UserGridControllerTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {UserGridControllerTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(UserGridControllerTests.UserEmail,
                    UserGridControllerTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(UserGridControllerTests.UserEmail, user.Email);

            UserGridControllerTests._user = user;
        }

        [TestMethod]
        public async Task CreateGetUpdateDeleteUserTests()
        {
            //Arrange
            var mockSession = UserGridControllerTests.MockHttpContext();
            
            //Assert - the session object has been created correctly
            Assert.AreEqual(UserGridControllerTests.UserEmail, mockSession.Get<string>(SessionConstants.EmailClaim));
            Assert.AreEqual(UserGridControllerTests.Rooturl, mockSession.Get<string>(SessionConstants.WebServicesUrl));
            Assert.AreEqual(UserGridControllerTests._user.Id.ToString().Base64Encode(), mockSession.Get<string>(SessionConstants.EncodedUserId));
            
            var controller = new UserGridController();

            //Assert
            Assert.IsNotNull(controller);

            controller.MockSession = mockSession;

            //Arrange
            string name = Guid.NewGuid().ToString();

            UserModel model = new UserModel
            {
                Email = $"{name}@grosvenor-leasing.co.uk",
                UserName = name,
                Active = true,
                CompanyId = 1,
                SuperUser = false
            };

            DataSourceRequest request = new DataSourceRequest();

            //Act - fetch the users
            var users = await controller.EditingPopup_Read(request);

            //Assert
            Assert.IsNotNull(users);
            
            //Act - create the user
            var result = await controller.EditingPopup_Create(request, model);

            //Assert
            Assert.IsNotNull(result);

            model.UserName = $"UPDATED_{model.UserName}";

            //Act - Update the user
            result = await controller.EditingPopup_Update(request, model);

            //Assert
            Assert.IsNotNull(result);

            //Act - Fetch the user
            result = await controller.EditingPopup_Update(request, model);

            //Assert
            Assert.IsNotNull(result);

            //Act - Fetch the user
            result = await controller.EditingPopup_Destroy(request, model);

            //Assert
            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, UserGridControllerTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, UserGridControllerTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, UserGridControllerTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, UserGridControllerTests._user);
            return httpcontext;
        }
    }
}
