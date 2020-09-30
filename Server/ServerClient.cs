
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
        private bool encoded;

        private int sessionID;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;

            this.buffer = new byte[1024];
            this.stream = client.GetStream();
            this.encoded = false;

            this.rsaClient = new RSAClient();

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        #region stream dynamics
        public void WriteTextMessage(string message)
        {
            if (!encoded)
            {
                byte[] dataAsBytes = System.Text.Encoding.ASCII.GetBytes(message + "\r\n\r\n");
                stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                stream.Flush();
            }
            else
            {
                //encode that shit
            }
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                totalBuffer += receivedText;
            }
            catch (IOException)
            {
                server.Disconnect(this);
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
                if (encoded)
                    packet = rsaClient.decryptMessage(packet);

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

        private bool handleUserCredentials(JObject data)
        {
            string username = (string)data["Username"];
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
        private string getUserCredentialsResponse(bool hasSucceeded)
        {
            this.sessionID = server.getSessionID();
            dynamic json = new
            {
                Type = "userCredentialsResponse",
                Data = new
                {
                    Status = hasSucceeded,
                    Session = sessionID
                },
                Checksum = 0
            };
            return addChecksum(json);
        }
        private string getConnectionResponseMessage(byte[] modulus, byte[] exponent)
        {

            dynamic json = new
            {
                Type = "response",
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

        #region send handlers
        private void sendUserCredentialsResponse(bool hasSucceeded)
        {
            if (hasSucceeded)
            {
                Console.WriteLine("Login attempt succeeded");
            }
            else
                Console.WriteLine("Login attempt failed");

            WriteTextMessage(getUserCredentialsResponse(hasSucceeded));
        }

        
        private void sendConnectionRequest()
        {
            WriteTextMessage(getConnectionResponseMessage(rsaClient.getModulus(), rsaClient.getExponent()));
        }

        #endregion

    }
}