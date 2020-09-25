using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
    internal class ServerClient
    {

        private TcpClient client;
        private string bike;
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public ServerClient(TcpClient client, string bike)
        {
            this.client = client;
            this.bike = bike;
            this.streamWriter = new StreamWriter(client.GetStream(), Encoding.ASCII, -1, true);
            this.streamReader = new StreamReader(client.GetStream(), Encoding.ASCII);
        }

        public TcpClient GetTcpClient()
        {
            return this.client;
        }

        public string getBike()
        {
            return this.bike;
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

        public string ReadTextMessage()
        {
            try
            {
                return streamReader.ReadLine();
            }
            catch
            {
                return "exit";
            }

        }

    } 
}