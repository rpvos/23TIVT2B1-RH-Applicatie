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
    public class ServerClient
    {

        private TcpClient client;
        private Server server;

        private Crypto crypto;
        private byte[] buffer;
        private string totalBuffer;

        public bool inSession { get; set; }

        public User user { get; set; }
        private StringBuilder logger;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;
            this.inSession = false; 

            this.buffer = new byte[1024];
            this.crypto = new Crypto(client.GetStream(), handleData, Disconnect);

            this.logger = new StringBuilder();
        }

        #region stream dynamics

        public void WriteTextMessage(string message)
        {
            logger.Append("\nServer:\n" + message);

            crypto.WriteTextMessage(message);
        }

        //This method writes logs to a text file.
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

        #region handle received data

        //This method receives packets and uses a switch case to handle different kind of types of packets.
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
                        this.server.SendToPatients(getJsonObject("globalmessage", data));
                        break;
                    case "resistance":
                        this.server.sendResistanceToOneClient(data);
                        this.server.sendResistanceToAllDoctors(data, this);
                        this.server.setResistancePerClient(data);
                        break;
                    case "privMessage":
                        this.server.sendPrivMessage(data);
                        break;
                    case "privateMessageToDoctor":
                        this.server.sendPrivateMessageToDoctors(data);
                        break;
                    case "disconnect":
                        Disconnect();
                        break;
                    case "inSession":
                        StartAndStopSession(data);
                        break;
                   
                    case"dataRequest":
                        requestData(data);
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

        public void StartAndStopSession(JObject data)
        {
            string username = (string)data["Username"];
            bool inSession = (bool)data["InSession"];

            //Check which user's session is stopped or started.
            foreach (ServerClient client in this.server.Clients)
                if (client.user.getUsername() == username)
                {
                    client.inSession = inSession;
                    if (!client.inSession)
                        this.server.saveUser(client.user);
                    break;
                }

           
        }

        internal void sendClientLeft(ServerClient leavingClient)
        {
            WriteTextMessage(getLeavingClientString(leavingClient.user.getUsername()));
        }

        

        public void requestData(JObject data)
        {
            string username = (string)data["Username"];

            //Check from which user the data is requested.
            foreach(ServerClient client in this.server.Clients)
                if(client.user.getUsername() == username)
                {
                    sendDataToDoctor(client);
                }
        }

        public void sendDataToDoctor(ServerClient client)
        {
            //Send each packet of data to the doctor.
            foreach (DataSet set in client.user.userDataStorage.dataSets)
            {
                sendDataSet(set.UpdateType, set.Value, set.DateStamp,client.user.getUsername());
            }
            WriteTextMessage(getFinishedUserString(client.user.getUsername()));
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

            if (this.inSession)
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
                if (this.user.getRole() == Role.Patient)
                {
                    sendAddUserMessage(username);

                    //Adds a spot in the resistance list when connected.
                    this.server.usernameAndResistance.Add(username, "0");
                }

                if (this.user.getRole() == Role.Doctor)
                    this.server.addUsersToThisDoctorClient(this);

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
            WriteTextMessage(getResistanceString(resistance, this.user.getUsername()));
        }

        private void sendDataSet(UpdateType updateType, double value, DateTime dateStamp, string username)
        {
            WriteTextMessage(getDataSetString(updateType, value, dateStamp, username));
        }

        public void sendResistanceToDoctor(string resistance, string username)
        {
            WriteTextMessage(getResistanceString(resistance, username));
        }

        public void sendPrivMessage(string message)
        {
            WriteTextMessage(getMessageString(message));
        }

        public void sendPrivateMessageToDoctor(string username, string message)
        {
            WriteTextMessage(getMessageStringToDoctor(username, message));
        }

        private string getAddUserString(string username)
        {
            dynamic data = new
            {
                Username = username
            };

            return getJsonObject("AddUser", data);
        }

        private string getLeavingClientString(string username)
        {
            dynamic data = new
            {
                Username = username
            };

            return getJsonObject("LeftClient", data);
        }

        private string getFinishedUserString(string username)
        {
            dynamic data = new
            {
                username = username
            };

            return getJsonObject("SendingDataSetsFinished", data);
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

        private string getDataSetString(UpdateType updateType, double value, DateTime dateStamp, string username)
        {
            dynamic data = new
            {
                UpdateType = updateType,
                Value = value,
                DateStamp = dateStamp,
                Username = username
            };

            return getJsonObject("dataSet", data);
        }

        private string getMessageStringToDoctor(string username, string message)
        {
            dynamic data = new
            {
                Message = message,
                Username = username
            };

            return getJsonObject("message", data);
        }

        private string getResistanceString(string resistance, string username)
        {
            dynamic data = new
            {
                Resistance = resistance,
                Username = username
            };

            return getJsonObject("resistance", data);
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
            {
                server.RemoveThisClient(this);
                Console.WriteLine("Login attempt failed");
            }

            WriteTextMessage(getUserCredentialsResponse(role));
        }


        internal void sendMessage(string message)
        {
            WriteTextMessage(getMessageString(message));
        }

        #endregion

        #region disconnecting
        public void Disconnect()
        {
            //This part makes sure the client is disconnected safely.
            this.server.Clients.Remove(this);
            this.server.usernameAndResistance.Remove(this.user.getUsername());
            this.user.loggedIn = false;
            this.server.Disconnect(this);

            // Try to close the stream
            try
            {
                this.client.GetStream().Close();
                this.client.Close();
            }
            catch { }

            Console.WriteLine(this.user.getUsername() + " disconnected");
        }
        #endregion
    }
}