using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace FietsDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int errorCode = 0;
            BLE bleBike = new BLE();
            BLE bleHeart = new BLE();

            // We need some time to list available devices
            Thread.Sleep(1000);

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = await bleBike.OpenDevice("Avans Bike");
            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            // __TODO__ error check

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

            // Heart rate
            errorCode = await bleHeart.OpenDevice("Avans Bike");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");


            Console.Read();
        }

        private static void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            String name = e.ServiceName;

            if (name == "00002a37-0000-1000-8000-00805f9b34fb")
            {
                StringBuilder data = new StringBuilder();

                name = "HeartRate";
                foreach (byte b in e.Data)
                {
                    data.Append(b);
                    data.Append(" ");
                }

                //Console.WriteLine("{0}: {1}", name, data.ToString());
            }
            else if (name == "6e40fec2-b5a3-f393-e0a9-e50e24dcca9e")
            {
                name = "Avans Bike";

                byte[] bytes = e.Data;
                // Starting byte should always be 0xA4
                if (bytes[0] == 0xA4)
                {
                    // Length of the message
                    int length = bytes[1];
                    // Type of message
                    int type = bytes[2];


                    // Checking if the checksum is correct
                    int checksum = bytes[length + 3];
                    int checksumCalculated = bytes[0];
                    for (int i = 1; i < bytes.Length - 1; i++)
                    {
                        checksumCalculated = bytes[i] ^ checksumCalculated;
                    }

                    // If it does not match the message is corrupted so we leave this message alone
                    if (checksum != checksumCalculated)
                    {
                        Console.WriteLine("Error wrong message construction");
                        return;
                    }


                    // Get the channel id from the message
                    int channelID = bytes[3];

                    // Starting byte of the message
                    int startingByteMessage = 4;

                    // Check wich command it is
                    if (bytes[startingByteMessage] == 0x10)
                    {
                        // General fe datapage

                        // Equipment Type Bit Field
                        int equipmentTypeBit = bytes[startingByteMessage + 1];

                        // Elapsed Time
                        int elapsedTime = bytes[startingByteMessage + 2];

                        // Distance Traveled
                        int distanceTraveled = bytes[startingByteMessage + 3];

                        // Speed LSB
                        int leastSignificantBit = bytes[startingByteMessage + 4];

                        // Speed MSB
                        int mostSignificantBit = bytes[startingByteMessage + 5];

                        // Heart Rate
                        int heartRateFromBike = bytes[startingByteMessage + 6];

                        // Capabilities Bit Field (4 bits) and FE State Bit Field (4 bits)
                        // __TODO__ make an seperator

                        double speed = (leastSignificantBit + (mostSignificantBit << 8)) / 1000.0;



                        //Console.WriteLine("{0}: \t speed: {1}", name, speed);
                    }
                    else if (bytes[startingByteMessage] == 0x19)
                    {
                        // Specific Trainer/Stationary Bike Data 

                        // Update Event Count
                        int eventCount = bytes[startingByteMessage+1];

                        // Instantaneous Cadence
                        int instantaneousCadence = bytes[startingByteMessage + 2];

                        // Accumulated Power LSB
                        int leastSignificantBit = bytes[startingByteMessage + 3];

                        // Accumulated Power MSB
                        int mostSignificantBit = bytes[startingByteMessage + 4];

                        // Instantaneous Power LSB 
                        int instantaneousPowerLSB = bytes[startingByteMessage + 5];

                        // Instantaneous Power MSN
                        int instantaneousPowerMSN = bytes[startingByteMessage + 6];

                        // Trainer Status Bit Field
                        int trainerStatusBitField = bytes[startingByteMessage + 7];

                        // __TODO__ seperate the byte


                        // Acumelated power calculation
                        double acumelatedPower = (leastSignificantBit + (mostSignificantBit << 8)) / 1000.0;



                        Console.WriteLine("{0}: \t acumelated power: {1}", name, acumelatedPower);
                    }

                }

            }
        }
    }

}

