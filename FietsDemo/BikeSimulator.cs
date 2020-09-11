using Avans.TI.BLE;
using System;
using System.Threading;

namespace FietsDemo
{
    public class BikeSimulator
    {
        public bool running;

        private float Resistance;

        private IBLEcallBack IBLEcallBack;
        private Page0x10Message SendingPage0x10Message;
        private Page0x19Message SendingPage0x19Message;
        private HeartRateMessage SendingHeartRateMessage;
        public BikeSimulator(IBLEcallBack bLEcallBack)
        {
            IBLEcallBack = bLEcallBack;
            SendingPage0x10Message = new Page0x10Message();
            SendingPage0x19Message = new Page0x19Message();
            SendingHeartRateMessage = new HeartRateMessage();
            running = true;
            Thread backgroundSender = new Thread(new ThreadStart(update));
            backgroundSender.Start();
        }

        public void setSpeed(byte speed)
        {
            SendingPage0x10Message.Speed = speed;
        }

        public void setInstantaneousPower(byte power)
        {
            SendingPage0x19Message.InstantaneousPower = power;
        }

        public void setHeartRate(byte heartrate)
        {
            SendingHeartRateMessage.HeartRate = heartrate;
        }

        private void update()
        {
            bool page = false;
            while (running)
            {
                BLESubscriptionValueChangedEventArgs args = null;

                if (page)
                {
                    args = new BLESubscriptionValueChangedEventArgs
                    {
                        Data = SendingPage0x10Message.getData(),
                        ServiceName = "Simulator"
                    };
                    updateTimeAndDistance();

                    page = false;
                }
                else
                {
                    args = new BLESubscriptionValueChangedEventArgs
                    {
                        Data = SendingPage0x19Message.getData(),
                        ServiceName = "Simulator"
                    };
                    updateEventCount();

                    page = true;
                }
                BLESubscriptionValueChangedEventArgs argsHRSensor = new BLESubscriptionValueChangedEventArgs
                {
                    Data = SendingHeartRateMessage.getData(),
                    ServiceName = "SimulatorHeartRate"
                };

                IBLEcallBack.BleBike_SubscriptionValueChanged(null, argsHRSensor);
                IBLEcallBack.BleBike_SubscriptionValueChanged(null, args);
                Thread.Sleep(125);
            }
        }

        private void updateTimeAndDistance()
        {
            if (SendingPage0x10Message.Time != 255)
            {
                SendingPage0x10Message.Time++;
            }
            else
            {
                SendingPage0x10Message.Time = 0;
            }

            updateDistance();
        }

        private void updateDistance()
        {
            double traveledMeters = (SendingPage0x10Message.Speed / 4.0);

            if (SendingPage0x10Message.Distance + traveledMeters <= 255)
            {
                SendingPage0x10Message.Distance += traveledMeters;
            }
            else
            {
                SendingPage0x10Message.Distance =
                    (traveledMeters - (255 - SendingPage0x10Message.Distance));
            }
        }

        private void updateEventCount()
        {
            if (SendingPage0x19Message.EventCount != 255)
            {
                SendingPage0x19Message.EventCount++;
            }
            else
            {
                SendingPage0x19Message.EventCount = 0;
            }
        }

        public void WriteCharacteristic(string address, byte[] byteArray)
        {
            int checksumCalculated = byteArray[0];
            for (int i = 1; i < byteArray.Length - 1; i++)
            {
                checksumCalculated = byteArray[i] ^ checksumCalculated;
            }

            if(checksumCalculated != byteArray[byteArray.Length - 1])
            {
                Console.WriteLine("Simulator: message received is corrupted\nAddress: {0}", address);
                return;
            }

            Resistance = (float)(byteArray[12] / 2.0);
        }
    }
    class Page0x10Message
    {
        // value represents the speed in m/s.
        public byte Speed;
        // value represents the heartrate in BPM.
        public byte Heartrate;
        // value represents the time in 0.25s, rollover at 255.
        public byte Time;
        // value represents distance in meters, rollover at 255.
        public double Distance;
        // if a lap is completed this value turns true.
        public bool LAP;
        // value represents the state of the device. 2 = READY.
        public byte FEState;
        
