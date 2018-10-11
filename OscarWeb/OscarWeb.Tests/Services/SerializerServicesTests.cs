using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OscarWeb.Services;

namespace OscarWeb.Tests.Services
{
    [TestClass]
    public class SerializerServicesTests
    {
        [TestMethod]
        public void SerialiseDeserialiseTests()
        {
            //Act
            TestClass testclass = new TestClass
            {
                StringProperty = "this is a string",
                IntProperty = 1,
                DatetimeProperty = new DateTime(2018, 1, 1)
            };

            //Arrange
            SerializerServices serializer = new SerializerServices();
            string serialised = serializer.SerializeObject(testclass);

            //Assert
            Assert.IsNotNull(serialised);
            Assert.IsTrue(serialised.Length > 0);

            var deSerialised = serializer.DeserializeObject<TestClass>(serialised);

            //Assert
            Assert.IsNotNull(deSerialised);
            Assert.AreEqual(testclass.StringProperty, deSerialised.StringProperty);
            Assert.AreEqual(testclass.IntProperty, deSerialised.IntProperty);
            Assert.AreEqual(testclass.DatetimeProperty, deSerialised.DatetimeProperty);
        }

        [TestMethod]
        public void DeserialiseDateTests()
        {
            //Act
            DateTime testdate = new DateTime(2018, 1, 20);

            //Arrange
            string jsonDate = new SerializerServices().SerializeObject(testdate);
            DateTime dt = new SerializerServices().DeserializeObject<DateTime>(jsonDate);

            //Assert
            Assert.AreEqual(testdate, dt);
        }
    }

    class TestClass
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public DateTime DatetimeProperty { get; set; }
    }
}
