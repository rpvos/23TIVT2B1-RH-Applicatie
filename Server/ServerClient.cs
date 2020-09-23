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
        private String username;
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public ServerClient(TcpClient client)
        {
            this.client = client;
            this.streamWriter = new StreamWriter(client.GetStream(), Encoding.ASCII, -1, true);
            this.streamReader = new StreamReader(client.GetStream(), Encoding.ASCII);
        }

        public void setUsername(String username)
        {
            this.username = username;
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