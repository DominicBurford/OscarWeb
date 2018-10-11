using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Extensions;
using OscarWeb.Services;

using Common.Models;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class CompanyAdminPageModelServiceTests
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
                if (string.IsNullOrEmpty(CompanyAdminPageModelServiceTests._endpoint))
                {
                    CompanyAdminPageModelServiceTests._endpoint = CompanyAdminPageModelServiceTests.Rooturl;
                }
                return CompanyAdminPageModelServiceTests._endpoint;
            }
            set { CompanyAdminPageModelServiceTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            CompanyAdminPageModelServiceTests.Endpoint = CompanyAdminPageModelServiceTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {CompanyAdminPageModelServiceTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    CompanyAdminPageModelServiceTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {CompanyAdminPageModelServiceTests.Endpoint}");
        }


        [TestMethod]
        public async Task GetAllCompaniesTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - get the list of companies
            var response = await new CompanyAdminPageModelService().GetAllCompanies(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Companies);
            Assert.IsTrue(response.Companies.Any());
            Assert.IsTrue(response.Companies.Exists(c => c.Name == "OSCAR"));
        }

        [TestMethod]
        public async Task GetCompanyByIdTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - get the list of companies
            var response = await new CompanyAdminPageModelService().GetCompanyById(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, user.CompanyId);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(user.CompanyId, response.Id);
        }

        [TestMethod]
        public async Task GetCompanyAddressesTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - get the list of companies
            var response = await new CompanyAdminPageModelService().GetCompanyAddresses(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, user.CompanyId, false);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Addresses);
            Assert.IsTrue(response.Addresses.Any());
            Assert.IsTrue(response.Addresses.Count == 1);

            response = await new CompanyAdminPageModelService().GetCompanyAddresses(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, user.CompanyId, true);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Addresses);
            Assert.IsTrue(response.Addresses.Any());
            Assert.IsTrue(response.Addresses.Count == 2);
        }

        [TestMethod]
        public async Task GetCompanyAddressTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act - get company
            var response = await new CompanyAdminPageModelService().GetCompanyAddress(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, 1);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("Unit Test House", response.AddressLine1);
        }

        [TestMethod]
        public async Task GetCompanyAddressInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act 
            var response = await new CompanyAdminPageModelService().GetCompanyAddress(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, 0);

            //Assert
            Assert.IsNull(response);

            //Act 
            response = await new CompanyAdminPageModelService().GetCompanyAddress(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, -1);

            //Assert
            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task InsertUpdateGetDeleteCompanyAddressTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            CompanyAddressModel address = new CompanyAddressModel
            {
                Id = 0,
                CompanyId = user.CompanyId,
                Postcode = "NN15 6XU",
                AddressLine1 = "Unit Test House OscarWeb",
                AddressLine2 = "Kettering Business Venture Park",
                AddressLine3 = "Kettering",
                AddressLine4 = "Northants",
                AddressLine5 = "",
                Primary = false,
                Active = true
            };

            //Act - create the company address
            var response = await new CompanyAdminPageModelService().CreateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(response);

            var addresses = await new CompanyAdminPageModelService().GetCompanyAddresses(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, user.CompanyId, true);

            //Assert
            Assert.IsNotNull(addresses);
            Assert.IsNotNull(addresses.Addresses);
            Assert.IsTrue(addresses.Addresses.Any());

            //find the newly added company address
            var foundAddress = addresses.Addresses.Find(c => c.AddressLine1 == "Unit Test House OscarWeb");

            //Assert  
            Assert.IsNotNull(foundAddress);
            Assert.IsTrue(foundAddress.Id > 0);
            Assert.AreEqual(address.AddressLine1, foundAddress.AddressLine1);

            //Arrange 
            int identifier = foundAddress.Id;
            address.Id = identifier;
            address.AddressLine1 = $"{address.AddressLine1}_updated";

            //Act - update the company address
            response = await new CompanyAdminPageModelService().UpdateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(response);

            addresses = await new CompanyAdminPageModelService().GetCompanyAddresses(user.Email, CompanyAdminPageModelServiceTests.Rooturl, encodedId, user.CompanyId, true);

            //Assert
            Assert.IsNotNull(addresses);
            Assert.IsNotNull(addresses.Addresses);
            Assert.IsTrue(addresses.Addresses.Any());

            //find the updated company address
            foundAddress = addresses.Addresses.Find(c => c.AddressLine1 == "Unit Test House OscarWeb_updated");

            //Assert
            Assert.IsNotNull(foundAddress);
            Assert.IsTrue(foundAddress.Id > 0);
            Assert.AreEqual(address.AddressLine1, foundAddress.AddressLine1);

            //Arrange
            response = await new CompanyAdminPageModelService().DeleteCompanyAddress(identifier, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task CreateCompanyAddressInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            CompanyAddressModel address = new CompanyAddressModel
            {
                Id = 0,
                CompanyId = user.CompanyId,
                Postcode = "NN15 6XU",
                AddressLine1 = "Unit Test House OscarWeb",
                AddressLine2 = "Kettering Business Venture Park",
                AddressLine3 = "Kettering",
                AddressLine4 = "Northants",
                AddressLine5 = "",
                Primary = false,
                Active = true
            };

            var service = new CompanyAdminPageModelService();

            //Act
            var response = await service.CreateCompanyAddress(null, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "";

            //Act
            response = await service.CreateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "NN15 6XU";
            address.AddressLine1 = "";

            //Act
            response = await service.CreateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "NN15 6XU";
            address.AddressLine1 = "Unit Test House OscarWeb";

            response = await service.CreateCompanyAddress(address, null, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            response = await service.CreateCompanyAddress(address, "", CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task UpdateCompanyAddressInvalidTests()
        {
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            CompanyAddressModel address = new CompanyAddressModel
            {
                Id = 0,
                CompanyId = user.CompanyId,
                Postcode = "NN15 6XU",
                AddressLine1 = "Unit Test House OscarWeb",
                AddressLine2 = "Kettering Business Venture Park",
                AddressLine3 = "Kettering",
                AddressLine4 = "Northants",
                AddressLine5 = "",
                Primary = false,
                Active = true
            };

            var service = new CompanyAdminPageModelService();

            //Act
            var response = await service.UpdateCompanyAddress(null, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "";

            //Act
            response = await service.UpdateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "NN15 6XU";
            address.AddressLine1 = "";

            //Act
            response = await service.UpdateCompanyAddress(address, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            address.Postcode = "NN15 6XU";
            address.AddressLine1 = "Unit Test House OscarWeb";

            response = await service.UpdateCompanyAddress(address, null, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            response = await service.UpdateCompanyAddress(address, "", CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);
        }

        [TestMethod]
        public async Task DeleteCompanyAddressInvalidTests()
        {
            //Arrange
            //Act - firstly get the user object as we need the user's ID
            var user = await new UserServices().GetUserDetails(CompanyAdminPageModelServiceTests.UserEmail, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(UserEmail, user.Email);

            //Arrange
            int userId = user.Id;
            string encodedId = userId.ToString().Base64Encode();

            //Act
            var response = await new CompanyAdminPageModelService().DeleteCompanyAddress(0, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            //Act
            response = await new CompanyAdminPageModelService().DeleteCompanyAddress(-1, encodedId, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            //Act
            response = await new CompanyAdminPageModelService().DeleteCompanyAddress(1, null, CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            //Act
            response = await new CompanyAdminPageModelService().DeleteCompanyAddress(1, "", CompanyAdminPageModelServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);
        }
    }
}
