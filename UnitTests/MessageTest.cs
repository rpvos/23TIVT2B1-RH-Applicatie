using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void TestCheckSum()
        {
            //Arrange
            dynamic dynamicJson = new
            {
                Data = new
                {
                    Test = "This is a test!"
                }
            };
            //Act
            //Add checksum method
            JObject json = JObject.Parse(JsonConvert.SerializeObject(dynamicJson));
            byte checksum1 = 0;
            byte[] data1 = Encoding.ASCII.GetBytes(((JObject)json["Data"]).ToString());
            foreach (byte b in data1)
            {
                checksum1 ^= b;
            }
            json["Checksum"] = checksum1;
            //Check checksum method
            byte checksum2 = (byte)json["Checksum"];
            JObject jObject = (JObject)json["Data"];
            byte[] data2 = Encoding.ASCII.GetBytes(jObject.ToString());
            foreach (byte b in data2)
                checksum2 ^= b;
            //Assert
            Assert.IsTrue(checksum2 == 0, "Checksum calculation is wrong");
        }
    }
}
