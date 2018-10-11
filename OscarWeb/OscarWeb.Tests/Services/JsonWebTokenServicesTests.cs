using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class JsonWebTokenServicesTests
    {
        [TestMethod]
        public void CreateJsonWebtokenTests()
        {
            //Act
            const string result =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE1MTQ3NjQ4MDAuMCwic3Vic2NyaWJlciI6IkdST1NWRU5PUiJ9.aY2c7KG89e4XOe2CGwYsYVNRbljRZFAiNsD1z4hWNng";

            //Arrange
            DateTime dt = new DateTime(2018, 1, 1);
            string token = JsonWebTokenServices.CreateJsonWebtoken(dt);

            //Assert
            Assert.AreEqual(result, token);
        }
    }
}
