using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Models;

namespace OscarWeb.Tests.Models
{
    [TestClass]
    public class WebServicesModelTests
    {
        [TestMethod]
        public void GetSetTests()
        {
            //Act
            const string endpoint = "http://some.test.endpoint/";
            WebServicesModel model = new WebServicesModel {Endpoint = endpoint};

            //Assert
            Assert.AreEqual(endpoint, model.Endpoint);
        }
    }
}
