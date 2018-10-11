using Microsoft.VisualStudio.TestTools.UnitTesting;
using OscarWeb.Extensions;

namespace OscarWeb.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void Base64EncodeDecodeTests()
        {
            //Act
            const string original = "thisstringistobeencoded";
            const string encoded = "dGhpc3N0cmluZ2lzdG9iZWVuY29kZWQ=";

            //Assert
            Assert.AreEqual(encoded, original.Base64Encode());
            Assert.AreEqual(original, encoded.Base64Decode());
            
            //Arrange
            string encodedstring = original.Base64Encode();

            //Assert
            Assert.AreEqual(encoded, encodedstring);

            //Arrange
            string decodedstring = encodedstring.Base64Decode();

            //Assert
            Assert.AreEqual(original, decodedstring);
        }

        [TestMethod]
        public void NormalizeJsonStringTests()
        {
            //Arrange
            string rawstring = @"\this string needs to be normalised\";
            const string normalised = "this string needs to be normalised";

            //Act
            string result = rawstring.NormalizeJsonString();

            //Assert
            Assert.AreEqual(normalised, result);

            rawstring = "this\\ string needs to be \\normalised";
            
            //Act
            result = rawstring.NormalizeJsonString();

            //Assert
            Assert.AreEqual(normalised, result);

            Assert.AreEqual("", "".NormalizeJsonString());

            string nullstring = null;
            Assert.AreEqual(null, nullstring);
        }
    }
}
