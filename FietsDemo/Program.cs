using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avans.TI.BLE;

namespace FietsDemo
{
    class Program
    {

        private GUI gui;
        private Simulator simulator;

        private double accumulatedPower;
        private int accumulatedPowerCounter = 0;
        private double previousAccumulatedPower;


        private double distanceTraveledInKM;
        private int distanceTraveledCounter = 0;
        private double previousDistanceTraveled;

        private double timeElapsedInSeconds;
        private int previousTimeElapsed;
        private int timeElapsedCounter = 0;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.start();


        }

        public void start()
        {
            Thread thread = new Thread(startGUI);
            thread.Start();

            Thread thread2 = new Thread(startSimulator);
            thread2.Start();

            initialize();

        }

        public void startSimulator()
        {
            this.simulator = new Simulator();
            this.simulator.run();
        }

        public void startGUI()
        {
            this.gui = new GUI();
            this.gui.run();
        }

        public async Task initialize()
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
            errorCode = await bleBike.OpenDevice("Avans Bike A918");
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
            errorCode = await bleHeart.OpenDevice("Avans Bike A918");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");



            Console.Read();
        }

        public void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
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

                Console.WriteLine("{0}: {1}", name, data.ToString());
                String heartrateString = data.ToString();
                setValuesInGui("heartrate", 0, heartrateString);


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

                        // Capabilities, LAP and FEtype merged byte
                        int capabilitiesAndFeType = bytes[startingByteMessage + 7];

                        // LAP
                        int LAP = capabilitiesAndFeType >> 7;

                        // FEtype
                        int FEType = (capabilitiesAndFeType << 1) >> 5;

                        // CAPABILITIES: 0 or 1
                        bool capabilities = (capabilitiesAndFeType & (1 << 2)) != 0;

                        // Total speed value
                        double speed = ((leastSignificantBit + (mostSignificantBit << 8)) / 1000.0) * 3.6;


                        // Calculation time elapsed
                        if (previousTimeElapsed > elapsedTime)
                            timeElapsedCounter++;

                        this.timeElapsedInSeconds = ((64 * timeElapsedCounter) + elapsedTime * 0.25);
                        this.previousTimeElapsed = elapsedTime;
                                              
                        setValuesInGui("elapsedTime", timeElapsedInSeconds, "");

                        // Calculation distance traveled
                        if (previousDistanceTraveled > distanceTraveled)
                            distanceTraveledCounter++;

                        this.distanceTraveledInKM = ((256 * distanceTraveledCounter) + distanceTraveled) / 1000;
                        this.previousDistanceTraveled = distanceTraveled;


                        setValuesInGui("DT", this.distanceTraveledInKM, "");
                        setValuesInGui("speed", speed, "");

                        Console.WriteLine("{0}: \t distance traveled: {1}", name, this.distanceTraveledInKM);
                    }
                    else if (bytes[startingByteMessage] == 0x19)
                    {
                        // Specific Trainer/Stationary Bike Data 


                        // Update Event Count
                        int eventCount = bytes[startingByteMessage + 1];


                        // Instantaneous Cadence (ratations per minute)
                        int instantaneousCadence = bytes[startingByteMessage + 2];


                        // Accumulated Power LSB
                        int accumalatedPowerLSB = bytes[startingByteMessage + 3];


                        // Accumulated Power MSB
                        int accumelatedPowerMSB = bytes[startingByteMessage + 4];


                        // Instantaneous Power LSB 
                        int instantaneousPowerLSB = bytes[startingByteMessage + 5];


                        // Instantaneous Power MSN
                        int instantaneousPowerMSN = 0;
                        {
                            byte tempByte = bytes[startingByteMessage + 6];
                            int bytesAmount = 4;
                            for (int bitNumber = 0; bitNumber < bytesAmount; bitNumber++)
                            {
                                Boolean bit = (tempByte & (1 << bitNumber - 1)) != 0;

                                instantaneousPowerMSN <<= 1;

                                if (bit)
                                {
                                    instantaneousPowerMSN++;
                                }
                            }
                        }


                        // Trainer Status Bit Field
                        bool needsBicyclePowerCalibration;
                        bool needsResistanceCalibration;
                        bool needsUserConfiguration;
                        {
                            byte tempByte = bytes[startingByteMessage + 6];


                            needsBicyclePowerCalibration = (tempByte & (1 << 4)) != 0;
                            needsResistanceCalibration = (tempByte & (1 << 5)) != 0;
                            needsUserConfiguration = (tempByte & (1 << 6)) != 0;
                            bool reservedForFuture = (tempByte & (1 << 7)) != 0;
                        }


                        // Flags Bit Field 
                        // Target Power Limits
                        // 0 – Trainer operating at the target power, or no target power set.
                        // 1 – User’s cycling speed is too low to achieve target power.
                        // 2 – User’s cycling speed is too high to achieve target power.
                        // 3 – Undetermined (maximum or minimum) target power limit reached.
                        int targetPowerLimits = 0;
                        {
                            byte tempByte = bytes[startingByteMessage + 7];


                            if ((tempByte & (1 << 1)) != 0)
                            {
                                targetPowerLimits++;
                                targetPowerLimits <<= 1;
                            }
                            if ((tempByte & (1 << 1)) != 0)
                            {
                                targetPowerLimits++;
                            }

                            bool reservedForFuture = (tempByte & (1 << 2)) != 0;
                            bool reservedForFuture2 = (tempByte & (1 << 3)) != 0;
                        }


                        // FE State Bit Field
                        // 0 - Reserved
                        // 1 - ASLEEP (OFF)
                        // 2 - READY
                        // 3 - IN_USE
                        // 4 - FINISHED (PAUSED)
                        // 5-7 - Reserved. Do not send or interpret
                        int feState = 0;

                        // A change in value of the lap toggle bit indicates a lap event
                        Boolean lapToggleBit;
                        {
                            byte tempByte = bytes[startingByteMessage + 7];

                            int bytesAmount = 3;
                            for (int bitNumber = 4; bitNumber < bytesAmount + 4; bitNumber++)
                            {
                                Boolean bit = (tempByte & (1 << bitNumber - 1)) != 0;

                                feState <<= 1;

                                if (bit)
                                {
                                    feState++;
                                }
                            }

                            lapToggleBit = (tempByte & (1 << 7)) != 0;
                        }


                        // Acumelated power calculation
                        double accumulatedPower = (accumalatedPowerLSB + (accumelatedPowerMSB << 8)) / 1000.0;
                        if (this.previousAccumulatedPower > accumulatedPower)
                            this.accumulatedPowerCounter++;

                        this.accumulatedPower = (65536 * accumulatedPowerCounter) + accumulatedPower;
                        this.previousAccumulatedPower = accumulatedPower;

                        // Instantaneous power calculation
                        double instantaneousPower = (instantaneousPowerLSB + (instantaneousPowerMSN << 8));

                        setValuesInGui("AP", this.accumulatedPower, "");



                        Console.WriteLine("{0}: \t acumelated power: {1} \t rpm: {2} \t instantaneous power: {3} \t state: {4}", name, accumulatedPower, instantaneousCadence, instantaneousPowerMSN, feState);
                    }

                }

            }
        }

        public void setValuesInGui(String valueType, double value, String heartrate)
        {
            switch (valueType)
            {
                case "speed":
                    this.gui.getForm().setSpeed(value);
                    break;
                case "heartrate":
                    this.gui.getForm().setHeartrate(heartrate);
                    break;
                case "AP":
                    this.gui.getForm().setAP(value);
                    break;
                case "DT":
                    this.gui.getForm().setDT(value);
                    break;
                case "elapsedTime":
                    this.gui.getForm().setElapsedTime(value);
                    break;
                    
            }

        }
    }

}

