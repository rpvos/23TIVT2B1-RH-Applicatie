
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Server
{
    internal class ServerClient
    {

        private TcpClient client;
        private Server server;

        private NetworkStream stream;

        private byte[] buffer;
        private string totalBuffer;

        private RSAClient rsaClient;

        private string username;
        private StringBuilder logger;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;

            this.buffer = new byte[1024];
            this.stream = client.GetStream();

            this.rsaClient = new RSAClient();

            this.logger = new StringBuilder();


            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        #region stream dynamics

        public void WriteTextMessage(string message)
        {
            logger.Append("\nServer:\n" + message);

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
                server.Disconnect(this);
                log();
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));

                logger.Append("\nClient:\n" + packet);

                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                handleData(packet);
            }
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void log()
        {
            string location = Environment.CurrentDirectory;
            if (username != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(location + $"/log{username}.txt", false))
                {
                    streamWriter.Write(this.logger.ToString());
                    streamWriter.Flush();
                }
            }
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(location + $"/logUnconnected.txt", true))
                {
                    streamWriter.Write(this.logger.ToString());
                    streamWriter.Flush();
                }
            }
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
                    case "request":
                        if (handleConnectionRequest(data))
                            sendConnectionRequest();
                        break;

                    case "userCredentials":
                        sendUserCredentialsResponse(handleUserCredentials(data));
                        break;

                    case "updateType":
                        handleUpdateInformation(data);
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

        private void handleUpdateInformation(JObject data)
        {
            //todo handle update information
            throw new NotImplementedException();
        }

        private bool handleUserCredentials(JObject data)
        {
            this.username = (string)data["Username"];
            string password = (string)data["Password"];

            return server.checkUser(username, password);
        }

        private bool handleConnectionRequest(JObject json)
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

        #region message construction

        private string getJsonObject(string type, dynamic data)
        {
            dynamic json = new
            {
                Type = type,
                Data = data,
                Checksum = 0
            };
            return addChecksum(json);
        }

        private string getUserCredentialsResponse(bool hasSucceeded)
        {
            dynamic data = new
            {
                Status = hasSucceeded
            };

            return getJsonObject("userCredentialsResponse", data);
        }

        private string getConnectionResponseMessage(byte[] modulus, byte[] exponent)
        {
            dynamic data = new
            {
                Modulus = modulus,
                Exponent = exponent
            };

            return getJsonObject("response", data);
        }

        private string getMessageString(string message)
        {
            dynamic data = new
            {
                Message = message
            };

            return getJsonObject("message", data);
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

        #region send handlers

        private void sendUserCredentialsResponse(bool hasSucceeded)
        {
            if (hasSucceeded)
                Console.WriteLine("Login attempt succesful");
            else
                Console.WriteLine("Login attempt failed");

            WriteTextMessage(getUserCredentialsResponse(hasSucceeded));
        }


        private void sendConnectionRequest()
        {
            WriteTextMessage(getConnectionResponseMessage(rsaClient.getModulus(), rsaClient.getExponent()));
        }

        internal void sendMessage(string message)
        {
            WriteTextMessage(getMessageString(message));
        }

        #endregion

    }
}