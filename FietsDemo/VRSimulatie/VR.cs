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
using System.Timers;

namespace TCP_naar_VR
{    class TcpClientVR
    {
        #region Variables
        private NetworkStream stream;
        private TcpClient tcpClient;
        private Dictionary<string, string> objects;
        private bool receiving;
        private string id, nodeId, camera;
        private double time;
        private System.Timers.Timer panelUpdateTimer;
        private CallMethod callMethod;
        internal double speed { get; set; }
        internal double heartRate { get; set; }
        internal double AP { get; set; }
        internal double DT { get; set; }
        internal double elapsedTime { get; set; }
        internal double resistance { get; set; }
        #endregion

        public TcpClientVR(string ip, int port)
        {
            objects = new Dictionary<string, string>();
            tcpClient = new TcpClient(ip, port);
            stream = tcpClient.GetStream();
            callMethod = new CallMethod(this, objects);
            receiving = true;
            SetTimer();
            this.time = 12;
            Thread receivingTCPDataThread = new Thread(new ThreadStart(Receive));
            receivingTCPDataThread.Start();
        }

        #region KickOff
        //Send a starting message to the server for connection
        public void SendKickOff()
        {
            dynamic data = new
            {
                id = "session/list"
            };
            string jsonString = JsonConvert.SerializeObject(data);
            SendMessage(jsonString);
        }
        #endregion

        #region Send TunnelMessage
        //Send a new tunnel request
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

        //Get the text from the specified JSON file
        internal TunnelMessage GetTunnelMessage(string jsonName)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string pathFile = currentPath + @"\Json files\" + jsonName;
            JObject message = JObject.Parse(File.ReadAllText(pathFile));
            TunnelMessage tunnelMessage = new TunnelMessage(message, id);
            return tunnelMessage;
        }

        //Send a message to the server
        internal void SendMessage(string message)
        {

            byte[] length = BitConverter.GetBytes(message.Length);
            stream.Write(length, 0, length.Length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Receive TunnelMessage
        //Receive bytes from a message
        public byte[] ReceiveBytes(int count)
        {
            byte[] buffer = new byte[count];
            int receivedBytes = 0;
            while (receivedBytes < count)
                receivedBytes += stream.Read(buffer, receivedBytes, count - receivedBytes);
            return buffer;
        }

        //Check the receiving message
        public void Receive()
        {
            while (receiving)
            {
                byte[] lenghtBuffer = ReceiveBytes(4);
                int length = BitConverter.ToInt32(lenghtBuffer, 0);
                var buffer = ReceiveBytes(length);
                string jsonS = Encoding.ASCII.GetString(buffer);
                JObject json = JObject.Parse(jsonS);

                ReceiveMessage(json);               
            }
        }

        //Listen to the response from the server
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
                GetJsonData(json);
            }
        }
        #endregion

