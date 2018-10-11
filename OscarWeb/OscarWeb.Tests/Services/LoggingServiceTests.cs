using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Constants;
using OscarWeb.Services;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class LoggingServiceTests
    {
        [TestMethod]
        public void TrackEventTests()
        {
            //Arrange
            TimeSpan timespan = new TimeSpan(1000);
            const string useremail = "unittest@grosvenor-leasing.co.uk";
            const string endpoint = "https://oscarinternal.thegrosvenorgroup.co.uk/";

            //Act
            try
            {
                new LoggingService().TrackEvent(LoggingServiceConstants.UnitTestEvent, timespan);
            }
            catch (Exception ex)
            {
                Assert.Fail($"LoggingService().TrackEvent() failed: {ex.Message}");
            }
            
            var properties = new Dictionary<string, string>
            {
                { "UserEmail", useremail },
                { "WebServicesEndpoint", endpoint }
            };

            //Act
            try
            {
                new LoggingService().TrackEvent(LoggingServiceConstants.UnitTestEvent, timespan, properties);
            }
            catch (Exception ex)
            {
                Assert.Fail($"LoggingService().TrackEvent() failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void TrackTraceTests()
        {
            //Arrange
            const string useremail = "unittest@grosvenor-leasing.co.uk";
            const string endpoint = "https://oscarinternal.thegrosvenorgroup.co.uk/";

            //Act
            try
            {
                new LoggingService().TrackTrace(LoggingServiceConstants.UnitTestTrace);
            }
            catch (Exception ex)
            {
                Assert.Fail($"LoggingService().TrackTrace() failed: {ex.Message}");
            }
            
            var properties = new Dictionary<string, string>
            {
                { "UserEmail", useremail },
                { "WebServicesEndpoint", endpoint }
            };

            //Act
            try
            {
                new LoggingService().TrackTrace(LoggingServiceConstants.UnitTestTrace, properties);
            }
            catch (Exception ex)
            {
                Assert.Fail($"LoggingService().TrackTrace() failed: {ex.Message}");
            }
        }
    }
}
