using FietsDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class BleTest
    {
        [TestMethod]
        public void TestBLEParam()
        {
            //Arrange
            BluetoothBike bleBike = new BluetoothBike();
            //Act
            bleBike.BleBike_SubscriptionValueChanged(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException),
        "An invalid resistance value was incorrectly handled.")]
        public void TestBleResistanceParam1()
        {
            //Arrange
            BluetoothBike bleBike = new BluetoothBike();
            //Act
            bleBike.setResistance(100.1);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException),
        "An invalid resistance value was incorrectly handled.")]
        public void TestBleResistanceParam2()
        {
            //Arrange
            BluetoothBike bleBike = new BluetoothBike();
            //Act
            bleBike.setResistance(-0.1);
        }
    }
}
