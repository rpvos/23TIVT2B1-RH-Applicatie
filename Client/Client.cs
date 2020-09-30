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
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Client client = new Client();

        }

        private TcpClient server;
        private NetworkStream stream;

        private RSAClient rsaClient;

        private byte[] buffer;
        private string totalBuffer;

        private bool connectedSuccesfully;
        private string sessionID;

        public Client()
        {
            this.rsaClient = new RSAClient();

            this.server = new TcpClient("127.0.0.1", 8080);

            this.stream = this.server.GetStream();
            this.buffer = new byte[1024];

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);

            WriteTextMessage(getRequestMessage(this.rsaClient.getModulus(), this.rsaClient.getExponent()));

            Console.ReadKey();
        }

        #region stream dynamics
        public void WriteTextMessage(string message)
        {
            byte[] dataAsBytes = Encoding.UTF8.GetBytes(message + "\r\n\r\n");
            stream.Write(dataAsBytes, 0, dataAsBytes.Length);
            stream.Flush();
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                totalBuffer += receivedText;
            }
            catch (IOException)
            {
                Console.WriteLine("Server disconnected"); ;
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                handleData(packet);
            }
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }
        #endregion

        #region handle recieved data
        private void handleData(string packet)
        {
            try
            {
                JObject json = JObject.Parse(packet);
                if (!checkChecksum(json))
                    return;

                JObject data = (JObject)json["Data"];

                switch (json["Type"].ToString())
                {
                    case "response":
                        if (handleConnectionResponse(data))
                        {
                            sendCredentialMessage();
                            connectedSuccesfully = true;
                        }
                        break;

                    case "userCredentialsResponse":
                        if (handleUserCredentialsResponse(data))
                        {
                            Console.WriteLine("Login succesful");
                        }
                        else
                        {
                            Console.WriteLine("Login failed");
                        }
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

        private bool handleUserCredentialsResponse(JObject data)
        {
            this.sessionID = (string)data["Session"];

            //check if connected succesfully
            if (connectedSuccesfully)
            {
                return (bool)data["Status"];
            }
            else
            {
                return false;
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
        #endregion

        #region send handlers
        private void sendCredentialMessage()
        {
            string username = "admin";
            string password = "admin";

            WriteTextMessage(getUserDetails(username, password));
        }

        internal Task sendUpdatedValues(int session, int heartrate, double accDistance, double speed, double instPower, double accPower)
        {
            WriteTextMessage(getUpdateMessageString(session, heartrate, accDistance, speed, instPower, accPower));
            return Task.CompletedTask;
        }

        #endregion

        #region message construction
        private string getUserDetails(string username, string password)
        {
            dynamic json = new
            {
                Type = "userCredentials",
                Data = new
                {
                    Username = username,
                    Password = password
                },
                Checksum = 0
            };
            return addChecksum(json);
        }
        public string getUpdateMessageString(int session, int heartrate, double accDistance, double speed, double instPower, double accPower)
        {
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    HeartRate = heartrate,
                    AccumulatedDistance = accDistance,
                    Speed = speed,
                    InstantaniousPower = instPower,
                    AccumulatedPower = accPower
                },
                Checksum = 0
            };

            return addChecksum(json);
        }

        public string getMessageString(int session, string message)
        {
            dynamic json = new
            {
                Session = session,
                data = new
                {
                    Message = message
                },
                Checksum = 0
            };

            return addChecksum(json);
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

            return addChecksum(json);
        }

        private string addChecksum(dynamic dynamicJson)
        {
            JObject json = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(dynamicJson));
            byte checksum = 0;
            byte[] data = Encoding.ASCII.GetBytes(((JObject)json["Data"]).ToString());
            foreach (byte b in data)
            {
                checksum ^= b;
            }
            json["Checksum"] = checksum;

            return json.ToString();
        }
        #endregion

    }
}
