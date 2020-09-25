using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using simulatie;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace TCP_naar_VR
{
    class TcpClientVR
    {
        private NetworkStream stream;
        private TcpClient tcpClient;
        private Dictionary<string, string> objects;
        private bool receiving;
        private string id;
        private CallMethod callMethod;
        
        public TcpClientVR(string ip, int port)
        {
            objects = new Dictionary<string, string>();
            tcpClient = new TcpClient(ip, port);
            stream = tcpClient.GetStream();
            callMethod = new CallMethod(this, objects);
            receiving = true;
            Thread receivingTCPDataThread = new Thread(new ThreadStart(Receive));
            receivingTCPDataThread.Start();
        }
        public void SendKickOff()
        {
            dynamic data = new
            {
                id = "session/list"
            };
            string jsonString = JsonConvert.SerializeObject(data);
            SendMessage(jsonString);
        }
        public void SendTunnelRequest(string id)
        {
            dynamic data = new
            {
                id = "tunnel/create",
                data = new
                {
                    session = id,
                    key = ""
                }
            };
            string jsonString = JsonConvert.SerializeObject(data);
            SendMessage(jsonString);
        }

        internal TunnelMessage GetTunnelMessage(string jsonName)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string pathFile = currentPath + @"\Json files\" + jsonName;
            JObject message = JObject.Parse(File.ReadAllText(pathFile));
            TunnelMessage tunnelMessage = new TunnelMessage(message, id);
            return tunnelMessage;
        }

        internal void SendMessage(string message)
        {
            
            byte[] length = BitConverter.GetBytes(message.Length);
            stream.Write(length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer);
        }

        public byte[] ReceiveBytes(int count)
        {
            byte[] buffer = new byte[count];
            int receivedBytes = 0;
            while(receivedBytes < count)
                receivedBytes += stream.Read(buffer, receivedBytes, count-receivedBytes);
            return buffer;
        }

        public void Receive()
        {
            while (receiving)
            {
                byte[] lenghtBuffer = ReceiveBytes(4);
                int length = BitConverter.ToInt32(lenghtBuffer);
                var buffer = ReceiveBytes(length);
                string jsonS = Encoding.ASCII.GetString(buffer);
                JObject json = JObject.Parse(jsonS);

                ReceiveMessage(json);
            }
        }

        private void ReceiveMessage(JObject json)
        {
            string id = (string)json["id"];

            if (id == "session/list")
            {
                PrintUsers(json);
            }
            else if (id == "tunnel/create")
            {               
                CheckTunnelStatus(json);
            }
            else if (id == "tunnel/send")
            {
                ReceiveNodeNameAndUuid(json);
            }
        }

        

        private void ReceiveNodeNameAndUuid(JObject json)
        {
            JObject tempdata = (JObject)json["data"];
            JObject data = (JObject)tempdata["data"];

            if ((string)data["id"] == "scene/node/add")
            {
                if ((string)data["status"] == "ok")
                {
                    JObject data2 = (JObject)data["data"];
                    string name = (string)data2["name"];
                    string uuid = (string)data2["uuid"];
                    try
                    {
                        objects.Add(name, uuid);
                    } catch (ArgumentException e)
                    {
                        
                    }
                    
                    Console.WriteLine("Added node to dictionary\nName: {0}\nuuid: {1}", name, uuid);

                    //TEMP
                    if (name == "ground")
                    {
                        callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/grass_diffuse.png", uuid);
                    }
                    if (name == "tree")
                    {
                        callMethod.AddTexture("data/NetworkEngine/models/trees/fantasy/Tree_07.png", "", uuid);
                    }

                    callMethod.NewRoutePoints(new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 });
                    callMethod.NewRoutePoints(new int[] { 0, 0, 0 }, new int[] { 10, 0, -10 });
                    callMethod.NewRoutePoints(new int[] { 30, 0, 0 }, new int[] { 20, 0, 5 });
                    callMethod.NewRoutePoints(new int[] { 0, 0, 15 }, new int[] { -15, 0, -10 });
                    callMethod.NewRoutePoints(new int[] { 7, 0, 0 }, new int[] { 8, 0, -5 });
                    callMethod.NewRoutePoints(new int[] { 0, 0, 20 }, new int[] { 13, 0, 25 });

                    callMethod.AddRoute();

                                     
                }
                else
                {
                    Console.WriteLine("Error when adding node: {0}", (string)data["status"]);
                }
            }
            else if ((string)data["id"] == "route/add")
            {
                if ((string)data["status"] == "ok")
                {
                    JObject data2 = (JObject)data["data"];
                    string name = "route";
                    string uuid = (string)data2["uuid"];
                    try
                    {
                        objects.Add(name, uuid);
                    } catch (ArgumentException e)
                    {
                        
                    }
                    
                    Console.WriteLine("Added route to dictionary\nName: {0}\nuuid: {1}", name, uuid);
                    callMethod.AddRoad("data/NetworkEngine/textures/tarmac_normal.png", "data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_specular.png", uuid);
                }
                else
                {
                    Console.WriteLine("Error when adding node: {0}", (string)data["status"]);
                }
            }
            else if ((string)data["id"] == "scene/node/addlayer")
            {
                if ((string)data["status"] == "ok")
                {
                    SendMessage(GetTunnelMessage("RouteSetMessage.json").ToString());
                }
            } else if((string)data["id"] == "scene/road/add")
            {
                if ((string)data["status"] == "ok")
                {
                    Random random = new Random();
                    for(int i = 0; i < 10; i++)
                    {
                        int x = -100 + random.Next(200);
                        int y = -100 + random.Next(200);
                        int treeNumber = random.Next(11);
                        float scale = (float)(0.1 * random.Next(20));
                        callMethod.AddObject("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, x, y, 0, scale);
                    }
                }
            }
        }

        private void CheckTunnelStatus(JObject json)
        {
            JObject data = (JObject)json["data"];
            string status = (string)data["status"];

            string id = (string)data["id"];

            if (status == "ok")
            {
                this.id = id;
                //callMethod.SetTime(12);
                callMethod.AddTerrain();
                callMethod.AddNode();

                //followRoute("");
            }

            Console.WriteLine("Status for tunnel: {0}\nid: {1}", status, id);
        }

        private void PrintUsers(JObject json)
        {
            JArray data = (JArray)json["data"];
            Console.WriteLine("USERS:");

            for (int i = 0; i < data.Count; i++)
            {
                JObject clientInfo = (JObject)data[i]["clientinfo"];
                Console.WriteLine(clientInfo);

                if ((string)clientInfo["host"] == Environment.MachineName)
                {
                    SendTunnelRequest((string)data[i]["id"]);
                }
            }
        }       
    }
}
