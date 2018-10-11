using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;

using Common.Models;

namespace OscarWeb.Tests.Extensions
{
    [TestClass]
    public class DocumentTreeModelsExtensionsTests
    {
        [TestMethod]
        public void ToKendoTreeViewItemModelListTests()
        {
            //Arrange
            var documents = DocumentTreeModelsExtensionsTests.CreateDocuments();

            //Act
            var tree = documents.ToKendoTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());

            Assert.IsTrue(tree.Count == 2);

            var development = tree[0];
            var customers = tree[1];

            Assert.IsNotNull(development);
            Assert.IsNotNull(customers);
            Assert.AreEqual("Development", development.Text);
            Assert.AreEqual("Customers", customers.Text);

            var subdevelopment = development.Items;

            Assert.IsNotNull(subdevelopment);

            var codingstandards = subdevelopment[0];
            var testing = subdevelopment[1];

            Assert.IsNotNull(codingstandards);
            Assert.IsNotNull(testing);
            Assert.AreEqual("Coding standards.docx", codingstandards.Text);
            Assert.AreEqual("Testing", testing.Text);

            var subtesting = testing.Items;

            Assert.IsNotNull(subtesting);

            var testingstrategy = subtesting[0];
            var unittesting = subtesting[1];

            Assert.IsNotNull(testing);
            Assert.AreEqual("Testing strategy.docx", testingstrategy.Text);
            Assert.IsNotNull(unittesting);
            Assert.AreEqual("Unit Testing", unittesting.Text);

            var subunittesting = unittesting.Items;

            Assert.IsNotNull(subunittesting);

            var unittestingdoc = subunittesting[0];

            Assert.IsNotNull(unittestingdoc);
            Assert.AreEqual("Unit Testing.docx", unittestingdoc.Text);
            
            var subcustomer = customers.Items;

            Assert.IsNotNull(subcustomer);

            var spirax = subcustomer[0];

            Assert.IsNotNull(spirax);
            Assert.AreEqual("Spirax", spirax.Text);

            var subspirax = spirax.Items;

            Assert.IsNotNull(subspirax);

            var contract = subspirax[0];
            var terms = subspirax[1];

            Assert.IsNotNull(contract);
            Assert.IsNotNull(terms);
            Assert.AreEqual("Contract.docx", contract.Text);
            Assert.AreEqual("Terms and conditions.docx", terms.Text);
        }

        [TestMethod]
        public void ToKendoTreeViewItemModelListTests2()
        {
            //Arrange
            var documents = DocumentTreeModelsExtensionsTests.CreateDocuments2();

            //Act
            var tree = documents.ToKendoTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());
            Assert.IsTrue(tree.Count == 3);

            var developmentfolder = tree[0];

            Assert.IsNotNull(developmentfolder);
            Assert.AreEqual("Development", developmentfolder.Text);

            var developmentsubitems = developmentfolder.Items;

            Assert.IsNotNull(developmentsubitems);
            Assert.IsTrue(developmentsubitems.Count == 1);

            var codingstandards = developmentsubitems[0];

            Assert.IsNotNull(codingstandards);
            Assert.IsTrue(codingstandards.Items.Count == 0);
            Assert.AreEqual("Coding standards.docx", codingstandards.Text);
            
            var testingdocument = tree[1];

            Assert.IsNotNull(testingdocument);
            Assert.IsTrue(testingdocument.Items.Count == 0);
            Assert.AreEqual("Testing document.docx", testingdocument.Text);

            var testingstrategy = tree[2];

