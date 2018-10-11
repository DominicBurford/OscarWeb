using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;

using Common.Models;

namespace OscarWeb.Tests.Extensions
{
    [TestClass]
    public class MainMenuModelsExtensionsTests
    {
        [TestMethod]
        public void ToTreeViewItemModelListTests()
        {
            //Arrange
            var menu = MainMenuModelsExtensionsTests.CreateMainMenu();

            //Act
            var tree = menu.ToTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());

            Assert.IsTrue(tree.Count == 2);

            var admin = tree[0];
            var documentmanager = tree[1];

            Assert.IsNotNull(admin);
            Assert.IsNotNull(documentmanager);
            Assert.AreEqual("Admin", admin.Text);
            Assert.AreEqual("Document Manager", documentmanager.Text);

            Assert.IsTrue(admin.Items.Count == 3);
            Assert.IsTrue(documentmanager.Items.Count == 2);

            var subadmin = admin.Items;
            var subdocumentmanager = documentmanager.Items;

            Assert.IsNotNull(subadmin);
            Assert.IsNotNull(subdocumentmanager);

            var companyadmin = subadmin[0];
            var roleadmin = subadmin[1];
            var useradmin = subadmin[2];

            Assert.IsNotNull(companyadmin);
            Assert.IsNotNull(roleadmin);
            Assert.IsNotNull(useradmin);

            Assert.AreEqual("Company Admin", companyadmin.Text);
            Assert.AreEqual("Role Admin", roleadmin.Text);
            Assert.AreEqual("User Admin", useradmin.Text);

            var subcompany = companyadmin.Items;

            Assert.IsNotNull(subcompany);
            Assert.IsTrue(subcompany.Count == 1);

            var addcompany = subcompany[0];

            Assert.IsNotNull(addcompany);
            Assert.AreEqual("Add Company", addcompany.Text);

            var upload = subdocumentmanager[0];
            var download = subdocumentmanager[1];

            Assert.IsNotNull(upload);
            Assert.IsNotNull(download);

            Assert.AreEqual("Upload", upload.Text);
            Assert.AreEqual("Download", download.Text);
        }

        [TestMethod]
        public void ToKendoTreeViewItemModelList()
        {
            //Arrange
            var menu = MainMenuModelsExtensionsTests.CreateMainMenu();

            //Act
            var tree = menu.ToKendoTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());

            Assert.IsTrue(tree.Count == 2);

            var admin = tree[0];
            var documentmanager = tree[1];

            Assert.IsNotNull(admin);
            Assert.IsNotNull(documentmanager);
            Assert.AreEqual("Admin", admin.Text);
            Assert.AreEqual("Document Manager", documentmanager.Text);

            Assert.IsNotNull(admin.Id);
            Assert.IsNotNull(documentmanager.Id);

            Assert.IsTrue(admin.Items.Count == 3);
            Assert.IsTrue(documentmanager.Items.Count == 2);

            var subadmin = admin.Items;
            var subdocumentmanager = documentmanager.Items;

            Assert.IsNotNull(subadmin);
            Assert.IsNotNull(subdocumentmanager);

            var companyadmin = subadmin[0];
            var roleadmin = subadmin[1];
            var useradmin = subadmin[2];

            Assert.IsNotNull(companyadmin.Id);
            Assert.IsNotNull(roleadmin.Id);
            Assert.IsNotNull(useradmin.Id);

            Assert.IsNotNull(companyadmin);
            Assert.IsNotNull(roleadmin);
            Assert.IsNotNull(useradmin);

            Assert.AreEqual("Company Admin", companyadmin.Text);
            Assert.AreEqual("Role Admin", roleadmin.Text);
            Assert.AreEqual("User Admin", useradmin.Text);

            var subcompany = companyadmin.Items;

            Assert.IsNotNull(subcompany);
            Assert.IsTrue(subcompany.Count == 1);

            var addcompany = subcompany[0];

            Assert.IsNotNull(addcompany.Id);
            
            Assert.IsNotNull(addcompany);
            Assert.AreEqual("Add Company", addcompany.Text);

            var upload = subdocumentmanager[0];
            var download = subdocumentmanager[1];

            Assert.IsNotNull(upload);
            Assert.IsNotNull(download);

            Assert.IsNotNull(upload.Id);
            Assert.IsNotNull(download.Id);

            Assert.AreEqual("Upload", upload.Text);
            Assert.AreEqual("Download", download.Text);
        }

        [TestMethod]
        public void ToKendoMenuItemModelListTests()
        {
            //Arrange
            var menu = MainMenuModelsExtensionsTests.CreateMainMenu();

            //Act
            var tree = menu.ToKendoMenuItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());

            Assert.IsTrue(tree.Count == 2);

            var admin = tree[0];
            var documentmanager = tree[1];

            Assert.IsNotNull(admin);
            Assert.IsNotNull(documentmanager);
            Assert.AreEqual("Admin", admin.Text);
            Assert.AreEqual("Document Manager", documentmanager.Text);

            Assert.IsTrue(admin.Items.Count == 3);
            Assert.IsTrue(documentmanager.Items.Count == 2);

            var subadmin = admin.Items;
            var subdocumentmanager = documentmanager.Items;

            Assert.IsNotNull(subadmin);
            Assert.IsNotNull(subdocumentmanager);

            var companyadmin = subadmin[0];
            var roleadmin = subadmin[1];
            var useradmin = subadmin[2];

            Assert.IsNotNull(companyadmin);
            Assert.IsNotNull(roleadmin);
            Assert.IsNotNull(useradmin);

            Assert.AreEqual("Company Admin", companyadmin.Text);
            Assert.AreEqual("Role Admin", roleadmin.Text);
            Assert.AreEqual("User Admin", useradmin.Text);

            var subcompany = companyadmin.Items;

            Assert.IsNotNull(subcompany);
            Assert.IsTrue(subcompany.Count == 1);

            var addcompany = subcompany[0];

            Assert.IsNotNull(addcompany);
            Assert.AreEqual("Add Company", addcompany.Text);

            var upload = subdocumentmanager[0];
            var download = subdocumentmanager[1];

            Assert.IsNotNull(upload);
            Assert.IsNotNull(download);

            Assert.AreEqual("Upload", upload.Text);
            Assert.AreEqual("Download", download.Text);
        }

        private static MainMenuModels CreateMainMenu()
        {
            MainMenuModels result = new MainMenuModels {MenuItems = new List<MainMenuModel>()};
            MainMenuModel menuitem = new MainMenuModel
            {
                DisplayText = "Admin",
                ParentId = 0,
                Id = 1
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Company Admin",
                ParentId = 1,
                Id = 2
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Add Company",
                ParentId = 2,
                Id = 3
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Role Admin",
                ParentId = 1,
                Id = 4
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "User Admin",
                ParentId = 1,
                Id = 5
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Document Manager",
                ParentId = 0,
                Id = 6
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Upload",
                ParentId = 6,
                Id = 7
            };
            result.MenuItems.Add(menuitem);
            menuitem = new MainMenuModel
            {
                DisplayText = "Download",
                ParentId = 6,
                Id = 8
            };
            result.MenuItems.Add(menuitem);
            return result;
        }
    }
}
