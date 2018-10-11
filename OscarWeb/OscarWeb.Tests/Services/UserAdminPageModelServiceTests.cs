using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class UserAdminPageModelServiceTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";
        private const string UserEmail = "unittest@grosvenor-leasing.co.uk";

        private static string _endpoint;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(UserAdminPageModelServiceTests._endpoint))
                {
                    UserAdminPageModelServiceTests._endpoint = UserAdminPageModelServiceTests.Rooturl;
                }
                return UserAdminPageModelServiceTests._endpoint;
            }
            set { UserAdminPageModelServiceTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            UserAdminPageModelServiceTests.Endpoint = UserAdminPageModelServiceTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {UserAdminPageModelServiceTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    UserAdminPageModelServiceTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {UserAdminPageModelServiceTests.Endpoint}");
        }

        [TestMethod]
        public async Task GetUsersByCompanyIdTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(UserAdminPageModelServiceTests.UserEmail, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            int companyId = user.CompanyId;

            //Act
            var response =
                await new UserAdminPageModelService().GetUsersByCompanyId(user.Email, UserAdminPageModelServiceTests.Rooturl, encodedId, companyId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Users);
            Assert.IsTrue(response.Users.Any());
        }

        [TestMethod]
        public async Task GetUsersByCompanyIdInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(UserAdminPageModelServiceTests.UserEmail, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            int companyId = user.CompanyId;

            //Act
            var response =
                await new UserAdminPageModelService().GetUsersByCompanyId(null, UserAdminPageModelServiceTests.Rooturl, encodedId, companyId);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new UserAdminPageModelService().GetUsersByCompanyId("", UserAdminPageModelServiceTests.Rooturl, encodedId, companyId);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new UserAdminPageModelService().GetUsersByCompanyId(user.Email, UserAdminPageModelServiceTests.Rooturl, null, companyId);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new UserAdminPageModelService().GetUsersByCompanyId(user.Email, UserAdminPageModelServiceTests.Rooturl, "", companyId);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new UserAdminPageModelService().GetUsersByCompanyId(user.Email, UserAdminPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new UserAdminPageModelService().GetUsersByCompanyId(user.Email, UserAdminPageModelServiceTests.Rooturl, encodedId, -1);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task CreateGetUpdateDeleteUserTests()
        {
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

            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(UserAdminPageModelServiceTests.UserEmail, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - Create the user
            bool result = await new UserAdminPageModelService().CreateUser(model, encodedId, UserAdminPageModelServiceTests.Rooturl);

            Assert.IsTrue(result);

            model.UserName = $"UPDATED_{model.UserName}";

            //Act - Update the user
            result = await new UserAdminPageModelService().UpdateUserDetails(model, encodedId, UserAdminPageModelServiceTests.Rooturl);

            Assert.IsTrue(result);

            //Act - fetch the user
            var saveduser = await new UserServices().GetUserDetails(model.Email, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(saveduser);
            Assert.AreEqual(model.Email, saveduser.Email);

            //Act - Delete the user
            result = await new UserAdminPageModelService().DeleteUser(model.Email, encodedId, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateUserInvalidTests()
        {
            //Act
            bool result = await new UserAdminPageModelService().CreateUser(null, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            //Act
            UserModel model = new UserModel();
            result = await new UserAdminPageModelService().CreateUser(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.UserName = "username";
            result = await new UserAdminPageModelService().CreateUser(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.Email = "useremail";
            model.UserName = "";
            result = await new UserAdminPageModelService().CreateUser(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.UserName = "username";
            result = await new UserAdminPageModelService().CreateUser(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            result = await new UserAdminPageModelService().CreateUser(model, null, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            result = await new UserAdminPageModelService().CreateUser(model, "", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateUserDetailsTests()
        {
            //Act
            bool result = await new UserAdminPageModelService().CreateUser(null, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            //Act
            UserModel model = new UserModel();
            result = await new UserAdminPageModelService().UpdateUserDetails(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.UserName = "username";
            result = await new UserAdminPageModelService().UpdateUserDetails(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.Email = "useremail";
            model.UserName = "";
            result = await new UserAdminPageModelService().UpdateUserDetails(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            model.UserName = "username";
            result = await new UserAdminPageModelService().UpdateUserDetails(model, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            result = await new UserAdminPageModelService().UpdateUserDetails(model, null, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            result = await new UserAdminPageModelService().UpdateUserDetails(model, "", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteUserTests()
        {
            //Act
            bool result =
                await new UserAdminPageModelService().DeleteUser("", "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            //Act
            result =
                await new UserAdminPageModelService().DeleteUser(null, "encodedId", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);
            
            //Act
            result =
                await new UserAdminPageModelService().DeleteUser("email", "", UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);

            //Act
            result =
                await new UserAdminPageModelService().DeleteUser("email", null, UserAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
