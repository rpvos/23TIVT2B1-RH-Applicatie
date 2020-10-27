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
        private bool receiving, bikeAdded;
        private string id, nodeId;
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
            this.objects = new Dictionary<string, string>();
            this.tcpClient = new TcpClient(ip, port);
            this.stream = tcpClient.GetStream();
            this.callMethod = new CallMethod(this, objects);
            this.receiving = true;
            this.bikeAdded = false;
            this.heartRate = 50;
            SetTimer();            
            
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
            this.stream.Write(length, 0, length.Length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            this.stream.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Receive TunnelMessage
        //Receive bytes from a message
        public byte[] ReceiveBytes(int count)
        {
            byte[] buffer = new byte[count];
            int receivedBytes = 0;
            while (receivedBytes < count)
                receivedBytes += this.stream.Read(buffer, receivedBytes, count - receivedBytes);
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
                    //error prevention
                    if (data2 != null)
                    {
                        string name = (string)data2["name"];
                        string uuid = (string)data2["uuid"];
                        //Adds the object to the objects dictionary, and looks if an object already exists
                        try
                        {
                            this.objects.Add(name, uuid);
                        }
                        catch (ArgumentException e)
                        {
                            if (this.objects.ContainsValue(uuid))
                            {
                                Console.WriteLine("already exists");
                            }
                        }

                        Console.WriteLine("Added node to dictionary\nName: {0}\nuuid: {1}", name, uuid);
                        //adds textures based on name
                        switch (name)
                        {
                            case "ground":
                                //Adds 3 layers of groundTexture, grasses for the lower levels, rocks for higher levels
                                this.callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/terrain/grass_green_d.jpg", uuid, 0, 2, 1);
                                this.callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/terrain/grass_ground_d.jpg", uuid, 1, 3, 1);
                                this.callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/terrain/grass_rocky_d.jpg", uuid, 3, 7, 1);
                                break;

                            case "tree":
                                //Adds tree texture
                                this.callMethod.AddTexture("data/NetworkEngine/models/trees/fantasy/Tree_05.png", "", uuid, 0, 10, 1);
                                break;

                            case "panel":
                                //Clears the panel
                                this.callMethod.ClearPanel(uuid);
                                break;

                            case "bike":
                                //Scales the bike and lets the bike follow the route
                                this.callMethod.UpdateNode(objects["bike"], this.objects["bike"], new double[] { 0, 0, 0 }, 0.015, new double[] { 0, 0, 0 });
                                this.callMethod.FollowRoute(objects["route"], this.objects["bike"], 5, true, new double[] { 0, 0, 0 }, new int[] { 0, 0, 0 });

                                //Connects the camera and panel with the bike. Scales the objects correctly
                                if (this.objects.ContainsKey("panel") && this.objects.ContainsKey("camera"))
                                {
                                    this.callMethod.UpdateNode(this.objects["camera"], this.objects["bike"], new double[] { -660, -150, 0 }, 200, new double[] { 0, 90, 0 });
                                    this.callMethod.UpdateNode(this.objects["panel"], this.objects["camera"], new double[] { 1, 1.7, 1.9 }, 0.3, new double[] { 0, 0, 0 });
                                }
                                break;
                        }       
                    }           
                    break;

                //Prints the scene data
                case "scene/get":
                    Console.WriteLine("GOT A SCENE");
                    Console.WriteLine("Get scene status: " + data["status"]);
                    break;

                //Resets the scene
                case "scene/reset":
                    this.callMethod.FindNode("GroundPlane");
                    break;

                //Adds a route
                case "route/add":
                    JObject routeData = (JObject)data["data"];
                    string routeName = "route";
                    string routeId = (string)routeData["uuid"];
                    try
                    {
                        this.objects.Add(routeName, routeId);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }

                    this.callMethod.AddRoad("data/NetworkEngine/textures/tarmac_normal.png", "data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_specular.png", routeId);
                    break;

                case "scene/node/update":
                    break;

                case "scene/node/addlayer":
                    SetRoute();
                    break;

                case "scene/road/add":
                    //Adds the bike
                    //Prevention of adding multiple bikes.
                    if (!this.bikeAdded)
                    {
                        this.callMethod.AddObjectNode("data/NetworkEngine/models/bike/bike_anim.fbx", "bike", new int[] { 0, 100, 0 }, new int[] { 0, 0, 0 }, true, "Armature|Fietsen", 0.1);
                        this.bikeAdded = true;
                    }
                    break;

                case "scene/panel/clear":
                    //Updates the panel with the new values
                    this.callMethod.SetText("Welcome", this.objects["panel"], new double[] { 20, 50 }, 30.0);
                    this.callMethod.UpdatePanel(this.objects["panel"]);
                    break;

                case "scene/node/find":
                    //Finds a node
                    JArray nodeData = (JArray)data["data"];
                    this.nodeId = nodeData[0]["uuid"].ToString();
                    string nodeName = nodeData[0]["name"].ToString();
                    Console.WriteLine("UUID found: " + this.nodeId);
                    switch (nodeName)
                    {
                        //Finds the camera and adds it to the objects
                        case "Camera":
                            this.objects.Add("camera", this.nodeId);
                            break;

                        //Deletes the Head, GroundPlane, RightHand and LeftHand from the VR
                        case "Head":
                            Console.WriteLine("head deleted");
                            this.callMethod.DeleteNode(this.nodeId);
                            break;
                        case "GroundPlane":
                        case "RightHand":  
                        case "LeftHand":
                            this.callMethod.DeleteNode(this.nodeId);
                            break;
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
                var date = DateTime.Now;

                //Calls all the first methods
                this.callMethod.SetTime(date.Hour);
                this.callMethod.GetScene();
                this.callMethod.AddTerrain(40000);
                this.callMethod.AddGroundNode("ground", new int[] { -100, 0, -100 }, new int[] { 0, 0, 0 });
                this.callMethod.AddPanelNode("panel", new double[] { -1.5, 1.5, 0 }, new double[] { 0, 0, 0 }, new double[] { 1, 1 }, new int[] { 512 }, new int[] { 1, 1, 1, 1 });
                this.callMethod.FindNode("Camera");
                this.callMethod.FindNode("GroundPlane");
                this.callMethod.FindNode("LeftHand");
                this.callMethod.FindNode("RightHand");
                this.callMethod.FindNode("Head");

                //Adds trees on random positions
                Random random = new Random();
                for (int i = 0; i < 100; i++)
                {
                    int x = -100 + random.Next(200);
                    int y = -100 + random.Next(200);
                    int treeNumber = random.Next(11);
                    float scale = (float)(0.1 * random.Next(20));

                    //TODO fix height
                    Console.WriteLine(x + " " +  y);
                    this.callMethod.AddObjectNode("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, new int[] { x, 0, y }, new int[] { 0, 0, 0 }, false, "no", scale);
                }

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
                    SendTunnelRequest((string)data[i]["id"]); 
            }
        }

        //Set new routepoints for a route
        private void SetRoute()
        {
            //Makes the beautiful route
            callMethod.NewRoutePoints(new int[] { 0,  0, 0  }, new int[] {  8, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 20, 0, 0  }, new int[] { 10, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 30, 0,-15 }, new int[] { 10, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 50, 0,-15 }, new int[] { 10, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 70, 0, 10 }, new int[] {  0, 0, 10 });
            callMethod.NewRoutePoints(new int[] { 65, 0, 80 }, new int[] {-20, 0,-20 });
            callMethod.NewRoutePoints(new int[] { 40, 0, 60 }, new int[] {-10, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 20, 0, 70 }, new int[] {-10, 0, 0  });
            callMethod.NewRoutePoints(new int[] {  0, 0, 30 }, new int[] {-10, 0,-10 });
            callMethod.NewRoutePoints(new int[] {-50, 0, 70 }, new int[] {-10, 0,-10 });
            callMethod.NewRoutePoints(new int[] {-70, 0, 30 }, new int[] { 3,  0,-5  });
            callMethod.NewRoutePoints(new int[] {-50, 0, 0  }, new int[] {-4,  0,-5  });
            callMethod.NewRoutePoints(new int[] {-40, 0,-60 }, new int[] { 10, 0, 8  });
            callMethod.NewRoutePoints(new int[] { 60, 0,-60 }, new int[] { 10, 0, 8  });
            callMethod.NewRoutePoints(new int[] { 65, 0,-40 }, new int[] {-10, 0, 0  });
            callMethod.NewRoutePoints(new int[] { 30, 0,-30 }, new int[] { 20, 0, 20 });
            callMethod.NewRoutePoints(new int[] {-20, 0,-15 }, new int[] { 20, 0, 20 });

            callMethod.AddRoute();
        }
        #endregion

        #region Timer
        //Sets a Timer for the panel
        private void SetTimer()
        {
            this.panelUpdateTimer = new System.Timers.Timer(500);
            this.panelUpdateTimer.Elapsed += OnTimedEvent;
            this.panelUpdateTimer.AutoReset = true;
            this.panelUpdateTimer.Enabled = true;
        }

        //Updates the panel when expired.
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (this.objects.ContainsKey("panel"))
                this.callMethod.ClearPanel(this.objects["panel"]);
        }
        #endregion
    }
}
