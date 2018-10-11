using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;

using Common.Models;

namespace OscarWeb.Tests.Extensions
{
    [TestClass]
    public class DocumentTypeModelsExtensionsTests
    {
        [TestMethod]
        public void ToSelectListItemsTests()
        {
            //Arrange
            DocumentTypeModels models = new DocumentTypeModels();
            DocumentTypeModel model = new DocumentTypeModel
            {
                Id = 1,
                Description = "Item1",
                Extension = "ext1"
            };
            models.DocumentTypes.Add(model);
            model = new DocumentTypeModel
            {
                Id = 2,
                Description = "Item2",
                Extension = "ext2"
            };
            models.DocumentTypes.Add(model);

            //Act
            var result = models.ToSelectListItems();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.Exists(i => i.Value == "1" && i.Text == "Item1 (ext1)"));
            Assert.IsTrue(result.Exists(i => i.Value == "2" && i.Text == "Item2 (ext2)"));
        }

        [TestMethod]
        public void ToSelectListItemsInvalidTests()
        {
            //Arrange
            DocumentTypeModels models = new DocumentTypeModels {DocumentTypes = null};
            
            //Act
            var result = models.ToSelectListItems();

            //Assert
            Assert.IsNull(result);
        }
    }
}
