using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;

using Common.Models;

namespace OscarWeb.Tests.Extensions
{
    [TestClass]
    public class DocumentCategoryModelsExtensionsTests
    {
        [TestMethod]
        public void ToSelectListItemsTests()
        {
            //Arrange
            DocumentCategoryModels models = new DocumentCategoryModels();
            DocumentCategoryModel model = new DocumentCategoryModel
            {
                Id = 1,
                Description = "Item1"
            };
            models.DocumentCategories.Add(model);
            model = new DocumentCategoryModel
            {
                Id = 2,
                Description = "Item2"
            };
            models.DocumentCategories.Add(model);

            //Act
            var result = models.ToSelectListItems();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.Exists(i => i.Value == "1" && i.Text == "Item1"));
            Assert.IsTrue(result.Exists(i => i.Value == "2" && i.Text == "Item2"));
        }

        [TestMethod]
        public void ToSelectListItemsInvalidTests()
        {
            //Arrange
            DocumentCategoryModels models = new DocumentCategoryModels();
            models.DocumentCategories = null;
            
            //Act
            var result = models.ToSelectListItems();

            //Assert
            Assert.IsNull(result);
        }
    }
}
