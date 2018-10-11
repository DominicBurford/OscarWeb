using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;
using OscarWeb.Services;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class ToolbarServicesTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";
        private const string UserEmail = "unittest@grosvenor-leasing.co.uk";
        private const string SuperUserEmail = "dominic.burford@grosvenor-leasing.co.uk";

        private static string _endpoint;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(ToolbarServicesTests._endpoint))
                {
                    ToolbarServicesTests._endpoint = ToolbarServicesTests.Rooturl;
                }
                return ToolbarServicesTests._endpoint;
            }
            set { ToolbarServicesTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            ToolbarServicesTests.Endpoint = ToolbarServicesTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {ToolbarServicesTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    ToolbarServicesTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {ToolbarServicesTests.Endpoint}");
        }

        [TestMethod]
        public async Task GetToolbarAdminUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(ToolbarServicesTests.UserEmail, ToolbarServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(ToolbarServicesTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const string toolbarname = "Unit Test Toolbar";

            //Act
            var toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNotNull(toolbar);
            Assert.IsNotNull(toolbar.ToolbarItems);
            Assert.IsTrue(toolbar.ToolbarItems.Any());
            Assert.IsTrue(toolbar.ToolbarItems.Count == 1);
            Assert.IsTrue(toolbar.ToolbarItems.Exists(t => t.DisplayText == "Admin"));
        }

        [TestMethod]
        public async Task GetToolbarSuperUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(ToolbarServicesTests.SuperUserEmail, ToolbarServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(ToolbarServicesTests.SuperUserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const string toolbarname = "Unit Test Toolbar";

            //Act
            var toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNotNull(toolbar);
            Assert.IsNotNull(toolbar.ToolbarItems);
            Assert.IsTrue(toolbar.ToolbarItems.Any());
            Assert.IsTrue(toolbar.ToolbarItems.Exists(t => t.DisplayText == "Admin"));
            Assert.IsTrue(toolbar.ToolbarItems.Exists(t => t.DisplayText == "Developer"));
            Assert.IsTrue(toolbar.ToolbarItems.Exists(t => t.DisplayText == "Designer"));
            Assert.IsTrue(toolbar.ToolbarItems.Exists(t => t.DisplayText == "Tester"));
        }

        [TestMethod]
        public async Task GetToolbarInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(ToolbarServicesTests.UserEmail, ToolbarServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(ToolbarServicesTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const string toolbarname = "Unit Test Toolbar";

            //Act
            var toolbar = await new ToolbarServices().GetToolbar(null, ToolbarServicesTests.Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNull(toolbar);

            //Act
            toolbar = await new ToolbarServices().GetToolbar("", ToolbarServicesTests.Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNull(toolbar);

            //Act
            toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, null, toolbarname);

            //Assert
            Assert.IsNull(toolbar);

            //Act
            toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, "", toolbarname);

            //Assert
            Assert.IsNull(toolbar);

            //Act
            toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, encodedId, null);

            //Assert
            Assert.IsNull(toolbar);

            //Act
            toolbar = await new ToolbarServices().GetToolbar(user.Email, ToolbarServicesTests.Rooturl, encodedId, "");

            //Assert
            Assert.IsNull(toolbar);
        }
    }
}
