using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Primitives;

using OscarWeb.Constants;
using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class DocumentManagerPageModelServiceTests
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
                if (string.IsNullOrEmpty(DocumentManagerPageModelServiceTests._endpoint))
                {
                    DocumentManagerPageModelServiceTests._endpoint = DocumentManagerPageModelServiceTests.Rooturl;
                }
                return DocumentManagerPageModelServiceTests._endpoint;
            }
            set { DocumentManagerPageModelServiceTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            DocumentManagerPageModelServiceTests.Endpoint = DocumentManagerPageModelServiceTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {DocumentManagerPageModelServiceTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    DocumentManagerPageModelServiceTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {DocumentManagerPageModelServiceTests.Endpoint}");
        }

        [TestMethod]
        public async Task GetDocumentForParentUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var response = await new DocumentManagerPageModelService().GetDocumentForParentUser(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Documents);
            Assert.IsTrue(response.Documents.Any());
            Assert.IsTrue(response.Documents.Exists(d => d.Description == "Unit test document"));
        }

        [TestMethod]
        public async Task GetDocumentByIdTests()
        {
            //Act - firstly get the user object as we need the user's ID
            const int documentId = 4;
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var response = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentId);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(documentId, response.Id);
        }

        [TestMethod]
        public async Task GetDocumentForParentUserInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var response = await new DocumentManagerPageModelService().GetDocumentForParentUser(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentForParentUser("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentForParentUser(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, 0);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentForParentUser(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", 0);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentForParentUser(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", -1);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetDocumentsForDocumentTreeTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Documents);
            Assert.IsTrue(response.Documents.Any());
        }

        [TestMethod]
        public async Task GetDocumentsForDocumentTreeInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree("", DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentsForDocumentTree(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "");

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetDocumentByIdInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            const int documentId = 4;
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the document
            var response = await new DocumentManagerPageModelService().GetDocumentById(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentById("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, documentId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", documentId);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", 0);

            //Assert
            Assert.IsNull(response);

            response = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", -1);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetToolbarForAdminUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const string toolbarname = "Unit Test Toolbar";

            //Act - fetch the toolbar
            var response = await new DocumentManagerPageModelService().GetToolbarForUser(user.Email, Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ToolbarItems);
            Assert.IsTrue(response.ToolbarItems.Any());
            Assert.IsTrue(response.ToolbarItems.Exists(t => t.DisplayText == "Admin"));
        }

        [TestMethod]
        public async Task GetToolbarForSuperUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.SuperUserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(SuperUserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const string toolbarname = "Unit Test Toolbar";

            //Act - fetch the toolbar
            var response = await new DocumentManagerPageModelService().GetToolbarForUser(user.Email, Rooturl, encodedId, toolbarname);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ToolbarItems);
            Assert.IsTrue(response.ToolbarItems.Any());
            Assert.IsTrue(response.ToolbarItems.Exists(t => t.DisplayText == "Admin"));
            Assert.IsTrue(response.ToolbarItems.Exists(t => t.DisplayText == "Developer"));
            Assert.IsTrue(response.ToolbarItems.Exists(t => t.DisplayText == "Designer"));
            Assert.IsTrue(response.ToolbarItems.Exists(t => t.DisplayText == "Tester"));
        }

        [TestMethod]
        public void GetDocumentForDisplayTests()
        {
            //Arrange
            const int userId = 1;
            DocumentsModel model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = DateTime.MinValue,
                Created = new DateTime(2018, 1, 1),
                UploadedBy = userId,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = true
            };

            //Act
            var response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, userId);

            //Assert
            Assert.IsNotNull(response);

            var kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.DocumentOwner, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual("Never", kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);


            model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = DateTime.MinValue,
                Created = new DateTime(2018, 1, 1),
                UploadedBy = 1,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = true
            };

            //Act
            const int differentUserId = 2;
            response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, differentUserId);

            //Assert
            Assert.IsNotNull(response);

            kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.DocumentSubscriber, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual("Never", kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);


            model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = new DateTime(2018, 1, 1),
                Created = new DateTime(2018, 1, 1),
                UploadedBy = 1,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = true
            };

            //Act
            response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, userId);

            //Assert
            Assert.IsNotNull(response);

            kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.DocumentOwner, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);
            
            model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = new DateTime(2018, 1, 1),
                Created = new DateTime(2018, 1, 1),
                UploadedBy = 2,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = true
            };

            //Act
            response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, userId);

            //Assert
            Assert.IsNotNull(response);

            kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.DocumentSubscriber, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);

            model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = new DateTime(2018, 1, 1),
                Created = new DateTime(2018, 1, 1),
                UploadedBy = 1,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = false
            };

            //Act
            response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, userId);

            //Assert
            Assert.IsNotNull(response);

            kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.FolderOwner, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);

            model = new DocumentsModel
            {
                Description = "Description",
                LastViewed = new DateTime(2018, 1, 1),
                Created = new DateTime(2018, 1, 1),
                UploadedBy = 2,
                UploadedByUsername = "Unit.Test",
                DocumentTypeDescription = "DocumentType",
                DocumentCategoryDescription = "DocumentCategory",
                IsDocument = false
            };

            //Act
            response = new DocumentManagerPageModelService().GetDocumentForDisplay(model, userId);

            //Assert
            Assert.IsNotNull(response);

            kvp = response.First(r => r.Key == "OwnerSubscriber");
            Assert.AreEqual("OwnerSubscriber", kvp.Key);
            Assert.AreEqual(StringConstants.FolderSubscriber, kvp.Value);

            kvp = response.First(r => r.Key == "Description");
            Assert.AreEqual("Description", kvp.Key);
            Assert.AreEqual("Description", kvp.Value.ToString());

            kvp = response.First(r => r.Key == "LastView");
            Assert.AreEqual("LastView", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "Created");
            Assert.AreEqual("Created", kvp.Key);
            Assert.AreEqual(new DateTime(2018, 1, 1), kvp.Value);

            kvp = response.First(r => r.Key == "UploadedByUsername");
            Assert.AreEqual("UploadedByUsername", kvp.Key);
            Assert.AreEqual("Unit.Test", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentTypeDescription");
            Assert.AreEqual("DocumentTypeDescription", kvp.Key);
            Assert.AreEqual("DocumentType", kvp.Value);

            kvp = response.First(r => r.Key == "DocumentCategoryDescription");
            Assert.AreEqual("DocumentCategoryDescription", kvp.Key);
            Assert.AreEqual("DocumentCategory", kvp.Value);
        }

        [TestMethod]
        public async Task GetDocumentDownloadTests()
        {
            //Arrange
            const int documentId = 4;
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            var response =
                await new DocumentManagerPageModelService().GetDocumentDownload(DocumentManagerPageModelServiceTests.Rooturl,
                    encodedId, documentId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.BlobInformation);
            Assert.IsNotNull(response.ImageBytes);
            Assert.IsTrue(response.ImageBytes.Length > 0);
        }

        [TestMethod]
        public async Task GetDocumentTypesTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentTypes(
                DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl,
                encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentTypes);
            Assert.IsTrue(response.DocumentTypes.Any());
            Assert.IsTrue(response.DocumentTypes.Exists(t => t.Description == "Text Format"));
        }

        [TestMethod]
        public async Task GetDocumentCategoriesTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentCategories(
                DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl,
                encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentCategories);
            Assert.IsTrue(response.DocumentCategories.Any());
            Assert.IsFalse(response.DocumentCategories.Exists(t => t.Description == "Unit Testing"));
        }

        [TestMethod]
        public void GetDocumentTypeForDocumentTests()
        {
            //Arrange
            var service = new DocumentManagerPageModelService();
            var documentTypes = DocumentManagerPageModelServiceTests.GetUnitTestDocumentTypes();

            //Act
            var result = service.GetDocumentTypeForDocument(1, documentTypes);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Item1", result.Description);

            //Act
            result = service.GetDocumentTypeForDocument(2, documentTypes);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("Item2", result.Description);

            //Act
            result = service.GetDocumentTypeForDocument(3, documentTypes);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual("Item3", result.Description);
        }

        [TestMethod]
        public void GetDocumentTypeForDocumentInvalidTests()
        {
            //Arrange
            var service = new DocumentManagerPageModelService();
            var documentTypes = DocumentManagerPageModelServiceTests.GetUnitTestDocumentTypes();

            //Act
            var result = service.GetDocumentTypeForDocument(0, documentTypes);

            //Assert
            Assert.IsNull(result);

            //Act
            result = service.GetDocumentTypeForDocument(-1, documentTypes);

            //Assert
            Assert.IsNull(result);

            //Act
            result = service.GetDocumentTypeForDocument(1, null);

            //Assert
            Assert.IsNull(result);

            documentTypes.DocumentTypes = null;

            //Act
            result = service.GetDocumentTypeForDocument(1, null);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UploadGetDeleteDocumentImageTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);
            string encodedId = user.Id.ToString().Base64Encode();
            var documentImage = await this.GenerateUnitTestDocumentImage(user);

            //Assert
            Assert.IsNotNull(documentImage);
            Assert.IsNotNull(documentImage.Document);
            Assert.IsNotNull(documentImage.Image);

            //Act
            var success = await new DocumentManagerPageModelService().UploadDocumentImage(user.Email,
                DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentImage);

            //Assert
            Assert.IsTrue(success);

            DocumentsModel model = new DocumentsModel
            {
                CompanyId = documentImage.Document.CompanyId,
                UploadedBy = documentImage.UploadedBy,
                Name = documentImage.Document.Name
            };

            //Act - fetch the document
            var document =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNotNull(document);
            Assert.AreEqual(model.Name, document.Name);

            //Act - delete the document
            var deleted =
                await new DocumentManagerPageModelService().DeleteDocument(user.Email, Rooturl, encodedId, document.Id);

            //Assert
            Assert.IsTrue(deleted);
        } 

        [TestMethod]
        public void ToDocumentSubscribersTests()
        {
            //Arrange
            string[] values = { "unittest1", "unittest2"};
            StringValues subscribers = new StringValues(values);
            UserModels users = new UserModels();
            UserModel user1 = new UserModel {UserName = "unittest1", Id = 1};
            users.Users.Add(user1);
            UserModel user2 = new UserModel { UserName = "unittest2", Id = 2};
            users.Users.Add(user2);

            //Act
            var result = new DocumentManagerPageModelService().ToDocumentSubscribers(subscribers, users);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.Exists(u => u.UserId == 1));
            Assert.IsTrue(result.Exists(u => u.UserId == 2));
        }

        [TestMethod]
        public void ToDocumentSubscribersInvalidTests()
        {
            //Arrange
            string[] values = { "unittest1", "unittest2" };
            StringValues subscribers = new StringValues(values);
            UserModels users = new UserModels();
            UserModel user1 = new UserModel { UserName = "unittest1", Id = 1 };
            users.Users.Add(user1);
            UserModel user2 = new UserModel { UserName = "unittest2", Id = 2 };
            users.Users.Add(user2);

            //Act
            var result = new DocumentManagerPageModelService().ToDocumentSubscribers(new StringValues(), users);

            //Assert
            Assert.IsNull(result);

            //Act
            result = new DocumentManagerPageModelService().ToDocumentSubscribers(subscribers, null);

            //Assert
            Assert.IsNull(result);

            //Act
            result = new DocumentManagerPageModelService().ToDocumentSubscribers(subscribers, new UserModels());

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetUserIdFromUserListTests()
        {
            //Arrange
            UserModels users = new UserModels();
            UserModel user1 = new UserModel { UserName = "unittest1", Id = 1 };
            users.Users.Add(user1);
            UserModel user2 = new UserModel { UserName = "unittest2", Id = 2 };
            users.Users.Add(user2);

            //Act
            int result = new DocumentManagerPageModelService().GetUserIdFromUserList("unittest1", users);

            //Assert
            Assert.AreEqual(1, result);

            result = new DocumentManagerPageModelService().GetUserIdFromUserList("unittest2", users);

            //Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetUserIdFromUserListInvalidTests()
        {
            //Arrange
            UserModels users = new UserModels();
            UserModel user1 = new UserModel { UserName = "unittest1", Id = 1 };
            users.Users.Add(user1);
            UserModel user2 = new UserModel { UserName = "unittest2", Id = 2 };
            users.Users.Add(user2);

            //Act
            int result = new DocumentManagerPageModelService().GetUserIdFromUserList("", users);

            //Assert
            Assert.AreEqual(0, result);

            result = new DocumentManagerPageModelService().GetUserIdFromUserList(null, users);

            //Assert
            Assert.AreEqual(0, result);

            result = new DocumentManagerPageModelService().GetUserIdFromUserList("unittest1", null);

            //Assert
            Assert.AreEqual(0, result);

            result = new DocumentManagerPageModelService().GetUserIdFromUserList("unittest1", new UserModels());

            //Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IsFileTypeValidTests()
        {
            //Arrange
            DocumentTypeModels documentTypes = new DocumentTypeModels();
            DocumentTypeModel documentType = new DocumentTypeModel {Extension = "txt"};
            documentTypes.DocumentTypes.Add(documentType);
            documentType = new DocumentTypeModel { Extension = "png" };
            documentTypes.DocumentTypes.Add(documentType);
            documentType = new DocumentTypeModel { Extension = "jpg" };
            documentTypes.DocumentTypes.Add(documentType);

            DocumentManagerPageModelService service = new DocumentManagerPageModelService();

            //Act
            bool result = service.IsFileTypeSupported("txt", documentTypes);

            //Assert
            Assert.IsTrue(result);

            //Act
            result = service.IsFileTypeSupported("png", documentTypes);

            //Assert
            Assert.IsTrue(result);

            //Act
            result = service.IsFileTypeSupported("jpg", documentTypes);

            //Assert
            Assert.IsTrue(result);

            //Act
            result = service.IsFileTypeSupported("jpg1", documentTypes);

            //Assert
            Assert.IsFalse(result);

            //Act
            result = service.IsFileTypeSupported("doc", documentTypes);

            //Assert
            Assert.IsFalse(result);

            //Act
            result = service.IsFileTypeSupported("xls", documentTypes);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsFileTypeValidInvalidTests()
        {
            //Arrange
            DocumentTypeModels documentTypes = new DocumentTypeModels();
            DocumentTypeModel documentType = new DocumentTypeModel { Extension = "txt" };
            documentTypes.DocumentTypes.Add(documentType);
            documentType = new DocumentTypeModel { Extension = "png" };
            documentTypes.DocumentTypes.Add(documentType);
            documentType = new DocumentTypeModel { Extension = "jpg" };
            documentTypes.DocumentTypes.Add(documentType);

            DocumentManagerPageModelService service = new DocumentManagerPageModelService();

            //Act
            bool result = service.IsFileTypeSupported("", documentTypes);

            //Assert
            Assert.IsFalse(result);

            //Act
            result = service.IsFileTypeSupported(null, documentTypes);

            //Assert
            Assert.IsFalse(result);

            //Act
            result = service.IsFileTypeSupported("txt", null);

            //Assert
            Assert.IsFalse(result);

            //Act
            result = service.IsFileTypeSupported("txt", new DocumentTypeModels());

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateDocumentTests()
        {
            //Act - firstly get the user object as we need the user's ID
            const int documentId = 4;
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var document = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentId);

            //Assert
            Assert.IsNotNull(document);
            Assert.AreEqual(documentId, document.Id);

            //update the document description
            document.Description = $"Unit test document EDITED {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
            document.Active = true;  

            var response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, document);

            //Assert
            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task UpdateDocumentInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            const int documentId = 4;
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - fetch the documents
            var document = await new DocumentManagerPageModelService().GetDocumentById(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, documentId);

            //Assert
            Assert.IsNotNull(document);
            Assert.AreEqual(documentId, document.Id);

            //update the document description
            document.Description = $"Unit test document EDITED {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
            document.Active = true;

            var response =
                await new DocumentManagerPageModelService().UpdateDocument("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, "", encodedId, document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, null, encodedId, document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, document);

            //Assert
            Assert.IsFalse(response);

            response =
                await new DocumentManagerPageModelService().UpdateDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, null);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task GetDocumentByModelInvalidTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            DocumentsModel model = new DocumentsModel();
            
            //Act
            var response =
                await new DocumentManagerPageModelService().GetDocumentByModel("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", model);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, model);

            //Assert
            Assert.IsNull(response);

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, null);

            //Assert
            Assert.IsNull(response);

            model.Name = "";
            model.UploadedBy = 1;
            model.CompanyId = 1;

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNull(response);

            model.Name = "Name";
            model.UploadedBy = 0;
            model.CompanyId = 1;

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNull(response);

            model.Name = "Name";
            model.UploadedBy = 1;
            model.CompanyId = 0;

            //Act
            response =
                await new DocumentManagerPageModelService().GetDocumentByModel(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetDocumentDownloadInvalidTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentDownload(DocumentManagerPageModelServiceTests.Rooturl, "", 1);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentDownload(DocumentManagerPageModelServiceTests.Rooturl, null, 1);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentDownload(DocumentManagerPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentDownload(DocumentManagerPageModelServiceTests.Rooturl, encodedId, -1);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task UploadDocumentImageInvalidTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            DocumentImageModel model = new DocumentImageModel();

            //Act
            var response =
                await new DocumentManagerPageModelService().UploadDocumentImage("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().UploadDocumentImage(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().UploadDocumentImage(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", model);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().UploadDocumentImage(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, model);

            //Assert
            Assert.IsFalse(response);

            model.Document = new DocumentsModel();
            model.Image = null;

            //Act
            response =
                await new DocumentManagerPageModelService().UploadDocumentImage(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsFalse(response);

            model.Document = null;
            model.Image = new ImageStreamModel();

            //Act
            response =
                await new DocumentManagerPageModelService().UploadDocumentImage(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, model);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task DeleteDocumentInvalidTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(DocumentManagerPageModelServiceTests.UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response =
                await new DocumentManagerPageModelService().DeleteDocument("", DocumentManagerPageModelServiceTests.Rooturl, encodedId, 1);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().DeleteDocument(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId, 1);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().DeleteDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, "", 1);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().DeleteDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, null, 1);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().DeleteDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsFalse(response);

            //Act
            response =
                await new DocumentManagerPageModelService().DeleteDocument(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId, -1);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task NotifySubscribersTests()
        {
            //Arrange
            DocumentsModel document = new DocumentsModel
            {
                UploadedByUsername = "MR UNIT TEST",
                Name = "Unit Test Document.txt",
                Subscribers = new List<DocumentSubscriberModel>(),
                Active = true
            };
            var subscriber = new DocumentSubscriberModel
            {
                Username = "Dominic.Burford",
                Email = "dominic.burford@grosvenor-leasing.co.uk"
            };
            document.Subscribers.Add(subscriber);

            //Act
            var response = await new DocumentManagerPageModelService().NotifySubscribers(document, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task NotifySubscribersInvalidTests()
        {
            //Arrange
            DocumentsModel document = new DocumentsModel
            {
                UploadedByUsername = "MR UNIT TEST",
                Name = "Unit Test Document.txt",
                Subscribers = new List<DocumentSubscriberModel>(),
                Active = true
            };
            
            //Act
            var response = await new DocumentManagerPageModelService().NotifySubscribers(null, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            //Act
            response = await new DocumentManagerPageModelService().NotifySubscribers(new DocumentsModel(), DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            //Act
            response = await new DocumentManagerPageModelService().NotifySubscribers(document, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            var subscriber = new DocumentSubscriberModel
            {
                Username = "Dominic.Burford",
                Email = "dominic.burford@grosvenor-leasing.co.uk"
            };
            document.Subscribers.Add(subscriber);

            //Act
            response = await new DocumentManagerPageModelService().NotifySubscribers(document, null);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public void UpdateDocumentSubscribersTests()
        {
            //Arrange
            DocumentsModel document = new DocumentsModel
            {
                UploadedByUsername = "MR UNIT TEST",
                Name = "Unit Test Document.txt",
                Subscribers = new List<DocumentSubscriberModel>(),
                Active = true
            };
            var subscriber = new DocumentSubscriberModel
            {
                UserId = 1
            };
            document.Subscribers.Add(subscriber);

            UserModels users = new UserModels();
            UserModel user = new UserModel
            {
                UserName = "Unit.Test",
                Id = 1,
                Email = "unittest@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);
            user = new UserModel
            {
                UserName = "Unit.Test2",
                Id = 2,
                Email = "unittest2@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);
            user = new UserModel
            {
                UserName = "Unit.Test3",
                Id = 3,
                Email = "unittest3@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);

            //Act
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(document, users);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Subscribers);
            Assert.IsTrue(document.Subscribers.Any());

            var founduser = document.Subscribers.Find(u => u.UserId == 1);

            //Assert
            Assert.IsNotNull(founduser);
            Assert.AreEqual(1, founduser.UserId);
            Assert.AreEqual("Unit.Test", founduser.Username);
            Assert.AreEqual("unittest@grosvenor-leasing.co.uk", founduser.Email);
        }

        [TestMethod]
        public void UpdateDocumentSubscribersInvalidTests()
        {
            //Arrange
            UserModels users = new UserModels();
            UserModel user = new UserModel
            {
                UserName = "Unit.Test",
                Id = 1,
                Email = "unittest@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);
            user = new UserModel
            {
                UserName = "Unit.Test2",
                Id = 2,
                Email = "unittest2@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);
            user = new UserModel
            {
                UserName = "Unit.Test3",
                Id = 3,
                Email = "unittest3@grosvenor-leasing.co.uk"
            };
            users.Users.Add(user);

            //Act
            var document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(null, users);

            //Assert
            Assert.IsNull(document);

            //Act
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(new DocumentsModel(), users);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNull(document.Subscribers);

            //Act
            var documentToUpdate = new DocumentsModel {Subscribers = null};
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(documentToUpdate, users);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNull(document.Subscribers);

            //Act
            documentToUpdate = new DocumentsModel {Subscribers = null};
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(documentToUpdate, users);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNull(document.Subscribers);

            //Arrange
            documentToUpdate = new DocumentsModel
            {
                UploadedByUsername = "MR UNIT TEST",
                Name = "Unit Test Document.txt",
                Subscribers = new List<DocumentSubscriberModel>(),
                Active = true
            };
            var subscriber = new DocumentSubscriberModel
            {
                UserId = 1
            };
            documentToUpdate.Subscribers.Add(subscriber);

            //Act
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(documentToUpdate, null);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Subscribers);
            Assert.IsTrue(document.Subscribers.Any());
            Assert.IsTrue(document.Subscribers.Find(u => u.UserId == 1) != null);

            //Act
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(documentToUpdate, new UserModels());

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Subscribers);
            Assert.IsTrue(document.Subscribers.Any());
            Assert.IsTrue(document.Subscribers.Find(u => u.UserId == 1) != null);

            //Arrange
            users.Users = null;

            //Act
            document = new DocumentManagerPageModelService().UpdateDocumentSubscribers(documentToUpdate, users);

            //Assert
            Assert.IsNotNull(document);
            Assert.IsNotNull(document.Subscribers);
            Assert.IsTrue(document.Subscribers.Any());
            Assert.IsTrue(document.Subscribers.Find(u => u.UserId == 1) != null);
        }

        [TestMethod]
        public async Task GetDocumentEventsTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            int companyId = user.CompanyId;
            const int documentId = 4;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                userId.ToString(), documentId.ToString(), DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentEvents);
            Assert.IsTrue(response.DocumentEvents.Any());
            Assert.AreEqual(response.DocumentEvents.Count, response.DocumentEvents.FindAll(d => d.CompanyId == companyId && d.UserId == userId && d.DocumentId == documentId).Count);
        }

        [TestMethod]
        public async Task GetDocumentEventsForCompanyTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            int companyId = user.CompanyId;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                null, null, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentEvents);
            Assert.IsTrue(response.DocumentEvents.Any());
            Assert.AreEqual(response.DocumentEvents.Count, response.DocumentEvents.FindAll(d => d.CompanyId == companyId).Count);
        }

        [TestMethod]
        public async Task GetDocumentEventsForUserTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentEvents(null, userId.ToString(), null, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentEvents);
            Assert.IsTrue(response.DocumentEvents.Any());
            Assert.AreEqual(response.DocumentEvents.Count, response.DocumentEvents.FindAll(d => d.UserId == userId).Count);
        }

        [TestMethod]
        public async Task GetDocumentEventsForDocumentTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            const int documentId = 4;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentEvents(null, null, documentId.ToString(), DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.DocumentEvents);
            Assert.IsTrue(response.DocumentEvents.Any());
            Assert.AreEqual(response.DocumentEvents.Count, response.DocumentEvents.FindAll(d => d.DocumentId == documentId).Count);
        }

        [TestMethod]
        public async Task GetDocumentEventsInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            int companyId = user.CompanyId;
            const int documentId = 4;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                userId.ToString(), documentId.ToString(), null, encodedId);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                userId.ToString(), documentId.ToString(), "", encodedId);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                userId.ToString(), documentId.ToString(), DocumentManagerPageModelServiceTests.Rooturl, null);

            //Assert
            Assert.IsNull(response);

            //Act
            response = await new DocumentManagerPageModelService().GetDocumentEvents(companyId.ToString(),
                userId.ToString(), documentId.ToString(), DocumentManagerPageModelServiceTests.Rooturl, "");

            //Assert
            Assert.IsNull(response);

        }

        [TestMethod]
        public async Task UpdateDocumentTreeParentTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();
            const int parentDocumentId = 6000;
            const int subDocumentId = 6001;

            //Act - update the document's parent ID
            var result = await new DocumentManagerPageModelService().UpdateDocumentTreeParent(0, subDocumentId,
                DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsTrue(result);

            result = await new DocumentManagerPageModelService().UpdateDocumentTreeParent(parentDocumentId, subDocumentId,
                DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetUnreadDocumentsTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var result = await new DocumentManagerPageModelService().GetUnreadDocuments(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Documents);
            Assert.IsTrue(result.Documents.Any());
        }

        [TestMethod]
        public async Task GetUnreadDocumentsInvalidTests()
        {
            //Arrange
            var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var result = await new DocumentManagerPageModelService().GetUnreadDocuments(null, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNull(result);

            //Act
            result = await new DocumentManagerPageModelService().GetUnreadDocuments("", DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNull(result);

            //Act
            result = await new DocumentManagerPageModelService().GetUnreadDocuments(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl, null);

            //Assert
            Assert.IsNull(result);

            //Act
            result = await new DocumentManagerPageModelService().GetUnreadDocuments(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl, "");

            //Assert
            Assert.IsNull(result);

            //Act
            result = await new DocumentManagerPageModelService().GetUnreadDocuments(DocumentManagerPageModelServiceTests.UserEmail, null, encodedId);

            //Assert
            Assert.IsNull(result);

            //Act
            result = await new DocumentManagerPageModelService().GetUnreadDocuments(DocumentManagerPageModelServiceTests.UserEmail, "", encodedId);

            //Assert
            Assert.IsNull(result);
        }

        private async Task<DocumentImageModel> GenerateUnitTestDocumentImage(UserModel user)
        {
            string filename = $"oscar_web_unittest_{Guid.NewGuid().ToString()}.txt";

            //var user = await new UserServices().GetUserDetails(DocumentManagerPageModelServiceTests.UserEmail, DocumentManagerPageModelServiceTests.Rooturl);
            string encodedId = user.Id.ToString().Base64Encode();

            //fetch the document types and categories
            var types = await new DocumentManagerPageModelService().GetDocumentTypes(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId);
            var categories = await new DocumentManagerPageModelService().GetDocumentCategories(user.Email, DocumentManagerPageModelServiceTests.Rooturl, encodedId);

            var textType = types.DocumentTypes.Find(t => t.Description == "Text Format");
            var developmentCategory = categories.DocumentCategories.Find(c => c.Description == "Development");

            var document = new DocumentImageModel {UploadedBy = user.Id};
            document.Document = new DocumentsModel
            {
                Description = "This is a test document",
                CompanyId = user.CompanyId,
                CompanyName = user.CompanyName,
                Category = developmentCategory.Id,
                Name = filename,
                Active = true,
                IsDocument = true,
                IsPrivate = true,
                LastViewed = DateTime.MinValue,
                UploadedBy = document.UploadedBy,
                DocumentType = textType.Id,
                ParentId = 0
            };

            byte[] buffer = GetImage(256);

            document.Image = new ImageStreamModel
            {
                MimeType = textType.MimeType,
                ImagesList = new Dictionary<string, byte[]>
                {
                    { filename, buffer }
                }
            };

            return document;
        }

        private static byte[] GetImage(int imageSize)
        {
            var randBytes = new byte[imageSize];
            System.Security.Cryptography.RNGCryptoServiceProvider rand =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rand.GetBytes(randBytes);
            return randBytes;
        }
        
        private static DocumentTypeModels GetUnitTestDocumentTypes()
        {
            DocumentTypeModels result = new DocumentTypeModels {DocumentTypes = new List<DocumentTypeModel>()};
            DocumentTypeModel model = new DocumentTypeModel
            {
                Id = 1,
                Description = "Item1"
            };
            result.DocumentTypes.Add(model);
            model = new DocumentTypeModel
            {
                Id = 2,
                Description = "Item2"
            };
            result.DocumentTypes.Add(model);
            model = new DocumentTypeModel
            {
                Id = 3,
                Description = "Item3"
            };
            result.DocumentTypes.Add(model);
            return result;
        }
    }
}
