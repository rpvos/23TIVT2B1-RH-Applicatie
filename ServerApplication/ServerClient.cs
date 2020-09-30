using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace DoctorServer
{
    public class ServerClient
    {

        private TcpClient client;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private Server server;
        public int resistance { set; get; }
        public String username { get; set; }

        public ServerClient(TcpClient client, String username, StreamWriter writer, StreamReader reader, Server server)
        {
            this.server = server;
            this.resistance = 0;
            this.username = username;
            this.client = client;
            this.streamWriter = writer;
            this.streamReader = reader;
        }


        public TcpClient GetTcpClient()
        {
            return this.client;
        }

  

        public void WriteTextMessage(string message)
        {
            try
            {
                streamWriter.WriteLine(message);
                streamWriter.Flush();
            }
            catch { }
        }

        public void ReadTextMessage()
        {
            while (true)
            {
                try
                {
                    String a = streamReader.ReadLine();

                    if (a.Substring(5, 4) == "RSTE")
                    {
                        server.WriteToOneClient(a.Substring(5));
                    }
                    else
                    {
                        this.server.WriteToAllDoctors(a);

                    }

                }
                catch
                {
                    client.Close();
                }
            }

        }

        public override string ToString()
        {
            return this.username;
        }
    }
}
