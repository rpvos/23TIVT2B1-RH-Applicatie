﻿using System;
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
        private List<ServerClient> doctors;

        private TcpListener listener;
        private Dictionary<string, string> clientDataBase;
        private Dictionary<string, string> doktorDataBase;


        static void Main(string[] args)
        {
            Server server = new Server();
        }

        public Server()
        {
            this.clients = new List<ServerClient>();
            this.doctors = new List<ServerClient>();

            this.clientDataBase = new Dictionary<string, string>();
            this.doktorDataBase = new Dictionary<string, string>();
            fillUsers();

            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            this.listener = new TcpListener(localhost, 8080);

            Console.WriteLine("Starting server");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.ReadLine();
        }

        private void fillUsers()
        {
            clientDataBase.Add("admin", "admin");
            doktorDataBase.Add("admin", "admin");
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

        //todo check if it is a doktor or patient
        internal bool checkUser(string username, string password)
        {
            if (clientDataBase.ContainsKey(username))
                if (clientDataBase[username] == password)
                    return true;

            return false;
        }

        internal void broadcast(string message)
        {
            foreach (ServerClient client in clients)
                client.sendMessage(message);
        }
    }
}
