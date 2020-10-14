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

        private List<string> usernames;
        private Crypto crypto;

        private byte[] buffer;
        private string totalBuffer;

        private Login login;
        public string selectedUsername { get; set; }

        static void Main()
        {
            DoctorClient doctorServer = new DoctorClient();
            doctorServer.startLogin();
        }

        public DoctorClient()
        {
            this.selectedUsername = "-1";
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

            this.buffer = new byte[1024];
            this.crypto = new Crypto(server.GetStream(), handleData);

            this.usernames = new List<string>();

            //stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);

            WriteTextMessage(getUserDetailsMessageString(username, password));
        }

        #region stream dynamics
        public void WriteTextMessage(string message)
        {
            crypto.WriteTextMessage(message);
        }
        public void sendPrivMessage(string message, string username)
        {

            WriteTextMessage(getPrivMessageString(message, username));
        }

        
        public void sendResistance(string resistance, string username)
        {
            WriteTextMessage(getResistanceString(resistance, username));
        }
        #endregion

        #region handle received data
        private void handleData(string packet)
        {
            try
            {
                Console.WriteLine(packet);

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
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
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

            string username = (string)data["Username"];

            if (username == this.selectedUsername)
            {
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


