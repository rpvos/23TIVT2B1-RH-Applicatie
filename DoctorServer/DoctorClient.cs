using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Text;
using SharedItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace DoctorServer
{
    public class DoctorClient
    {

        private DoctorForm mainForm;
        private TcpClient server;
        private NetworkStream stream;

        private List<string> usernames;

        private byte[] buffer;
        private string totalBuffer;

        private Login login;

        static void Main()
        {
            DoctorClient doctorServer = new DoctorClient();
            doctorServer.startLogin();
        }

        public void startLogin()
        {

            this.login = new Login(this);
            this.login.run();

        }


        public void Start()
        {
            //startClient();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();

            this.mainForm = new DoctorForm(this);
            //ShowDialog() instead of Application.Run() could affect the working a bit
            this.mainForm.ShowDialog();
            //Application.Run(mainForm);
        }

  

        public void startClient(string username, string password)
        {
            this.server = new TcpClient("127.0.0.1", 8080);

            this.stream = this.server.GetStream();
            this.buffer = new byte[1024];

            this.usernames = new List<string>();

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);

            WriteTextMessage(getUserDetailsMessageString(username, password));
        }

        #region stream dynamics
        public void WriteTextMessage(string message)
        {
            byte[] dataAsBytes = Encoding.UTF8.GetBytes(message + "\r\n\r\n");
            stream.Write(dataAsBytes, 0, dataAsBytes.Length);
            stream.Flush();
        }

        public void sendResistance(string resistance, string username)
        {
            WriteTextMessage(getResistanceString(resistance, username));
        }

        public void sendPrivMessage(string message, string username)
        {
            WriteTextMessage(getPrivMessageString(message, username));

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
                            Thread startThread = new Thread(Start);
                            startThread.Start();
                            this.login.loginSucceeded();
                        }
                        else
                        {
                            Console.WriteLine("Login failed");
                            this.login.loginFailed();
                        }
                        break;
                    case "update":
                        handleUpdate(data);
                        break;
                    case "AddUser":
                        AddUser(data);
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

        private void AddUser(JObject data)
        {
           
                string username = (string)data["Username"];
            this.mainForm.addBike(username);

            
        }

        private void handleUpdate(JObject data)
        {
            UpdateType type = (UpdateType)Enum.Parse(typeof(UpdateType), (string)data["UpdateType"], true);
            double value = (double)data["Value"];
            switch (type)
            {
                case UpdateType.AccumulatedDistance:
                    mainForm.setDT(value.ToString());
                    break;

                case UpdateType.AccumulatedPower:
                    mainForm.setAP(value.ToString());
                    break;

                case UpdateType.ElapsedTime:
                    mainForm.setElapsedTime(value.ToString());
                    break;

                case UpdateType.Heartrate:
                    mainForm.setHeartrate(value.ToString());
                    break;

                case UpdateType.InstantaniousPower:
                    //TODO mainForm.set(value.ToString());
                    break;

                case UpdateType.Resistance:
                    //TODO doctor sends resistance and client doesn't set resitance except vr
                    break;

                case UpdateType.Speed:
                    mainForm.setSpeed(value.ToString());
                    break;
            }
        }

        private bool handleUserCredentialsResponse(JObject data)
        {
            // Check if the status is ok and the user is a doctor that is signing in
            return (bool)data["Status"] && (Role)Enum.Parse(typeof(Role), (string)data["Role"], true) == Role.Doctor;
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
        private string getMessageString(string message)
        {
            dynamic data = new
            {
                Message = message
            };

            return getJsonObject("message", data);
        }

        private string getResistanceString(string resistance, String username)
        {
            dynamic data = new
            {
                Resistance = resistance,
                Username = username
            };

            return getJsonObject("resistance", data);
        }

        private string getPrivMessageString(string message, String username)
        {
            dynamic data = new
            {
                Message = message,
                Username = username
            };

            return getJsonObject("privMessage", data);
        }

        private string getGlobalMessageString(string message)
        {
            dynamic data = new
            {
                Message = message
            };

            return getJsonObject("globalmessage", data);
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

        public void sendGlobalChatMessage(string message)
        {
            WriteTextMessage(getGlobalMessageString(message));
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

        #region send handlers
        private void sendCredentialMessage(string username, string password)
        {
            WriteTextMessage(getUserDetailsMessageString(username, password));
        }
        #endregion
    }


}
