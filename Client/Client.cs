using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameClient
{
    class Client
    {
        static void Main(string[] args)
        {
            Client client = new Client();
        }

        private String username;
        private TcpClient server;
        private StreamWriter streamWriter;
        private StreamReader streamReader;


        public Client()
        {

            Console.Write("Username: ");
            this.username = Console.ReadLine();

            this.server = new TcpClient("127.0.0.1", 1330);

            this.streamWriter = new StreamWriter(server.GetStream(), Encoding.ASCII, -1, true);
            this.streamReader = new StreamReader(server.GetStream(), Encoding.ASCII);

            Console.WriteLine("Type 'exit' to end connection");

            WriteTextMessage(server, username);

            Thread receiveMessages = new Thread(ReadTextMessage);
            receiveMessages.Start();

            while (true)
            {
                string message = Console.ReadLine();

                WriteTextMessage(server, message);

                if (message.Equals("exit"))
                {
                    server.Close();
                    break;
                }

            }
        }

        public void WriteTextMessage(TcpClient client, string message)
        {
            try
            {
               
             this.streamWriter.WriteLine(message);
             this.streamWriter.Flush();
                
            }
            catch
            {

            }
        }

        public void ReadTextMessage()
        {
            while (true)
            {
                try
                {
                    
                 Console.WriteLine(this.streamReader.ReadLine());
                    
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
