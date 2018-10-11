using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;

using Common.Models.EmailRequests;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class SendEmailServiceTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";

        private static string _endpoint;

        /// <summary>
        /// Gets and sets the endpoint of the deployed web services
        /// </summary>
        public static string Endpoint
        {
            get
            {
                if (string.IsNullOrEmpty(SendEmailServiceTests._endpoint))
                {
                    SendEmailServiceTests._endpoint = SendEmailServiceTests.Rooturl;
                }
                return SendEmailServiceTests._endpoint;
            }
            set { SendEmailServiceTests._endpoint = value; }
        }

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            Console.WriteLine($"TestClassinitialize - {context.TestName}");

            //Default the endpoint to the internal testing endpoint so the tests can be run in the Visual Studio IDE.
            //When run during the deployments to STAGING and PRODUCTION we pass in deployment endpoints at runtime from TFS.
            SendEmailServiceTests.Endpoint = SendEmailServiceTests.Rooturl;
            Console.WriteLine($"Default deployment endpoint {SendEmailServiceTests.Endpoint}");

            if (context.Properties.ContainsKey("webAppUrl"))
            {
                string endpoint = context.Properties["webAppUrl"].ToString();
                if (!string.IsNullOrEmpty(endpoint))
                {
                    SendEmailServiceTests.Endpoint = context.Properties["webAppUrl"].ToString();
                    Console.WriteLine($"Using deployed endpoint passed via parameter from TFS {endpoint} ");
                }
            }
            Console.WriteLine($"Running unit tests against deployed endpoint {SendEmailServiceTests.Endpoint}");
        }

        [TestMethod]
        public async Task SendDocumentNotificationTests()
        {
            //Arrange
            var sendList = new List<string>
            {
                "dominic.burford@grosvenor-leasing.co.uk",
                "geoff.hall@grosvenor-leasing.co.uk"
            };
            DocumentSubscriberNotificationModel notification =
                new DocumentSubscriberNotificationModel
                {
                    DocumentName = "Unit test document.txt",
                    UploaderName = "MR UNIT TEST",
                    UserName = "TEST RECIPIENT",
                    EmailRecipient = sendList
                };

            //Act
            var response =
                await new SendEmailService().SendDocumentNotification(notification, SendEmailServiceTests.Rooturl);

            //Assert
            Assert.IsTrue(response);
        }

        [TestMethod]
        public async Task SendDocumentNotificationInvalidTests()
        {
            //Arrange
            var sendList = new List<string>
            {
                "dominic.burford@grosvenor-leasing.co.uk",
                "geoff.hall@grosvenor-leasing.co.uk"
            };
            DocumentSubscriberNotificationModel notification =
                new DocumentSubscriberNotificationModel
                {
                    DocumentName = "Unit test document.txt",
                    UploaderName = "MR UNIT TEST",
                    UserName = "TEST RECIPIENT",
                    EmailRecipient = sendList
                };

            //Act
            var response =
                await new SendEmailService().SendDocumentNotification(null, SendEmailServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            notification.UserName = null;

            //Act
            response =
                await new SendEmailService().SendDocumentNotification(null, SendEmailServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);

            notification.UserName = "TEST RECIPIENT";
            notification.DocumentName = null;

            //Act
            response =
                await new SendEmailService().SendDocumentNotification(null, SendEmailServiceTests.Rooturl);

            //Assert
            Assert.IsFalse(response);
        }
    }
}
