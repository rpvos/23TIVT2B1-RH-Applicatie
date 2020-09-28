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

        static void Main(string[] args)
        {
            Server server = new Server();
        }

        public Server()
        {
            this.clients = new List<ServerClient>();
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(localhost, 8080);

            Console.WriteLine("Starting server");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.ReadLine();
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

    }
}