        public Page0x10Message()
        {
            Speed = 0;
            Heartrate = 0;
            Time = 0;
            Distance = 0;
            FEState = 2;
            LAP = false;
        }
        public byte[] getData()
        {
            // split the speed up in two bytes LSB and MSB.
            byte speedLSB = (byte)(((Speed * 1000) << 8) >> 8);
            byte speedMSB = (byte)((Speed * 1000) >> 8);

            // if lap is false the last bit of the last byte will be 0, else 1.
            byte capabilitiesAndFEtype = 0;

            if (LAP)
            {
                capabilitiesAndFEtype = 1;
            }

            // the last four bits are the LAP and FEtype, these are shifted together.
            capabilitiesAndFEtype = (byte)(capabilitiesAndFEtype << 3);
            capabilitiesAndFEtype += FEState;
            capabilitiesAndFEtype = (byte)(capabilitiesAndFEtype << 4);
            // After shifting the LAP and FEtype to the last four bits, the capabilities static value is added.
            capabilitiesAndFEtype += 0b0010;

            var returningData = new byte[] { 0xA4, 0x09, 0x4E, 0x05, 0x10, 0x19, Time, (byte)Math.Round(Distance), speedLSB, speedMSB, Heartrate, capabilitiesAndFEtype, 0 };

            byte checkSum = returningData[0];

            // XOR of all bytes.
            for (int i = 1; i < returningData.Length - 1; i++)
            {
                checkSum = (byte)(returningData[i] ^ checkSum);
            }

            returningData[returningData.Length - 1] = checkSum;

            return returningData;
        }
    }

    class Page0x19Message
    {
        public byte EventCount = 0;
        public int AccumulatedPower;
        public int InstantaneousPower;
        public int FEState;
        public bool LapTogleBit;

        public byte[] getData()
        {
            // Bitmask to get the first 8 bits and the 9th to 12th bit
            byte BIT_MASK_FIRST_EIGHT_BITS = (byte)0xff;
            int BIT_MASK_EIGHT_TO_TWELVE_BITS = (15 << 8);

            // Accumulated power calculation from big integer to two bytes
            byte accumulatedPowerLSB = (byte)((AccumulatedPower % 65536) & BIT_MASK_FIRST_EIGHT_BITS);
            byte accumulatedPowerMSB = (byte)((AccumulatedPower % 65536) >> 8);

            // Instantaneous power calculation from integer to 12 bits and last 4 bits are used for festate and laptoggle
            byte instantaneousPowerLSB = (byte)((InstantaneousPower % 4094) & BIT_MASK_FIRST_EIGHT_BITS);
            byte instantaneousPowerMSBAndTrainerStatus = (byte)((InstantaneousPower % 4094) & BIT_MASK_EIGHT_TO_TWELVE_BITS);
            byte feStateAndLapToggle = 0;
            if (LapTogleBit)
                feStateAndLapToggle = 1 << 7;
            feStateAndLapToggle = (byte)(feStateAndLapToggle & FEState << 3);

            // __TODO__ needs to get implemented using gears
            byte instantaneousCadence = 0; //__TODO__ 

            // Constructing the byte array we are going to send
            var returningData = new byte[] { 0xA4, 0x09, 0x4E, 0x05, 0x19, EventCount, instantaneousCadence, accumulatedPowerLSB, accumulatedPowerMSB, instantaneousPowerLSB, instantaneousPowerMSBAndTrainerStatus, feStateAndLapToggle, 0 };

            // Making the checksum
            byte checkSum = returningData[0];
            for (int i = 1; i < returningData.Length - 1; i++)
            {
                checkSum = (byte)(returningData[i] ^ checkSum);
            }

            // Setting the checksum
            returningData[returningData.Length - 1] = checkSum;

            return returningData;
        }
    }

    class HeartRateMessage
    {
        public byte HeartRate;

        public byte[] getData()
        {
            return new byte[] {0, HeartRate};
        }
    }
}
