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
    class Server
    {

        private List<ServerClient> clients;

        private TcpListener listener;
        private Dictionary<string, User> dataBase;


        static void Main(string[] args)
        {
            Server server = new Server();
        }

        public Server()
        {
            this.clients = new List<ServerClient>();

            this.dataBase = new Dictionary<string, User>();
            fillUsers();

            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(localhost, 8080);

            Console.WriteLine("Starting server");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.ReadKey();
        }


        private void fillUsers()
        {
            dataBase.Add("stoeptegel", new User("Stijn", "stoeptegel", "123", Role.Patient));
            dataBase.Add("aardappel", new User("Piet", "aardappel", "321", Role.Patient));

            dataBase.Add("dokter", new User("dokter", "dokter", "123", Role.Doctor));
        }

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
            foreach(ServerClient client in clients)
            {

                if(client.user.getRole() == Role.Patient && client.user.getUsername() == username)
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
                    Console.WriteLine("LETS GOOO");
                    client.sendPrivMessage(message);
                }
            }
        }
    }
}
