using Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace GameClient
{
    class Client
    {
        static void Main(string[] args)
        {
            Client client = new Client();

        }

        private String username;
        private TcpClient server;
        private NetworkStream stream;
        private RSAClient rsaClient;
        private byte[] buffer;
        private string totalBuffer;

        public Client()
        {
            this.rsaClient = new RSAClient();

            this.server = new TcpClient("127.0.0.1", 8080);

            this.stream = this.server.GetStream();
            this.buffer = new byte[1024];

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);

            Console.WriteLine(getRequestMessage(this.rsaClient.getModulus(), this.rsaClient.getExponent()));
            WriteTextMessage(getRequestMessage(this.rsaClient.getModulus(), this.rsaClient.getExponent()));

            Console.ReadKey();
        }

        public void WriteTextMessage(string message)
        {
            var dataAsBytes = System.Text.Encoding.ASCII.GetBytes(message + "\r\n\r\n");
            stream.Write(dataAsBytes, 0, dataAsBytes.Length);
            stream.Flush();
        }

        private void OnRead(IAsyncResult ar)
        {
            int receivedBytes = stream.EndRead(ar);
            string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            totalBuffer += receivedText;

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                handleData(packet);
            }
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void handleData(string packet)
        {
            try
            {
                packet = rsaClient.decryptMessage(packet);
                Console.WriteLine(packet);

                JObject json = JObject.Parse(packet);
                if (!checkChecksum(json))
                    return;

                switch (json["Type"].ToString())
                {
                    case "response":
                        if (handleConnectionResponse((JObject)json["Data"]))
                            Console.WriteLine("correct :)");

                        break;
                    default:
                        Console.WriteLine("Invalid type");
                        break;
                }
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("Invalid message");
            }
        }

        private bool handleConnectionResponse(JObject json)
        {
            byte[] modulus = Encoding.ASCII.GetBytes((string)json["Modulus"]);
            byte[] exponent = Encoding.ASCII.GetBytes((string)json["Exponent"]);
            try
            {
                rsaClient.setKey(modulus, exponent);
                return true;
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Wrong key value");
            }
            return false;
        }


        private bool checkChecksum(JObject json)
        {
            byte checksum = (byte)json["Checksum"];
            JObject jObject = (JObject)json["Data"];
            byte[] data = Encoding.ASCII.GetBytes(jObject.ToString());
            foreach (byte b in data)
                checksum ^= b;
            return checksum == 0;
        }

        public string getUpdateMessageString(int session, int heartrate, double accDistance, double speed, double instPower, double accPower)
        {
            int checksum = 0;
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    HeartRate = heartrate,
                    AccumulatedDistance = accDistance,
                    Speed = speed,
                    InstantaniousPower = instPower
                },
                Checksum = checksum
            };

            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        public string getMessageString(int session, string message)
        {
            int checksum = 0;
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    Message = message
                },
                Checksum = checksum
            };

            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        public string getRequestMessage(byte[] modulus, byte[] exponent)
        {

            dynamic json = new
            {
                Type = "request",
                Data = new
                {
                    Modulus = modulus,
                    Exponent = exponent
                },
                Checksum = 0
            };

            JObject jObject = addChecksum(json);

            return jObject.ToString();
        }

        private JObject addChecksum(dynamic dynamicJson)
        {
            JObject json = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(dynamicJson));
            byte checksum = 0;
            byte[] data = Encoding.ASCII.GetBytes(((JObject)json["Data"]).ToString());
            foreach (byte b in data)
            {
                checksum ^= b;
            }
            json["Checksum"] = checksum;

            return json;
        }
    }
}
