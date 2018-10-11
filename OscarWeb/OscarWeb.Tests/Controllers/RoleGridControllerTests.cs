using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;
using OscarWeb.Constants;
using OscarWeb.Controllers;
using OscarWeb.Extensions;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.Controllers
{
    [TestClass]
    public class RoleGridControllerTests
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
                if (string.IsNullOrEmpty(RoleGridControllerTests._endpoint))
                {
                    RoleGridControllerTests._endpoint = RoleGridControllerTests.Rooturl;
                }
                return RoleGridControllerTests._endpoint;
            }
            set { RoleGridControllerTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            RoleGridControllerTests.Endpoint = RoleGridControllerTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {RoleGridControllerTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    RoleGridControllerTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {RoleGridControllerTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(RoleGridControllerTests.UserEmail,
                    RoleGridControllerTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(RoleGridControllerTests.UserEmail, user.Email);

            RoleGridControllerTests._user = user;
        }

        [TestMethod]
        public async Task CreateGetUpdateDeleteRolesTests()
        {
            //Arrange
            var mockSession = RoleGridControllerTests.MockHttpContext();

            //Assert - the session object has been created correctly
            Assert.AreEqual(RoleGridControllerTests.UserEmail, mockSession.Get<string>(SessionConstants.EmailClaim));
            Assert.AreEqual(RoleGridControllerTests.Rooturl, mockSession.Get<string>(SessionConstants.WebServicesUrl));
            Assert.AreEqual(RoleGridControllerTests._user.Id.ToString().Base64Encode(), mockSession.Get<string>(SessionConstants.EncodedUserId));

            var controller = new RoleGridController();

            //Assert
            Assert.IsNotNull(controller);

            controller.MockSession = mockSession;

            DataSourceRequest request = new DataSourceRequest();

            //Act - fetch the roles
            var roles = await controller.EditingPopup_Read(request);

            //Assert
            Assert.IsNotNull(roles);

            //Arrange
            string name = Guid.NewGuid().ToString();
            RoleModel role = new RoleModel
            {
                Id = 0,
                Active = true,
                Name = name,
                CompanyId = RoleGridControllerTests._user.CompanyId
            };

            //Act - create the role
            var result = await controller.EditingPopup_Create(request, role);

            //Assert
            Assert.IsNotNull(result);

            //Act - fetch a list of the roles
            var listOfRoles =
                await new RoleAdminPageModelService().GetRolesByCompanyId(RoleGridControllerTests.UserEmail, RoleGridControllerTests.Rooturl, RoleGridControllerTests._user.Id.ToString().Base64Encode(), RoleGridControllerTests._user.CompanyId);

            Assert.IsNotNull(listOfRoles);
            Assert.IsNotNull(listOfRoles.Roles);
            Assert.IsTrue(listOfRoles.Roles.Any());

            //Find the newly created role
            var newrole = listOfRoles.Roles.Find(c => c.Name == name);

            //Assert
            Assert.IsNotNull(newrole);
            Assert.AreEqual(name, newrole.Name);

            //Act - update the role
            role.Name = $"{role.Name}updated";
            role.Id = newrole.Id;
            result = await controller.EditingPopup_Update(request, role);

            //Assert
            Assert.IsNotNull(result);

            //Act - delete the role
            result = await controller.EditingPopup_Destroy(request, role);

            //Assert
            Assert.IsNotNull(result);
        }
        
        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, RoleGridControllerTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, RoleGridControllerTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, RoleGridControllerTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, RoleGridControllerTests._user);
            return httpcontext;
        }
    }
}