            Assert.IsNotNull(testingstrategy);
            Assert.IsTrue(testingstrategy.Items.Count == 0);
            Assert.AreEqual("Testing strategy.docx", testingstrategy.Text);
        }

        [TestMethod]
        public void ToKendoTreeViewItemModelListNoSubItemsTests()
        {
            //Arrange
            var documents = DocumentTreeModelsExtensionsTests.CreateDocumentsNoSubItems();

            //Act
            var tree = documents.ToKendoTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());
            Assert.IsTrue(tree.Count == 3);

            var document = tree[0];

            Assert.IsNotNull(document);
            Assert.AreEqual("Development Strategy.docx", document.Text);

            document = tree[1];

            Assert.IsNotNull(document);
            Assert.AreEqual("Coding standards.docx", document.Text);

            document = tree[2];

            Assert.IsNotNull(document);
            Assert.AreEqual("Testing Strategy.docx", document.Text);
        }

        [TestMethod]
        public void ToKendoTreeViewItemModelListNoSubItemsTests2()
        {
            //Arrange
            var documents = DocumentTreeModelsExtensionsTests.CreateDocumentsNoSubItems2();

            //Act
            var tree = documents.ToKendoTreeViewItemModelList();

            //Assert
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Any());

            Assert.IsTrue(tree.Count == 3);

            var document = tree[0];

            Assert.IsNotNull(document);
            Assert.AreEqual("Development Strategy.docx", document.Text);

            document = tree[1];

            Assert.IsNotNull(document);
            Assert.AreEqual("Coding standards.docx", document.Text);

            document = tree[2];

            Assert.IsNotNull(document);
            Assert.AreEqual("Testing Strategy.docx", document.Text);
        }

        private static DocumentTreeModels CreateDocuments()
        {
            DocumentTreeModels result = new DocumentTreeModels();
            DocumentTreeModel document = new DocumentTreeModel
            {
                Id = 1,
                Name = "Development",
                IsDocument = false,
                ParentId = 0
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 2,
                Name = "Coding standards.docx",
                IsDocument = true,
                ParentId = 1
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 3,
                Name = "Testing",
                IsDocument = false,
                ParentId = 1
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 4,
                Name = "Testing strategy.docx",
                IsDocument = true,
                ParentId = 3
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 5,
                Name = "Customers",
                IsDocument = false,
                ParentId = 0
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 6,
                Name = "Spirax",
                IsDocument = false,
                ParentId = 5
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 7,
                Name = "Contract.docx",
                IsDocument = true,
                ParentId = 6
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 8,
                Name = "Terms and conditions.docx",
                IsDocument = true,
                ParentId = 6
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 9,
                Name = "Unit Testing",
                IsDocument = false,
                ParentId = 3
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 10,
                Name = "Unit Testing.docx",
                IsDocument = false,
                ParentId = 9
            };
            result.Documents.Add(document);
            return result;
        }

        private static DocumentTreeModels CreateDocuments2()
        {
            DocumentTreeModels result = new DocumentTreeModels();
            DocumentTreeModel document = new DocumentTreeModel
            {
                Id = 1,
                Name = "Development",
                IsDocument = false,
                ParentId = 0
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 2,
                Name = "Coding standards.docx",
                IsDocument = true,
                ParentId = 1
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 3,
                Name = "Testing document.docx",
                IsDocument = true,
                ParentId = 99
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 4,
                Name = "Testing strategy.docx",
                IsDocument = true,
                ParentId = 101
            };
            result.Documents.Add(document);
            return result;
        }

        private static DocumentTreeModels CreateDocumentsNoSubItems()
        {
            // all the documents are top level documents with a parent ID of 0

            DocumentTreeModels result = new DocumentTreeModels();
            DocumentTreeModel document = new DocumentTreeModel
            {
                Id = 1,
                Name = "Development Strategy.docx",
                IsDocument = true,
                ParentId = 0
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 2,
                Name = "Coding standards.docx",
                IsDocument = true,
                ParentId = 0
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 3,
                Name = "Testing Strategy.docx",
                IsDocument = true,
                ParentId = 0
            };
            result.Documents.Add(document);
            return result;
        }

        private static DocumentTreeModels CreateDocumentsNoSubItems2()
        {
            //a combination of top level documents (parent ID of 0) and documents with a parent that is NOT 0
            DocumentTreeModels result = new DocumentTreeModels();
            DocumentTreeModel document = new DocumentTreeModel
            {
                Id = 1,
                Name = "Development Strategy.docx",
                IsDocument = true,
                ParentId = 99
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 2,
                Name = "Coding standards.docx",
                IsDocument = true,
                ParentId = 99
            };
            result.Documents.Add(document);
            document = new DocumentTreeModel
            {
                Id = 3,
                Name = "Testing Strategy.docx",
                IsDocument = true,
                ParentId = 0
            };
            result.Documents.Add(document);
            return result;
        }
    }
}
