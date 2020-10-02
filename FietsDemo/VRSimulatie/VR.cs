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
        internal double speed { get; set; }
        internal double heartRate{ get; set; }
        internal double AP { get; set; }
        internal double DT { get; set; }
        internal double elapsedTime { get; set; }
        internal double resistance { get; set; }

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
            stream.Write(length,0,length.Length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer,0,buffer.Length);
        }

        public byte[] ReceiveBytes(int count)
        {
            byte[] buffer = new byte[count];
            int receivedBytes = 0;
            while (receivedBytes < count)
                receivedBytes += stream.Read(buffer, receivedBytes, count - receivedBytes);
            return buffer;
        }

        public void Receive()
        {
            while (receiving)
            {
                byte[] lenghtBuffer = ReceiveBytes(4);
                int length = BitConverter.ToInt32(lenghtBuffer,0);
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

            Console.WriteLine(data);
           

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
                    }
                    catch (ArgumentException e)
                    {
                        if (objects.ContainsValue(uuid))
                        {
                            Console.WriteLine("already exists");
                        }
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
                    //If name equals to "panel", clear the panel for first use
                    if (name == "panel")
                    {
                        callMethod.ClearPanel(uuid);
                    }
                }
                else
                {
                    Console.WriteLine("Error when adding node: {0}", (string)data["status"]);
                }
            }
            //After finding a specific node, delete this one
            //else if ((string)data["id"] == "scene/node/find")
            //{
            //    Console.WriteLine("FOUND NODE: " + (string) data["name"]);
            //}

            //Get scene, if scene found then delete groundplane
            else if ((string)data["id"] == "scene/get")
            {
                if((string)data["status"] == "ok")
                {
                    Console.WriteLine("GOT A SCENE");
                    callMethod.DeleteGroundPlane();
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
                        callMethod.NewRoutePoints(new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 });
                        callMethod.NewRoutePoints(new int[] { 0, 0, 0 }, new int[] { 10, 0, -10 });
                        callMethod.NewRoutePoints(new int[] { 30, 0, 0 }, new int[] { 20, 0, 5 });
                        callMethod.NewRoutePoints(new int[] { 0, 0, 15 }, new int[] { -15, 0, -10 });
                        callMethod.NewRoutePoints(new int[] { 7, 0, 0 }, new int[] { 8, 0, -5 });
                        callMethod.NewRoutePoints(new int[] { 0, 0, 20 }, new int[] { 13, 0, 25 });

                        callMethod.AddRoute();
                    }
                    catch (ArgumentException e)
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
            }
            else if ((string)data["id"] == "scene/road/add")
            {
                if ((string)data["status"] == "ok")
                {
                    //Add 10 trees on random positions
                    Random random = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        int x = -100 + random.Next(200);
                        int y = -100 + random.Next(200);
                        int treeNumber = random.Next(11);
                        float scale = (float)(0.1 * random.Next(20));
                        callMethod.AddObject("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, x, y, 0, scale);
                    }
                }
            }
            //After clearing the panel, call the setMethod to initialize it
            else if ((string)data["id"] == "scene/panel/clear")
            {
                Console.WriteLine("GOT THROUGH THE CLEARING PANEL RETURN STATUS");
                callMethod.SetText("Welcome", objects["panel"], new double[] { 0, 0 });

                string speedValue = this.speed.ToString();
                string heartRateValue = this.heartRate.ToString();
                string distanceTravelled = this.DT.ToString();
                string elapsedTime = this.elapsedTime.ToString();
                string resistance = this.resistance.ToString();

                callMethod.SetText(speedValue, objects["panel"], new double[] { 0, 20 });
                callMethod.SetText(heartRateValue, objects["panel"], new double[] { 0, 40 });
                callMethod.SetText(distanceTravelled, objects["panel"], new double[] { 0, 60 });
                callMethod.SetText(elapsedTime, objects["panel"], new double[] { 0, 80 });
                callMethod.SetText(resistance, objects["panel"], new double[] { 0, 100 });

                //foreach (var s in objects)
                //{
                //    if (s.Key == "panel")
                //    {
                //        Console.WriteLine("GOT THROUGH PANELCLEAR METHOD");
                //        Console.WriteLine("FIRST CALL OF SETTEXT METHOD");
                //        callMethod.SetText("Welcome", s.Value, new double[] { 0, 0 });
                //    }
                //}
            }
            //After getting the response of the drawText, add the values of the ebike
            else if((string)data["id"] == "scene/panel/drawtext")
            {               
                string status = (string)data["status"];
                Console.WriteLine("Status for set text: {0}", status);
                if((string)data["status"] == "ok")
                {
                    callMethod.SwapPanel(objects["panel"]);
                   
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
                Console.WriteLine("Status for tunnel: {0}\nid: {1}", status, id);
                this.id = id;
                callMethod.GetScene();
                //callMethod.FindNode("GroundPlane");
                //callMethod.SetTime(12);
                //callMethod.AddTerrain();
                //callMethod.AddNode();
                //callMethod.AddUniversalNode("panel", new int[] { 0, 0, 0 }, new int[] { 0, 0, 0});
                //followRoute("");
            }

           
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
