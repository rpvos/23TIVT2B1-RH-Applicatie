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
            StringBuilder data = new StringBuilder();

            if (name == "00002a37-0000-1000-8000-00805f9b34fb")
            {
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
                //starting byte should always be 0xA4
                if (bytes[0] == 0xA4)
                {
                    //length of the message
                    int length = bytes[1];
                    //type of message
                    int type = bytes[2];


                    //checking if the checksum is correct
                    int checksum = bytes[length + 3];
                    int checksumCalculated = bytes[0];
                    for (int i = 1; i < bytes.Length - 1; i++)
                    {
                        checksumCalculated = bytes[i] ^ checksumCalculated;
                    }

                    //if it does not match the message is corrupted so we leave this message alone
                    if (checksum != checksumCalculated)
                    {
                        Console.WriteLine("Error wrong message construction");
                        return;
                    }


                    //Console.WriteLine($"length = {length} \t type = {type}");
                    for (int i = 0; i < length; i++)
                    {
                        data.Append(bytes[i + 3]);
                        data.Append(" ");
                    }

                    Console.WriteLine("{0}: {1}", name, data.ToString());
                }
            }
        }

    }
}
