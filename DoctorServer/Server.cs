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
    class Server
    {

        private List<ServerClient> clients;
        private List<String> chatlogs;
        private DoctorServer doctorServer;
        private int amountOfClients;

        public Server(DoctorServer doctorServer)
        {
            this.amountOfClients = 0;
            this.doctorServer = doctorServer;
            this.chatlogs = new List<string>();
            this.clients = new List<ServerClient>();
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localhost, 1330);

            listener.Start();

            AcceptClients(listener);
        }

        public void AcceptClients(TcpListener listener)
        {
            Console.WriteLine("[Server]: Waiting for people to join...");
            Console.WriteLine("Type 'Save' to save chatlogs");

            Thread saveThread = new Thread(saveChatLogs);
            saveThread.Start();

            while (true)
            {

                TcpClient client = listener.AcceptTcpClient();
                ServerClient serverClient = new ServerClient(client,"Bike "+amountOfClients);
                this.clients.Add(serverClient);
                this.doctorServer.addClient(serverClient);
                amountOfClients++;


                Thread thread = new Thread(HandleClientThread);
                thread.Start(serverClient);
            }

           
            
        }

        public List<ServerClient> getClients()
        {
            return this.clients;
        }

        public void saveChatLogs()
        {
            String path = System.Environment.CurrentDirectory + "\\chatlogs.txt";
            while (true)
            {
                String a = Console.ReadLine();
                if (a == "Save")
                {
                    File.WriteAllLines(path, this.chatlogs);
                }
            }
        }

        public void HandleClientThread(object obj)
        {
            ServerClient client = obj as ServerClient;
            client.ReadTextMessage(this.doctorServer);


          
        }

    }
}
