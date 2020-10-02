using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoctorServer
{
    public class Server
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
        }

        private List<ServerClient> clients;
        private List<ServerClient> doctors;

        public Server()
        {
            this.clients = new List<ServerClient>();
            this.doctors = new List<ServerClient>();

            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localhost, 1330);

            listener.Start();

            AcceptClients(listener);
        }

        public void AcceptClients(TcpListener listener)
        {
            Console.WriteLine("[Server]: Waiting for people to join...");

            //Thread saveThread = new Thread(saveChatLogs);
            //saveThread.Start();

            while (true)
            {

                TcpClient client = listener.AcceptTcpClient();
                StreamWriter streamWriter = new StreamWriter(client.GetStream(), Encoding.ASCII, -1, true);
                StreamReader streamReader = new StreamReader(client.GetStream(), Encoding.ASCII);

                checkLogin(streamReader, streamWriter,client);
 
            }

           
            
        }

        public void checkLogin(StreamReader reader, StreamWriter writer,TcpClient client)
        {
            String a = reader.ReadLine();
            ServerClient serverClient;
            Thread thread = new Thread(HandleClientThread);
            if (a == "DOCTOR")
            {
                serverClient = new ServerClient(client, a,writer,reader,this);
                Console.WriteLine("A Doctor joined.");

                this.doctors.Add(serverClient);
                thread.Start(serverClient);

            }
            else if(a == "CLIENT")
            {
                serverClient = new ServerClient(client, a,writer,reader,this);
                Console.WriteLine("A Client joined.");

                this.clients.Add(serverClient);
                thread.Start(serverClient);



            }


        }

        public List<ServerClient> getClients()
        {
            return this.clients;
        }

        public void HandleClientThread(object obj)
        {
            ServerClient client = obj as ServerClient;
            client.ReadTextMessage();
        }

        public void WriteToAllClients(string message)
        {
            foreach(ServerClient client in clients)
            {
                client.WriteTextMessage(message);
            }
        }

        public void WriteToOneClient(ServerClient client, string message)
        {
                client.WriteTextMessage(message);
        }

        public void WriteToAllDoctors(string message)
        {
            foreach(ServerClient client in this.doctors)
            {
                client.WriteTextMessage(message);
            }
        }

    }
}