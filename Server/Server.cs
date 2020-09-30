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
        private Dictionary<string, string> users;
        private List<int> sessionIDs;
        private Random random;


        static void Main(string[] args)
        {
            Server server = new Server();
        }

        public Server()
        {
            this.clients = new List<ServerClient>();
            this.users = new Dictionary<string, string>();
            fillUsers();
            this.sessionIDs = new List<int>();
            this.random = new Random();

            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(localhost, 8080);

            Console.WriteLine("Starting server");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.ReadLine();
        }

        private void fillUsers()
        {
            users.Add("admin", "admin");
        }

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new ServerClient(tcpClient, this));
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

        internal bool checkUser(string username, string password)
        {
            if (users.ContainsKey(username))
                if (users[username] == password)
                    return true;

            return false;
        }

        internal int getSessionID()
        {
            int r = this.random.Next();
            if (!sessionIDs.Contains(r))
            {
                sessionIDs.Add(r);
                return r;
            }
            else
            {
                return getSessionID();
            }

        }
    }
}
