using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FietsDemo
{
    public class UserClient
    {
        private TcpClient server;

        private Crypto crypto;


        public UserClient()
        {
            this.server = new TcpClient("127.0.0.1", 8080);

            this.crypto = new Crypto(server.GetStream(),handleData);

            WriteTextMessage(getUserDetailsMessageString("stoeptegel", "123"));
        }

        private void WriteTextMessage(string message)
        {
            crypto.WriteTextMessage(message);
        }

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

        internal Task sendUpdatedValues(SharedItems.UpdateType valueType, double value)
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
