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
        private int ID;


        public Client(Program program, int ID)
        {
            this.ID = ID;
            this.mainProgram = program;


            this.server = new TcpClient("127.0.0.1", 1330);

            this.streamWriter = new StreamWriter(server.GetStream(), Encoding.ASCII, 1, true);
            this.streamReader = new StreamReader(server.GetStream(), Encoding.ASCII);
            WriteTextMessageToServer("CLIENT");



            Thread receiveMessages = new Thread(ReadTextMessageFromServer);
            receiveMessages.Start();

        }

        public void WriteTextMessageToServer(string message)
        {
            try
            {
                this.streamWriter.WriteLine(this.ID +""+message);
                this.streamWriter.Flush();
            }
            catch { }

        }

        public void ReadTextMessageFromServer()
        {
            while (true)
            {
                try
                {
                    String a = this.streamReader.ReadLine();
                    Console.WriteLine(a);
                    if(a.Substring(0,4) == "RSTE")
                    {
                        double resistance = Int32.Parse(a.Substring(9));
                        mainProgram.setResistance(resistance);
                        mainProgram.setValuesInGui("resistance", resistance);
                    }
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
