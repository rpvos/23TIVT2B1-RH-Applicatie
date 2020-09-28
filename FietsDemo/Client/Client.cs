using FietsDemo;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerClient
{
    public class Client
    {

        private TcpClient server;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private Program mainProgram;


        public Client(Program program)
        {
            this.mainProgram = program;

            //Console.Write("Username: ");
            //this.username = Console.ReadLine();

            this.server = new TcpClient("127.0.0.1", 1330);

            this.streamWriter = new StreamWriter(server.GetStream(), Encoding.ASCII, 1, true);
            this.streamReader = new StreamReader(server.GetStream(), Encoding.ASCII);

            //Console.WriteLine("Type 'exit' to end connection");

            //WriteTextMessage(server, username);

            Thread receiveMessages = new Thread(ReadTextMessage);
            receiveMessages.Start();

            //while (true)
            //{
            //    string message = Console.ReadLine();
            //
            //    WriteTextMessage(message);
            //
            //    if (message.Equals("exit"))
            //    {
            //        server.Close();
            //        break;
            //    }
            //
            //}
        }

        public void WriteTextMessage(string message)
        {
            try
            {
                this.streamWriter.WriteLine(message);
                this.streamWriter.Flush();
            }
            catch { }

        }

        public void ReadTextMessage()
        {
            while (true)
            {
                try
                {
                    String a = this.streamReader.ReadLine();
                    int i = Int32.Parse(a);
                    this.mainProgram.setResistance(i);
                    this.mainProgram.setValuesInGui("resistance", i);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
