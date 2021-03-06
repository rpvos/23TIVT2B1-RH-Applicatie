using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using WPFDoctorApplication.Models;
using WPFDoctorApplication.ViewModels;

namespace WPFDoctorApplication
{
    /// <summary>
    /// Class that connects to the server
    /// From the server it gets all the information about clients
    /// </summary>
    public class DoctorClient
    {
        private ShellViewModel shellViewModel;
        private TcpClient server;
        private string username;

        private List<string> usernames;
        private Crypto crypto;

        private byte[] buffer;
        private string totalBuffer;
        private int testCounter;

        public string SelectedUsername { get; set; }
        public ObservableCollection<PatientBike> PatientBikeList;

        public DoctorClient(ShellViewModel shellViewModel)
        {
            SelectedUsername = "-1";
            this.shellViewModel = shellViewModel;
            PatientBikeList = new ObservableCollection<PatientBike>();
            this.shellViewModel.PatientBikeList = PatientBikeList;
        }

        public void StartClient()
        {
            server = new TcpClient("127.0.0.1", 8080);

            buffer = new byte[1024];
            crypto = new Crypto(server.GetStream(), handleData, disconnect);

            usernames = new List<string>();

        }

        public void senderUserCredentials(string username, string password)
        {
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

        public void sendInSession(bool inSession, string username)
        {
            WriteTextMessage(getInSessionString(inSession, username));
        }

        public void AskUserData(string username)
        {
            WriteTextMessage(getUserDataRequestString(username));
        }
        #endregion

        #region handle received data
        private void handleData(string packet)
        {
            {

                JObject json = JObject.Parse(packet);
                if (!checkChecksum(json))
                {
                    return;
                }

                JObject data = (JObject)json["Data"];
                string type = json["Type"].ToString();



                switch (type)
                {
                    case "userCredentialsResponse":
                        if (handleUserCredentialsResponse(data))
                        {
                            Console.WriteLine("Login succesful");
                            //Thread startThread = new Thread(Start);
                            //startThread.Start();
                            shellViewModel.LoginViewModel.LoginSucces();

                        }
                        else
                        {
                            Console.WriteLine("Login failed");
                            shellViewModel.LoginViewModel.LoginFailed();
                        }
                        break;
                    case "update":
                        handleUpdate(data);
                        break;
                    case "AddUser":
                        AddUser(data);
                        break;
                    case "message":
                        AddMessage(data);
                        break;
                    case "resistance":
                        setResistance(data);
                        break;
                    case "dataSet":
                        addDataSet(data);
                        break;
                    case "SendingDataSetsFinished":
                        OpenHistoryWindow(data);
                        break;
                    case "LeftClient":
                        removeLeftClient(data);
                        break;
                    default:
                        Console.WriteLine("Invalid type");
                        break;
                }
            }
        }

        private void AddUser(JObject data)
        {
            string username = (string)data["Username"];

            //App.Current.Dispatcher in order to avoid threading problems
            App.Current.Dispatcher.Invoke(delegate
            {
                bool contains = false;
                foreach (PatientBike bike in PatientBikeList)
                {
                    if (bike.Username == username)
                    {
                        contains = true;
                        break;
                    }

                }

                if (!contains)
                {
                    PatientBikeList.Add(new PatientBike(this, username));
                }
            }
             );
            shellViewModel.DebugMessage = "Added Client";
        }
        private void removeLeftClient(JObject data)
        {
            string username = (string)data["Username"];

            //App.Current.Dispatcher in order to avoid threading problems
            App.Current.Dispatcher.Invoke(() =>
            {
                for (int i = PatientBikeList.Count - 1; i >= 0; i--)
                {
                    if (PatientBikeList[i].Username == username)
                    {
                        lock (PatientBikeList[i])
                        {
                            PatientBikeList.Remove(PatientBikeList[i]);
                        }
                    }
                }
            });
        }

        private void OpenHistoryWindow(JObject data)
        {
            string username = (string)data["username"];
            foreach (PatientBike patientBike in PatientBikeList)
            {
                if (patientBike.Username == username)
                {
                    App.Current.Dispatcher.Invoke(patientBike.ShowHistoricalDataWindow);
                }
            }
        }

        private void AddMessage(JObject data)
        {
            string message = (string)data["Message"];
            string username = (string)data["Username"];

            foreach (PatientBike patientBike in PatientBikeList)
            {
                if (patientBike.Username.Equals(username))
                {
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        patientBike.PrivateChatList.Add(username + ": " + message);
                    });
                }
            }
        }

        private void addDataSet(JObject data)
        {
            UpdateType type = (UpdateType)Enum.Parse(typeof(UpdateType), (string)data["UpdateType"], true);
            double value = (double)data["Value"];
            DateTime dateStamp = (DateTime)data["DateStamp"];
            string username = (string)data["Username"];

            foreach (PatientBike patientBike in PatientBikeList)
            {
                if (patientBike.Username == username)
                {
                    patientBike.AddDataSet(new DataSet(type, value, dateStamp));
                }
            }
        }

        public void setResistance(JObject data)
        {
            string username = (string)data["Username"];
            double resistance = (double)data["Resistance"];

            typeDivider(UpdateType.Resistance, username, resistance);
        }

        private void handleUpdate(JObject data)
        {
            UpdateType type = (UpdateType)Enum.Parse(typeof(UpdateType), (string)data["UpdateType"], true);

            string username = (string)data["Username"];
            double value = (double)data["Value"];

            App.Current.Dispatcher.Invoke(() =>
            {
                typeDivider(type, username, value);
            });
        }

        public void typeDivider(UpdateType type, string username, double value)
        {
            foreach (PatientBike patientBike in PatientBikeList)
            {
                if (patientBike.Username.Equals(username))
                {
                    switch (type)
                    {
                        case UpdateType.Heartrate:
                            patientBike.HeartRate = value;
                            break;
                        case UpdateType.Speed:
                            patientBike.Speed = value;
                            break;
                        case UpdateType.AccumulatedPower:
                            patientBike.AccumulatedPower = value;
                            break;
                        case UpdateType.InstantaniousPower:

                            break;
                        case UpdateType.AccumulatedDistance:
                            patientBike.DistanceTraveled = value;
                            break;
                        case UpdateType.ElapsedTime:
                            patientBike.ElapsedTime = value;
                            break;
                        case UpdateType.Resistance:
                            patientBike.ResistanceValue = (int)value;
                            break;
                    }
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
            {
                checksum ^= b;
            }

            return checksum == 0;
        }
        #endregion

        #region message construction

        private string getDisconnectString(string username)
        {
            dynamic data = new
            {
                Username = username
            };

            return getJsonObject("disconnect", data);
        }
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

        private string getResistanceString(string resistance, string username)
        {
            dynamic data = new
            {
                Resistance = resistance,
                Username = username
            };

            return getJsonObject("resistance", data);
        }

        private string getInSessionString(bool inSession, string username)
        {
            dynamic data = new
            {
                InSession = inSession,
                Username = username
            };

            return getJsonObject("inSession", data);
        }

        public string getUserDataRequestString(string username)
        {
            dynamic data = new
            {
                Username = username
            };

            return getJsonObject("dataRequest", data);
        }

        private string getPrivMessageString(string message, string username)
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

        #region disconnecting


        public void disconnect()
        {
            WriteTextMessage(getDisconnectString(username));
            crypto.disconnect();
        }
        #endregion
    }
}


