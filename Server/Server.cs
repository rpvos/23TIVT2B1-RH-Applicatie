using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {

        private List<ServerClient> clients;

        static void Main(string[] args)
        {
            Server server = new Server();
        }

        public Server()
        {
            this.clients = new List<ServerClient>();
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localhost, 1330);

            listener.Start();

            AcceptClients(listener);
        }

        public void AcceptClients(TcpListener listener)
        {
            Console.WriteLine("[Server]: Waiting for people to join...");

            while (true)
            {

                TcpClient client = listener.AcceptTcpClient();
                ServerClient serverClient = new ServerClient(client);
                this.clients.Add(serverClient);


                Thread thread = new Thread(HandleClientThread);
                thread.Start(serverClient);
            }
        }

        public void HandleClientThread(object obj)
        {
            ServerClient client = obj as ServerClient;

            string username = client.ReadTextMessage();
            Console.WriteLine("[Server]: " + username + " joined");
            client.setUsername(username);

            foreach (ServerClient clnt in this.clients)
            {
                clnt.WriteTextMessage("[Server]: " + username + " joined");
            }

                while (true)
                {
                string received = client.ReadTextMessage();
                Console.WriteLine("[" + username + "]: " + received);

                foreach(ServerClient clnt in this.clients)
                {
                    if(clnt != client)
                    {
                        clnt.WriteTextMessage("[" + username + "]: " + received);
                    }
                }

                if (received.Equals("exit"))
                {
                    client.WriteTextMessage("[Server]: Bye Bye, see you next time :)");
                    break;
                }
                


            }
            client.GetTcpClient().Close();
            Console.WriteLine("[Server]: "+username + " left the game.");
        }

    }
}
       