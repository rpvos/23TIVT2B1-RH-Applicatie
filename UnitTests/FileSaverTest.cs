using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using System;

namespace UnitTests
{
    [TestClass]
    public class FileSaverTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
        "An invalid foldername value was incorrectly handled.")]
        public void TestFolderNameExceptions()
        {
            //Arragement
            char[] BLACKLIST = new char[] { '/', '\\', '<', '>', ':', '"', '|', '?', '*' };
            //Act
            foreach (char c in BLACKLIST)
            {
                new CryptoFileSaver("fileName" + c);
            }
        }
    }
}
