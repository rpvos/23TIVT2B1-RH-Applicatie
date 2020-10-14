using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedItems;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server
{
    internal class ServerClient
    {

        private TcpClient client;
        private Server server;

        private Crypto crypto;
        private byte[] buffer;
        private string totalBuffer;

        public User user { get; set; }
        private StringBuilder logger;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;

            this.buffer = new byte[1024];
            this.crypto = new Crypto(client.GetStream(),handleData);


            this.logger = new StringBuilder();
        }

        #region stream dynamics

        public void WriteTextMessage(string message)
        {
            logger.Append("\nServer:\n" + message);

            crypto.WriteTextMessage(message);
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
                    case "globalmessage":
                        this.server.SendToPatients(getJsonObject("globalmessage",data));
                        break;
                    case "resistance":
                        this.server.sendResistanceToOneClient(data);
                        break;
                    case "privMessage":
                        this.server.sendPrivMessage(data);
                        break;

                    default:
                        Console.WriteLine("Invalid type");
                        break;
                }
            }
            catch (JsonReaderException)
            {
                Console.WriteLine(packet);
                Console.WriteLine("Invalid message");
            }
        }


        private void handleUpdateInformation(JObject data)
        {
            UpdateType updateType = (UpdateType)Enum.Parse(typeof(UpdateType), (string)data["UpdateType"], true);
            double value = (double)data["Value"];

            dynamic parsedData = new
            {
                UpdateType = updateType.ToString(),
                Username = user.getUsername(),
                Value = value
            };

            this.user.userDataStorage.addDataSet(updateType, value);
            this.server.SendToDoctors(getJsonObject("update", parsedData, this.user));
        }

        private Role handleUserCredentials(JObject data)
        {
            string username = (string)data["Username"];
            string password = (string)data["Password"];

            this.user = server.checkUser(username, password);
            if (user != null)
            {
                if(user.getRole() == Role.Patient)
                    sendAddUserMessage(username);

                return user.getRole();

            }
            else
                return Role.Invallid;
        }

        public void sendAddUserMessage(string username)
        {
            this.server.SendToDoctors(getAddUserString(username));
        }

        public void sendResistance(string resistance)
        {
            WriteTextMessage(getResistanceString(resistance));
        }

        public void sendPrivMessage(string message)
        {
            WriteTextMessage(getMessageString(message));
        }

        private string getAddUserString(string username)
        {
            dynamic data = new
            {
                Username = username
            };

            return getJsonObject("AddUser", data);
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


        private  string getResistanceString(string resistance)
        {
            dynamic data = new
            {
                Resistance = resistance
            };

            return getJsonObject("Resistance", data);
        }

        /// <summary>
        /// Adds an value to the checksum of the message
        /// </summary>
        /// <param name="dynamicJson">the message in dynamic format</param>
        /// <returns>the message in string format with checksum calculated</returns>
        private string addChecksum(dynamic dynamicJson)
        {
            JObject json = JObject.Parse(JsonConvert.SerializeObject(dynamicJson));
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