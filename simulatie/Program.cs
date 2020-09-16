using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_naar_VR
{
    class TcpClientVR
    {
        private NetworkStream stream;
        private TcpClient tcpClient;
        private bool receiving;
        public TcpClientVR(string ip, int port)
        {
            tcpClient = new TcpClient(ip, port);
            stream = tcpClient.GetStream();

            receiving = true;
            Thread receivingTCPDataThread = new Thread(new ThreadStart(receive));
            receivingTCPDataThread.Start();
        }
        public void sendKickOff()
        {
            string jsonS = "{\"id\" : \"session/list\"}";
            sendMessage(jsonS);
        }
        public void sendTunnelRequest()
        {
            string jsonS = "{\"id\" : \"tunnel/create\", \"data\" : {\"session\" : \"ce55ab4b-2743-4bf0-83fe-1cf0cfe02b03\", \"key\" : \"keeskroketmetzonnetje\"}}";
            sendMessage(jsonS);
        }

        private void sendMessage(string message)
        {
            byte[] length = BitConverter.GetBytes(message.Length);
            stream.Write(length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer);
        }

        public void receive()
        {
            while (receiving)
            {
                byte[] lenghtBuffer = new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    lenghtBuffer[i] = (byte)stream.ReadByte();
                }

                int length = BitConverter.ToInt32(lenghtBuffer);


                Console.WriteLine("Length: {0}", length);

                var buffer = new List<byte>();

                for (int i = 0; i < length; i++)
                {
                    buffer.Add((byte)stream.ReadByte());
                }

                string jsonS = Encoding.ASCII.GetString(buffer.ToArray());

                JObject json = JObject.Parse(jsonS);

                string id = (string)json["id"];

                if (id == "session/list")
                {
                    printUsers(json);
                }

                if (id == "tunnel/create")
                {
                    checkTunnelStatus(json);
                }
            }
        }

        private void checkTunnelStatus(JObject json)
        {
            JObject data = (JObject)json["data"];
            string status = (string)data["status"];
            string id = (string)data["id"];
            Console.WriteLine("Status for tunnel: {0}\nid: {1}", status, id);
        }

        private void printUsers(JObject json)
        {
            JArray data = (JArray)json["data"];
            Console.WriteLine("USERS:");

            for (int i = 0; i < data.Count; i++)
            {
                JObject clientInfo = (JObject)data[i]["clientinfo"];
                Console.WriteLine(clientInfo);
            }
        }
    }
}
