using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avans.TI.BLE;
using SharedItems;
using TCP_naar_VR;
using UpdateType = SharedItems.UpdateType;

namespace FietsDemo
{
    public class BluetoothBike : IBLEcallBack
    {

        public GUI gui { get; set; }
        private Simulator simulator;
        private BikeSimulator bikeSimulator;

        private BLE BleBike;
        private BLE HeartRateSensor;

        private double accumulatedPower;
        private int accumulatedPowerCounter = 0;
        private double previousAccumulatedPower;


        private double distanceTraveledInKM;
        private int distanceTraveledCounter = 0;
        private double previousDistanceTraveled;

        private double timeElapsedInSeconds;
        private int previousTimeElapsed;
        private int timeElapsedCounter = 0;

        private double resistance = 0;

        private UserClient client;

        private TcpClientVR tcpClientVR;

        private Random random;

        static void Main(string[] args)
        {
            BluetoothBike program = new BluetoothBike();
            program.start();
        }


        public void start()
        {
            this.random = new Random();

            Thread guiThread = new Thread(startGUI);
            guiThread.Start();

            Thread clientThread = new Thread(startClient);
            clientThread.Start();

            Thread VRThread = new Thread(startVR);
            VRThread.Start();
            
            initialize();
        }

        public void startClient()
        {
            this.client = new UserClient();

        }


        public void startVR()
        {
            tcpClientVR = new TcpClientVR("145.48.6.10", 6666);
            tcpClientVR.SendKickOff();
        }

        public void startSimulator()
        {
            // When starting the simulator, the first thing to do is to unsubscribe from the real BLE device, otherwise they would interfere.
            this.BleBike.SubscriptionValueChanged -= BleBike_SubscriptionValueChanged;
            this.HeartRateSensor.SubscriptionValueChanged -= BleBike_SubscriptionValueChanged;
            
            // The second step is to start the simulator and the GUI that comes with it.
            Console.WriteLine("starting simulator...");
            bikeSimulator = new BikeSimulator(this);

            Thread thread = new Thread(startSimulatorGUI);
            thread.Start();
            
        }

        private void startSimulatorGUI()
        {
            this.simulator = new Simulator(this.bikeSimulator);
            this.simulator.run();
        }

        public void stopSimulator() 
        {
            // To stop the simulator we first stop the simulator thread and the GUI.
            Console.WriteLine("Stopping Simulator...");
            this.bikeSimulator.running = false;
            // After that we subscribe to the BLE service again to continue measuring.
            this.BleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            this.HeartRateSensor.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
        }

        public void startGUI()
        {
            this.gui = new GUI(this);
            this.gui.run();
        }

        public async Task initialize()
        {
            int errorCode = 0;
            BleBike = new BLE();
            HeartRateSensor = new BLE();

            // We need some time to list available devices
            Thread.Sleep(1000);

            // List available devices
            List<String> bleBikeList = BleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = await BleBike.OpenDevice("Avans Bike");
            // __TODO__ Error check

            var services = BleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service}");
            }

            //Console.WriteLine("starting sim");
            //bikeSimulator = new BikeSimulator(this);
            //Thread thread = new Thread(startSimulator);
           // thread.Start();

            // Set service
            errorCode = await BleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            // __TODO__ error check

            // Subscribe
            BleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await BleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

            // Heart rate
            errorCode = await HeartRateSensor.OpenDevice("Avans Bike");

            await HeartRateSensor.SetService("HeartRate");

            HeartRateSensor.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            await HeartRateSensor.SubscribeToCharacteristic("HeartRateMeasurement");



