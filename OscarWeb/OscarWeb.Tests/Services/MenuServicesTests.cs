using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class MenuServicesTests
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
                if (string.IsNullOrEmpty(MenuServicesTests._endpoint))
                {
                    MenuServicesTests._endpoint = MenuServicesTests.Rooturl;
                }
                return MenuServicesTests._endpoint;
            }
            set { MenuServicesTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            MenuServicesTests.Endpoint = MenuServicesTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {MenuServicesTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    MenuServicesTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {MenuServicesTests.Endpoint}");
        }

        [TestMethod]
        public async Task GetModulesItemsForAdminUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(MenuServicesTests.UserEmail, MenuServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(MenuServicesTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            const string displayText = "Unit Test Admin";
            string encodedId = userId.ToString().Base64Encode();

            var menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, 0, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == displayText));

            //Arrange 
            var submenuitem = menuitems.MenuItems.Find(m => m.DisplayText == displayText);

            //Assert
            Assert.IsNotNull(submenuitem);

            //Act - fetch the sub menu items
            int parentId = submenuitem.Id;
            menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, parentId, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Developer"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Tester"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Designer"));
        }

        [TestMethod]
        public async Task GetModulesItemsForSuperUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(MenuServicesTests.SuperUserEmail, MenuServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(MenuServicesTests.SuperUserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            const string displayText = "Unit Test Admin";
            string encodedId = userId.ToString().Base64Encode();

            var menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, 0, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Unit Test Admin"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Unit Test Development"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Unit Test Testing"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Unit Test Design"));

            //Arrange 
            var submenuitem = menuitems.MenuItems.Find(m => m.DisplayText == displayText);

            //Assert
            Assert.IsNotNull(submenuitem);

            //Act - fetch the sub menu items
            int parentId = submenuitem.Id;
            menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, parentId, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Developer"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Tester"));
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == "Admin Designer"));
        }

        [TestMethod]
        public void GetParentIdForMenuItemTests()
        {
            //Arrange
            var menu = MenuServicesTests.GetUnitTestMenu();

            //Act - fetch the parent ID
            MenuServices services = new MenuServices();
            int parentId = services.GetParentIdForMenuItem("Unit Test Admin", menu);

            //Assert
            Assert.AreEqual(1, parentId);

            parentId = services.GetParentIdForMenuItem("Unit Test Development", menu);

            //Assert
            Assert.AreEqual(2, parentId);

            parentId = services.GetParentIdForMenuItem("Unit Test Testing", menu);

            //Assert
            Assert.AreEqual(3, parentId);

            parentId = services.GetParentIdForMenuItem("Unit Test Design", menu);

            //Assert
            Assert.AreEqual(4, parentId);
        }


        [TestMethod]
        public async Task GetModulesItemsForAdminUserTestsWithoutParentId()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(MenuServicesTests.UserEmail, MenuServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(MenuServicesTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            const string displayText = "Unit Test Admin";
            string encodedId = userId.ToString().Base64Encode();

            var menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == displayText));

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Admin") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Development") == 0);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Testing") == 0);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Design") == 0);

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Developer") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Tester") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Designer") == 1);

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Developer") == 0);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Tester") == 0);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Designer") == 0);
        }

        [TestMethod]
        public async Task GetModulesItemsForSuperUserTestsWithoutParentId()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(MenuServicesTests.SuperUserEmail, MenuServicesTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(MenuServicesTests.SuperUserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            const string displayText = "Unit Test Admin";
            string encodedId = userId.ToString().Base64Encode();

            var menuitems = await new MenuServices().GetModulesItemsForUser(user.Email, MenuServicesTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(menuitems);
            Assert.IsNotNull(menuitems.MenuItems);
            Assert.IsTrue(menuitems.MenuItems.Any());
            Assert.IsTrue(menuitems.MenuItems.Exists(m => m.DisplayText == displayText));

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Admin") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Development") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Testing") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Unit Test Design") == 1);

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Developer") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Tester") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Admin Designer") == 1);

            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Developer") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Tester") == 1);
            Assert.IsTrue(menuitems.MenuItems.Count(m => m.DisplayText == "Designer") == 1);
        }
        
        [TestMethod]
        public void GetParentIdForMenuItemInvalidTests()
        {
            //Arrange
            var menu = MenuServicesTests.GetUnitTestMenu();

            //Act - fetch the parent ID
            MenuServices services = new MenuServices();
            int parentId = services.GetParentIdForMenuItem("", menu);

            //Assert
            Assert.AreEqual(-1, parentId);

            parentId = services.GetParentIdForMenuItem(null, menu);

            //Assert
            Assert.AreEqual(-1, parentId);

            parentId = services.GetParentIdForMenuItem("Unit Test Testing", null);

            //Assert
            Assert.AreEqual(-1, parentId);

            menu = new MainMenuModels {MenuItems = null};

            parentId = services.GetParentIdForMenuItem("Unit Test Testing", menu);

            //Assert
            Assert.AreEqual(-1, parentId);
        }

        [TestMethod]
        public void CleanMainMenuModelTests()
        {
            //Arrange
            var menu = MenuServicesTests.GetUnitTestMenu();
            UserModel user = new UserModel
            {
                Email = "dominic.burford@grosvenor-leasing.co.uk",
                SuperUser = true
            };

            //Act - remove all unit testing menu items from the menu
            var cleanMenu = new MenuServices().CleanMainMenuModel(menu, user);

            //Assert
            Assert.IsNotNull(cleanMenu);
            Assert.IsNotNull(cleanMenu.MenuItems);
            Assert.IsFalse(cleanMenu.MenuItems.Exists(m => m.DisplayText.ToLower().Contains("unit test")));
        }

        [TestMethod]
        public void CleanMainMenuModelInvalidTests()
        {
            //Arrange
            UserModel user = new UserModel
            {
                Email = "dominic.burford@grosvenor-leasing.co.uk",
                SuperUser = true
            };

            //Act
            var cleanMenu = new MenuServices().CleanMainMenuModel(null, user);

            //Assert
            Assert.IsNull(cleanMenu);

            MainMenuModels menu = new MainMenuModels {MenuItems = null};
            cleanMenu = new MenuServices().CleanMainMenuModel(menu, user);

            //Assert
            Assert.IsNull(cleanMenu);

            menu = MenuServicesTests.GetUnitTestMenu();
            cleanMenu = new MenuServices().CleanMainMenuModel(menu, null);

            //Assert
            Assert.IsNull(cleanMenu);
        }

        private static MainMenuModels GetUnitTestMenu()
        {
            MainMenuModels menu = new MainMenuModels { MenuItems = new List<MainMenuModel>() };
            MainMenuModel menuitem = new MainMenuModel
            {
                DisplayText = "Unit Test Admin",
                Id = 1
            };
            menu.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Unit Test Development",
                Id = 2
            };
            menu.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Unit Test Testing",
                Id = 3
            };
            menu.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Unit Test Design",
                Id = 4
            };
            menu.MenuItems.Add(menuitem);
            return menu;
        }
    }
}
