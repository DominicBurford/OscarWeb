using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;
using OscarWeb.Constants;
using OscarWeb.Controllers;
using OscarWeb.Extensions;

using Common.Models;
using Kendo.Mvc.UI;

namespace OscarWeb.Tests.Controllers
{
    [TestClass]
    public class CompanyGridControllerTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";
        private const string UserEmail = "unittest@grosvenor-leasing.co.uk";
        private static UserModel _user = null;
        private static string _endpoint;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(CompanyGridControllerTests._endpoint))
                {
                    CompanyGridControllerTests._endpoint = CompanyGridControllerTests.Rooturl;
                }
                return CompanyGridControllerTests._endpoint;
            }
            set { CompanyGridControllerTests._endpoint = value; }
        }

        [ClassInitialize]
        public static async Task TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            CompanyGridControllerTests.Endpoint = CompanyGridControllerTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {CompanyGridControllerTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    CompanyGridControllerTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {CompanyGridControllerTests.Endpoint}");

            var user =
                await new UserServices().GetUserDetails(CompanyGridControllerTests.UserEmail,
                    CompanyGridControllerTests.Endpoint);

            Assert.IsNotNull(user);
            Assert.AreEqual(CompanyGridControllerTests.UserEmail, user.Email);

            CompanyGridControllerTests._user = user;
        }

        [TestMethod]
        public async Task CreateGetUpdateDeleteCompanyTests()
        {
            //Arrange
            var mockSession = CompanyGridControllerTests.MockHttpContext();

            //Assert - the session object has been created correctly
            Assert.AreEqual(CompanyGridControllerTests.UserEmail, mockSession.Get<string>(SessionConstants.EmailClaim));
            Assert.AreEqual(CompanyGridControllerTests.Rooturl, mockSession.Get<string>(SessionConstants.WebServicesUrl));
            Assert.AreEqual(CompanyGridControllerTests._user.Id.ToString().Base64Encode(), mockSession.Get<string>(SessionConstants.EncodedUserId));
            
            var controller = new CompanyGridController();

            //Assert
            Assert.IsNotNull(controller);

            controller.MockSession = mockSession;

            DataSourceRequest request = new DataSourceRequest();

            //Act - fetch the companies
            var companies = await controller.EditingPopup_Read(request);

            //Assert
            Assert.IsNotNull(companies);

            //Arrange
            string name = Guid.NewGuid().ToString();
            CompanyModel company = new CompanyModel
            {
                Id = 0,
                Active = true,
                Name = name,
                StorageContainerName = $"{name}container"
            };

            //Act - create the company
            var result = await controller.EditingPopup_Create(request, company);

            //Assert
            Assert.IsNotNull(result);

            //Act - fetch a list of the companies
            var listOfCompanies =
                await new CompanyAdminPageModelService().GetAllCompanies(CompanyGridControllerTests.UserEmail, CompanyGridControllerTests.Rooturl, CompanyGridControllerTests._user.Id.ToString().Base64Encode());

            
            Assert.IsNotNull(listOfCompanies);
            Assert.IsNotNull(listOfCompanies.Companies);
            Assert.IsTrue(listOfCompanies.Companies.Any());

            //Find the newly created company
            var newcompany = listOfCompanies.Companies.Find(c => c.Name == name);

            //Assert
            Assert.IsNotNull(newcompany);
            Assert.AreEqual(name, newcompany.Name);
            
            //Act - update the company
            company.Id = newcompany.Id;
            company.StorageContainerName = $"{company.StorageContainerName}updated";
            result = await controller.EditingPopup_Update(request, company);

            //Assert
            Assert.IsNotNull(result);

            //Act - delete the company
            result = await controller.EditingPopup_Destroy(request, company);

            //Assert
            Assert.IsNotNull(result);
        }

        private static ISession MockHttpContext()
        {
            MockHttpSession httpcontext = new MockHttpSession();
            httpcontext.Set<string>(SessionConstants.EmailClaim, CompanyGridControllerTests.UserEmail);
            httpcontext.Set<string>(SessionConstants.WebServicesUrl, CompanyGridControllerTests.Endpoint);
            httpcontext.Set<string>(SessionConstants.EncodedUserId, CompanyGridControllerTests._user.Id.ToString().Base64Encode());
            httpcontext.Set<UserModel>(SessionConstants.CurrentUser, CompanyGridControllerTests._user);
            return httpcontext;
        }
    }
}
