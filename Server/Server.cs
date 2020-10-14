using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        #region private atributes

        private List<ServerClient> clients;
        private TcpListener listener;
        private Dictionary<string, User> dataBase;
        private CryptoFileSaver cryptoFileSaver;

        #endregion

        static void Main(string[] args)
        {
            Server server = new Server();

        }

        #region Start up methods

        public Server()
        {
            this.clients = new List<ServerClient>();

            this.dataBase = new Dictionary<string, User>();
            this.cryptoFileSaver = new CryptoFileSaver("data_saves");
            //fillUsers();
            loadUsers();

            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(localhost, 8080);

            Console.WriteLine("Starting server");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.WriteLine(dataBase.Count);
            //this.saveAllUsers();

            Console.ReadKey();
        }

        private void loadUsers()
        {
            string[] users = this.cryptoFileSaver.GetSavedClients();

            foreach(string serializedUser in users)
            {
                User user = (User)JsonConvert.DeserializeObject(serializedUser);
                this.dataBase.Add(user.username, user);
            }
        }

        private void fillUsers()
        {
            dataBase.Add("stoeptegel", new User("Stijn", "stoeptegel", "123", Role.Patient));
            dataBase.Add("aardappel", new User("Piet", "aardappel", "321", Role.Patient));

            dataBase.Add("dokter", new User("dokter", "dokter", "123", Role.Doctor));
        }

        public void saveUser(User user)
        {
            this.cryptoFileSaver.WriteUserData(user.GetSaveFormat(), user.username);
        }

        public void saveAllUsers()
        {
            foreach (string userName in dataBase.Keys)
            {
                this.cryptoFileSaver.WriteUserData(dataBase[userName].GetSaveFormat(), userName);
            }
            Console.WriteLine("SAVED USERS :)");
        }

        #endregion

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");

            lock (clients)
            {
                clients.Add(new ServerClient(tcpClient, this));
            }

            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal void Disconnect(ServerClient client)
        {
            lock (clients)
            {
                clients.Remove(client);
            }
            Console.WriteLine("Client disconnected");
        }

        internal User checkUser(string username, string password)
        {
            if (dataBase.ContainsKey(username))
                if (dataBase[username].checkPassword(password) && dataBase[username].loggedIn == false)
                {
                    dataBase[username].loggedIn = true;
                    return dataBase[username];
                }


            return null;
        }

        #region message methods

        internal void broadcast(string message)
        {
            foreach (ServerClient client in clients)
                client.sendMessage(message);
        }

        internal void SendToDoctors(string jsonMessage)
        {
            foreach (ServerClient client in clients)
            {
                if (client.user != null)
                    if (client.user.getRole() == Role.Doctor)
                        client.WriteTextMessage(jsonMessage);
            }

        }

        internal void SendToPatients(string jsonMessage)
        {
            foreach (ServerClient client in clients)
            {
                if (client.user != null)
                    if (client.user.getRole() == Role.Patient)
                        client.WriteTextMessage(jsonMessage);
            }

        }

        public void sendResistanceToOneClient(JObject data)
        {
            string resistance = (string)data["Resistance"];
            string username = (string)data["Username"];
            foreach (ServerClient client in clients)
            {

                if (client.user.getRole() == Role.Patient && client.user.getUsername() == username)
                {
                    client.sendResistance(resistance);
                }
            }
        }

        public void sendPrivMessage(JObject data)
        {
            string message = (string)data["Message"];
            string username = (string)data["Username"];
            foreach (ServerClient client in clients)
            {

                if (client.user.getRole() == Role.Patient && client.user.getUsername() == username)
                {
                    client.sendPrivMessage(message);
                }
            }
        }

        #endregion
    }
}