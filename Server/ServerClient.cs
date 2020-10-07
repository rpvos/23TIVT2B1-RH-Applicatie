
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ValueType = Shared.ValueType;

namespace Server
{
    internal class ServerClient
    {

        private TcpClient client;
        private Server server;

        private NetworkStream stream;

        private byte[] buffer;
        private string totalBuffer;

        public User user { get; set; }
        private StringBuilder logger;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;

            this.buffer = new byte[1024];
            this.stream = client.GetStream();


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
            if (user != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(location + $"/log{user.getUsername()}.txt", false))
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
                    case "userCredentials":
                        sendUserCredentialsResponse(handleUserCredentials(data));
                        break;

                    case "update":
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
            ValueType valueType = (ValueType)Enum.Parse(typeof(ValueType), (string)data["ValueType"], true);
            double value = (double)data["Value"];
            
            dynamic parsedData = new 
            {
                ValueType = valueType.ToString(),
                Value = value
            };

            this.server.SendToDoctors(getJsonObject("update",parsedData,this.user));
        }

        private Role handleUserCredentials(JObject data)
        {
            string username = (string)data["Username"];
            string password = (string)data["Password"];

            this.user = server.checkUser(username, password);
            if (user != null)
                return user.getRole();
            else
                return Role.Invallid;
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

        private string getJsonObject(string type, dynamic data, User user)
        {
            dynamic json = new
            {
                Type = type,
                Data = data,
                Username = user.getUsername(),
                Checksum = 0
            };
            return addChecksum(json);
        }

        private string getUserCredentialsResponse(Role role)
        {
            bool hasSucceeded = (role != Role.Invallid);

            dynamic data = new
            {
                Status = hasSucceeded,
                Role = role.ToString()
            };

            return getJsonObject("userCredentialsResponse", data);
        }

        private string getMessageString(string message)
        {
            dynamic data = new
            {
                Message = message
            };

            return getJsonObject("message", data);
        }

        /// <summary>
        /// Adds an value to the checksum of the message
        /// </summary>
        /// <param name="dynamicJson">the message in dynamic format</param>
        /// <returns>the message in string format with checksum calculated</returns>
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

        private void sendUserCredentialsResponse(Role role)
        {
            if (role != Role.Invallid)
                Console.WriteLine($"Login as {role}");
            else
                Console.WriteLine("Login attempt failed");

            WriteTextMessage(getUserCredentialsResponse(role));
        }


        internal void sendMessage(string message)
        {
            WriteTextMessage(getMessageString(message));
        }

        #endregion
    }
}