using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UpdateType = Shared.UpdateType;

namespace FietsDemo
{
    public class UserClient
    {
        private TcpClient server;
        private NetworkStream stream;

        private byte[] buffer;
        private string totalBuffer;

        public UserClient(string username, string password)
        {
            this.server = new TcpClient("127.0.0.1", 8080);

            this.stream = this.server.GetStream();
            this.buffer = new byte[1024];

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            //WriteTextMessage(getUserDetailsMessageString("stoeptegel", "123"));
            WriteTextMessage(getUserDetailsMessageString(username, password));
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
                string type = json["Type"].ToString();



                switch (type)
                {
                    case "userCredentialsResponse":
                        if (handleUserCredentialsResponse(data))
                        {
                            Console.WriteLine("Login succesful");
                        }
                        else
                        {
                            Console.WriteLine("Login failed");
                            //System.Environment.Exit();
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
            //check if connected succesfully
            return (bool)data["Status"] && (Role)Enum.Parse(typeof(Role), (string)data["Role"], true) == Role.Patient;
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
        private void sendCredentialMessage(string username, string password)
        {
            username = "admin";
            password = "admin";

            WriteTextMessage(getUserDetailsMessageString(username, password));
        }

        internal Task sendUpdatedValues(Shared.UpdateType valueType, double value)
        {
            WriteTextMessage(getUpdateMessageString(valueType, value));
            return Task.CompletedTask;
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

        private string getUserDetailsMessageString(string username, string password)
        {
            dynamic data = new
            {
                Username = username,
                Password = password
            };

            return getJsonObject("userCredentials", data);
        }
        private string getUpdateMessageString(UpdateType updateType, double value)
        {
            dynamic data = new
            {
                UpdateType = updateType.ToString(),
                Value = value
            };

            return getJsonObject("update", data);
        }


        private string getUpdateMessageString(string type, double value)
        {
            dynamic data = new
            {
                Type = type,
                Value = value
            };

            return getJsonObject("updateType", data);
        }

        private string getMessageString(string message)
        {
            dynamic data = new
            {
                Message = message
            };

            return getJsonObject("message", data);
        }

        private string getRequestMessage(byte[] modulus, byte[] exponent)
        {
            List<byte> modulusList = new List<byte>(modulus);
            List<byte> exponentList = new List<byte>(exponent);
            dynamic data = new
            {
                Modulus = modulusList,
                Exponent = exponentList
            };

            return getJsonObject("request", data);
        }

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

    }
}