        #region GetJsonData
        //Receive the response from the server, then do a specific thing with it
        private void GetJsonData(JObject json)
        {
            JObject tempdata = (JObject)json["data"];
            JObject data = (JObject)tempdata["data"];

            Console.WriteLine(data);
            switch ((string)data["id"])
            {
                case "scene/node/add":
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
                    switch (name)
                    {
                        case "ground":
                            callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/grass_diffuse.png", uuid, 0, 10, 1);
                            break;

                        case "tree":
                            callMethod.AddTexture("data/NetworkEngine/models/trees/fantasy/Tree_07.png", "", uuid, 0, 10, 1);
                            break;

                        case "panel":
                            callMethod.ClearPanel(uuid);
                            break;

                        case "bike":
                            Console.WriteLine("ROUTE UUID TO FOLLOW: " + objects["route"]);
                            callMethod.FollowRoute(objects["route"], objects["bike"], 5, true, new double[] { 0, 0, 0 }, new int[] { 0, 0, 0 });

                            if (this.camera != null)
                            {
                                callMethod.UpdateNode(this.camera, objects["bike"], new double[] { -2.8, 0, 0 }, 1.0, new double[] { 0, 90, 0 });
                                //callMethod.FollowRoute(objects["route"], this.camera, 5, true, new double[] { 0, 7.84, 0 }, new int[] { 0, 0, 0 });

                            }
                            if (objects.ContainsKey("panel"))
                            {
                                callMethod.UpdateNode(objects["panel"], this.camera, new double[] { 1.5, 1.5, 1 }, 0.5, new double[] { 0, 0, 0 });
                                //callMethod.FollowRoute(objects["route"], objects["panel"], 5, true, new double[] { 0, 7.84, 0 }, new int[] { 0, 0, 0 });
                            }
                            break;
                    }           
                    break;

                case "scene/get":
                    Console.WriteLine("GOT A SCENE");
                    Console.WriteLine("Get scene status: " + data["status"]);
                    //callMethod.ResetScene();
                    break;

                case "scene/reset":
                    callMethod.FindNode("GroundPlane");
                    break;

                case "route/add":
                    JObject routeData = (JObject)data["data"];
                    string routeName = "route";
                    string routeId = (string)routeData["uuid"];
                    try
                    {
                        objects.Add(routeName, routeId);
                    }
                    catch (ArgumentException e)
                    {

                    }

                    //Console.WriteLine("Added route to dictionary\nName: {0}\nuuid: {1}", name, uuid);
                    callMethod.AddRoad("data/NetworkEngine/textures/tarmac_normal.png", "data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_specular.png", routeId);
                    break;

                case "scene/node/update":
                    Console.WriteLine();
                    break;

                case "scene/node/addlayer":
                    SetRoute();
                    break;

                case "scene/road/add":
                    //Add 10 trees on random positions
                    Random random = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        int x = -100 + random.Next(200);
                        int y = -100 + random.Next(200);
                        int treeNumber = random.Next(11);
                        float scale = (float)(0.1 * random.Next(20));
                        //callMethod.AddObjectNode("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, new int[] { x, y, 0 }, new int[] { 0, 0, 0 }, false, "no");
                    }

                    callMethod.AddObjectNode("data/NetworkEngine/models/bike/bike.fbx", "bike", new int[] { 0, 100, 0 }, new int[] { 0, 0, 0 }, true, "data/NetworkEngine/models/bike/bike_anim.fbx");
                    break;

                case "scene/panel/clear":
                    Console.WriteLine("GOT THROUGH THE CLEARING PANEL RETURN STATUS");
                    callMethod.SetText("Welcome", objects["panel"], new double[] { 20, 50 }, 30.0);
                    callMethod.UpdatePanel(objects["panel"]);
                    break;

                case "scene/node/find":
                    JArray nodeData = (JArray)data["data"];

                    nodeId = nodeData[0]["uuid"].ToString();
                    string nodeName = nodeData[0]["name"].ToString();
                    Console.WriteLine("UUID found: " + nodeId);

                    if (nodeName == "GroundPlane")
                    {
                        callMethod.DeleteNode(nodeId);
                    }
                    if (nodeName == "Camera")
                    {
                        this.camera = nodeId;
                    }
                    break;
            }  
        }
        #endregion

        #region Call first methods
        //Check for the tunnelstatus
        private void CheckTunnelStatus(JObject json)
        {
            JObject data = (JObject)json["data"];
            string status = (string)data["status"];

            string id = (string)data["id"];

            if (status == "ok")
            {
                Console.WriteLine("Status for tunnel: {0}\nid: {1}", status, id);
                this.id = id;
                //callMethod.GetScene();
                //
               
                callMethod.AddGroundNode("ground", new int[] { -100, 0, -100 }, new int[] { 0, 0, 0 });    
                callMethod.AddTerrain();
                callMethod.AddPanelNode("panel", new double[] { -1.5, 1.5, 0 }, new double[] { 0, 0, 0 }, new double[] { 1, 1 }, new int[] { 512 }, new int[] { 1, 1, 1, 1 });
                callMethod.FindNode("Camera");
               
            }         
        }
        #endregion

        #region Print users and set first route
        //Print the users who are currently on the server
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

        //Set new routepoints for a new, non-existing route
        private void SetRoute()
        {
            callMethod.NewRoutePoints(new int[] { 0, 0, 0 }, new int[] { 10, 0, 0 });
            callMethod.NewRoutePoints(new int[] { 20, 0, 0 }, new int[] { 10, 0, 0 });
            callMethod.NewRoutePoints(new int[] { 20, 0, -20 }, new int[] { 0, 0, -10 });
            callMethod.NewRoutePoints(new int[] { 0, 0, -20 }, new int[] { 0, 0, 10 });

            callMethod.AddRoute();
        }
        #endregion

        #region Timer
        private void SetTimer()
        {
            panelUpdateTimer = new System.Timers.Timer(100);
            panelUpdateTimer.Elapsed += OnTimedEvent;
            panelUpdateTimer.AutoReset = true;
            panelUpdateTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (objects.ContainsKey("panel"))
            {
                callMethod.ClearPanel(objects["panel"]);

            }
            callMethod.SetTime(this.time);
            this.time += (elapsedTime / 10);
            this.DT += 2.5;
            this.elapsedTime += 0.5;
        }
        #endregion
    }
}
