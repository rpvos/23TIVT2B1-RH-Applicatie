using Avans.TI.BLE;
using System.Threading;

namespace FietsDemo
{
    class BikeSimulator
    {
        public bool running;
        private IBLEcallBack IBLEcallBack;
        private Page0x10Message SendingPage0x10Message;
        private Page0x19Message SendingPage0x19Message;
        public BikeSimulator(IBLEcallBack bLEcallBack)
        {
            IBLEcallBack = bLEcallBack;
            SendingPage0x10Message = new Page0x10Message(50, 50);
            SendingPage0x19Message = new Page0x19Message();
            running = true;
            Thread backgroundSender = new Thread(new ThreadStart(update));
            backgroundSender.Start();
        }

        public void setSpeed(byte speed)
        {
            SendingPage0x10Message.Speed = speed;
        }

        public void setPower(byte power)
        {
            SendingPage0x19Message.InstantaneousPower = power;
        }

        public void setHeartRate(byte heartrate)
        {
            SendingPage0x10Message.Heartrate = heartrate;
        }

        private void update()
        {
            bool page = false;
            while (running)
            {
                if (page)
                {
                    BLESubscriptionValueChangedEventArgs args = new BLESubscriptionValueChangedEventArgs
                    {
                        Data = SendingPage0x10Message.getData(),
                        ServiceName = "Simulator"
                    };

                    page = false;
                }
                else
                {
                    BLESubscriptionValueChangedEventArgs args = new BLESubscriptionValueChangedEventArgs
                    {
                        Data = SendingPage0x19Message.getData(),
                        ServiceName = "Simulator"
                    };
                    updateEventCount();
                    page = true;
                }

                updateTime();
                Thread.Sleep(250);
            }
        }
        private void updateTime()
        {
            if (SendingPage0x10Message.Time != 255)
            {
                SendingPage0x10Message.Time++;
            }
            else
            {
                SendingPage0x10Message.Time = 0;
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
    }
    class Page0x10Message
    {
        public byte Speed;
        public byte Heartrate;
        public byte Time;

        public Page0x10Message(byte speed, byte heartrate)
        {
            Speed = speed;
            Heartrate = heartrate;
            Time = 0;
        }
        public byte[] getData()
        {
            byte speedLSB = (byte)(((Speed * 1000) << 8) >> 8);
            byte speedMSB = (byte)((Speed * 1000) >> 8);

            var returningData = new byte[] { 0xA4, 0x09, 0x4E, 0x05, 0x10, 0, Time, 0, speedLSB, speedMSB, Heartrate, 0, 0 };
            byte checkSum = returningData[0];
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
}
