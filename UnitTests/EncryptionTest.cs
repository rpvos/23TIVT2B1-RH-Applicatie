using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void TestEncryption()
        {
            //Arrange
            EncyptionService encyptionService = new EncyptionService();
            string text = "I am a test!";
            //Act
            byte[] cypher = encyptionService.EncryptStringToBytes(text);
            string decryptedText = encyptionService.DecryptStringFromBytes(cypher).Substring(0, text.Length);
            //Assert
            Debug.WriteLine(decryptedText);
            Assert.IsTrue(decryptedText == text, $"{text} -> Does noet equal -> {decryptedText}");
        }
        [TestMethod]
        public void TestBlockSizeCalculation()
        {
            //Arrange
            int textValue = 155;
            List<byte> totalBuffer = new List<byte>(BitConverter.GetBytes(textValue));
            //Act
            byte[] lengthArray = totalBuffer.GetRange(0, 4).ToArray();
            int length = BitConverter.ToInt32(lengthArray, 0);
            int blockSize = length + (16 - length % 16);
            //Assert
            Assert.IsTrue(blockSize == 160, "Block size calculation does not work");
        }
    }
}
