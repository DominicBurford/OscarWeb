using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class UserServicesTests
    {
        private const string Rooturl = "http://192.168.0.16:8080/";

        [TestMethod]
        public async Task GetUserDetailsTests()
        {
            //Arrange
            const string useremail = "unittest@grosvenor-leasing.co.uk";

            //Act
            var user = await new UserServices().GetUserDetails(useremail, Rooturl);

            //Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(useremail, user.Email);
            Assert.IsNotNull(user.Roles);
            Assert.IsTrue(user.Roles.Any());
            Assert.IsNotNull(user.Roles.Find(r => r.RoleName == "UNITTEST ADMIN ROLE"));
        }

        [TestMethod]
        public async Task GetUserDetailsInvalidTests()
        {
            //Arrange
            const string useremail = "unittest@grosvenor-leasing.co.uk";

            //Act
            var user = await new UserServices().GetUserDetails("", Rooturl);

            //Assert
            Assert.IsNull(user);

            //Act
            user = await new UserServices().GetUserDetails(null, Rooturl);

            //Assert
            Assert.IsNull(user);

            //Act
            user = await new UserServices().GetUserDetails(useremail, "");

            //Assert
            Assert.IsNull(user);

            //Act
            user = await new UserServices().GetUserDetails(useremail, null);

            //Assert
            Assert.IsNull(user);
        }
    }
}