            Console.Read();
        }

        public void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            string name = e.ServiceName;

            if (name == "00002a37-0000-1000-8000-00805f9b34fb" || name == "SimulatorHeartRate")
            {
                
                setValuesInGui(UpdateType.Heartrate, e.Data[1]);


            }
            else if (name == "6e40fec2-b5a3-f393-e0a9-e50e24dcca9e" || name == "Simulator")
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
                                              
                        setValuesInGui(UpdateType.ElapsedTime, timeElapsedInSeconds);

                        // Calculation distance traveled
                        if (previousDistanceTraveled > distanceTraveled)
                            distanceTraveledCounter++;

                        this.distanceTraveledInKM = ((256 * distanceTraveledCounter) + distanceTraveled) / 1000.0;
                        this.previousDistanceTraveled = distanceTraveled;

                        setValuesInGui(UpdateType.AccumulatedDistance, this.distanceTraveledInKM);
                        setValuesInGui(UpdateType.Speed, speed);

                        //resistance = 30f;
                        //setValuesInGui("resistance", setResistance(resistance));

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

                        setValuesInGui(UpdateType.AccumulatedPower, this.accumulatedPower);


                    
                        Console.WriteLine("{0}: \t acumelated power: {1} \t rpm: {2} \t instantaneous power: {3} \t state: {4}", name, accumulatedPower, instantaneousCadence, instantaneousPowerMSN, feState);
                    }

                }

            }
        }

        public void setValuesInGui(UpdateType valueType, double value)
        {
            this.client.sendUpdatedValues(valueType,value);

            switch (valueType)
            {
                case UpdateType.Speed:
                    this.gui.getForm().setSpeed(value);
                    this.tcpClientVR.speed = value;
                    break;
                case UpdateType.Heartrate:
                    this.gui.getForm().setHeartrate(value);
                    //TODO heartrate is one word
                    this.tcpClientVR.heartRate = value;
                    break;
                case UpdateType.AccumulatedPower:
                    this.gui.getForm().setAP(value);
                    this.tcpClientVR.AP = value;
                    break;
                case UpdateType.AccumulatedDistance:
                    this.gui.getForm().setDT(value);
                    this.tcpClientVR.DT = value;
                    break;
                case UpdateType.ElapsedTime:
                    this.gui.getForm().setElapsedTime(value);
                    this.tcpClientVR.elapsedTime = value;
                    break;
                case UpdateType.Resistance:
                    this.gui.getForm().setResistance(value);
                    this.tcpClientVR.resistance = value;
                    break;

            }

        }

        public double setResistance(double percentage)
        {

            if(this.simulator != null)
            {
                this.simulator.setResistance((int)percentage);
            }
            
            if (percentage <= 100.0 && percentage >= 0.0)
            {
                Byte[] byteArray = new byte[13];
                byteArray[0] = 0x4A;
                byteArray[1] = 0x09;
                byteArray[2] = 0x4E;
                byteArray[3] = 0x05;
                byteArray[4] = 0x30;
                byteArray[5] = 0xFF;
                byteArray[6] = 0xFF;
                byteArray[7] = 0xFF;
                byteArray[8] = 0xFF;
                byteArray[9] = 0xFF;
                byteArray[10] = 0xFF;
                byteArray[11] = (byte)(percentage*2);

                int checksumCalculated = byteArray[0];
                for (int i = 1; i < byteArray.Length - 1; i++)
                {
                    checksumCalculated = byteArray[i] ^ checksumCalculated;
                }

                byteArray[12] = (byte)checksumCalculated;

                BleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", byteArray);
                return percentage;
            }
            else
            {
                throw new System.InvalidOperationException("Parameter outside of acceptable bounds (0-200)");
            }

        }

        //public void setResistance(float percentage)
        //{
        //    //byte[] byteArray = { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(percentage * 2), 0 };
        //    byte[] byteArray = { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 40, 0 };


        //    int checksumCalculated = 0;
        //    for (int i = 0; i < byteArray.Length - 1; i++)
        //    {
        //        checksumCalculated = byteArray[i] ^ checksumCalculated;
        //    }

        //    byteArray[byteArray.Length - 1] = (byte)checksumCalculated;

        //    BleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", byteArray);
        //    //bikeSimulator.WriteCharacteristic("Simulator", byteArray);
        //}
    }
}

