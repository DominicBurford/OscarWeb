using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;

using Common.Models;
using Microsoft.Extensions.Primitives;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class UserRoleAdminPageModelServiceTests
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
                if (string.IsNullOrEmpty(UserRoleAdminPageModelServiceTests._endpoint))
                {
                    UserRoleAdminPageModelServiceTests._endpoint = UserRoleAdminPageModelServiceTests.Rooturl;
                }
                return UserRoleAdminPageModelServiceTests._endpoint;
            }
            set { UserRoleAdminPageModelServiceTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            UserRoleAdminPageModelServiceTests.Endpoint = UserRoleAdminPageModelServiceTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {UserRoleAdminPageModelServiceTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    UserRoleAdminPageModelServiceTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {UserRoleAdminPageModelServiceTests.Endpoint}");
        }

        [TestMethod]
        public void GetRolesForDisplayTests()
        {
            //Arrange
            var allRoles = UserRoleAdminPageModelServiceTests.GetAllRoleModels();
            var user = UserRoleAdminPageModelServiceTests.GetUser();

            //Act
            var result = new UserRoleAdminPageModelService().GetRolesForDisplay(user, allRoles);

            //Assert
            Assert.IsNotNull(result);

            var kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);
            
            List<string> allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 2);
            Assert.IsFalse(allRolesList.Contains("Role1"));
            Assert.IsTrue(allRolesList.Contains("Role2"));
            Assert.IsTrue(allRolesList.Contains("Role3"));
            
            kvp = result.First(r => r.Key == "UserRoles");
            Assert.AreEqual("UserRoles", kvp.Key);

            List<string> userRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(userRolesList);
            Assert.IsTrue(userRolesList.Count == 1);
            Assert.IsTrue(userRolesList.Contains("Role1"));
        }

        [TestMethod]
        public void GetRolesForDisplayNullUserRolesTests()
        {
            //Arrange
            var allRoles = UserRoleAdminPageModelServiceTests.GetAllRoleModels();
            var user = UserRoleAdminPageModelServiceTests.GetUserWithNullRoles();

            //Act
            var result = new UserRoleAdminPageModelService().GetRolesForDisplay(user, allRoles);

            //Assert
            Assert.IsNotNull(result);

            var kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);

            List<string> allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 3);
            Assert.IsTrue(allRolesList.Contains("Role1"));
            Assert.IsTrue(allRolesList.Contains("Role2"));
            Assert.IsTrue(allRolesList.Contains("Role3"));

            kvp = result.First(r => r.Key == "UserRoles");
            Assert.AreEqual("UserRoles", kvp.Key);

            List<string> userRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(userRolesList);
            Assert.IsTrue(userRolesList.Count == 0);
        }

        [TestMethod]
        public void GetRolesForDisplayInvalidTests()
        {
            //Arrange
            var allRoles = UserRoleAdminPageModelServiceTests.GetAllRoleModels();
            var user = UserRoleAdminPageModelServiceTests.GetUser();

            //Act
            var result = new UserRoleAdminPageModelService().GetRolesForDisplay(null, allRoles);

            //Assert
            Assert.IsNotNull(result);
            var kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);

            List<string> allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 3);
            Assert.IsTrue(allRolesList.Contains("Role1"));
            Assert.IsTrue(allRolesList.Contains("Role2"));
            Assert.IsTrue(allRolesList.Contains("Role3"));

            result = new UserRoleAdminPageModelService().GetRolesForDisplay(user, null);

            //Assert
            Assert.IsNotNull(result);
            kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);

            allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 0);

            var copyuser = user;
            copyuser.Roles = null;
            result = new UserRoleAdminPageModelService().GetRolesForDisplay(copyuser, allRoles);

            //Assert
            Assert.IsNotNull(result);
            kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);

            allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 3);
            Assert.IsTrue(allRolesList.Contains("Role1"));
            Assert.IsTrue(allRolesList.Contains("Role2"));
            Assert.IsTrue(allRolesList.Contains("Role3"));

            var copyallroles = allRoles;
            copyallroles.Roles = null;

            result = new UserRoleAdminPageModelService().GetRolesForDisplay(user, copyallroles);

            //Assert
            Assert.IsNotNull(result);
            kvp = result.First(r => r.Key == "AllRoles");
            Assert.AreEqual("AllRoles", kvp.Key);

            allRolesList = kvp.Value as List<string>;
            Assert.IsNotNull(allRolesList);
            Assert.IsTrue(allRolesList.Count == 0);
        }

        [TestMethod]
        public void ProcessSelectedUserRolesTests()
        {
            //Arrange
            var allRoles = UserRoleAdminPageModelServiceTests.GetAllRoleModels();
            var selectedRoles = UserRoleAdminPageModelServiceTests.GetSelectedRoles();

            //Act
            var result = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(selectedRoles, allRoles);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Exists(r => r.RoleId == 1 && r.RoleName == "Role1"));
        }

        [TestMethod]
        public void ProcessSelectedUserRolesInvalidTests()
        {
            //Arrange
            var allRoles = UserRoleAdminPageModelServiceTests.GetAllRoleModels();
            var selectedRoles = UserRoleAdminPageModelServiceTests.GetSelectedRoles();

            //Act
            var result = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(new StringValues(), allRoles);

            //Assert
            Assert.IsNull(result);

            //Act
            result = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(selectedRoles, null);

            //Assert
            Assert.IsNull(result);

            RoleModels emptyRoles = new RoleModels {Roles = null};

            //Act
            result = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(selectedRoles, emptyRoles);

            //Assert
            Assert.IsNull(result);

            emptyRoles = new RoleModels { Roles = new List<RoleModel>() };

            //Act
            result = new UserRoleAdminPageModelService().ProcessSelectedUserRoles(selectedRoles, emptyRoles);

            //Assert
            Assert.IsNull(result);
        }

        private static RoleModels GetAllRoleModels()
        {
            RoleModels result = new RoleModels {Roles = new List<RoleModel>()};
            RoleModel role = new RoleModel
            {
                Id = 1,
                Name = "Role1"
            };
            result.Roles.Add(role);
            role = new RoleModel
            {
                Id = 2,
                Name = "Role2"
            };
            result.Roles.Add(role);
            role = new RoleModel
            {
                Id = 3,
                Name = "Role3"
            };
            result.Roles.Add(role);
            return result;
        }

        private static UserModel GetUser()
        {
            UserModel result = new UserModel {Roles = new List<UserRolesModel>()};
            UserRolesModel roles = new UserRolesModel
            {
                RoleId = 1,
                RoleName = "Role1"
            };
            result.Roles.Add(roles);
            return result;
        }

        private static UserModel GetUserWithNullRoles()
        {
            UserModel result = new UserModel { Roles = null };
            return result;
        }

        private static StringValues GetSelectedRoles()
        {
            string[] strarray = { "Role1" };
            StringValues selectedroles = new StringValues(strarray);
            return selectedroles;
        }
    }
}
