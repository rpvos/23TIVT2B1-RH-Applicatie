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
{    class TcpClientVR
    {
        private NetworkStream stream;
        private TcpClient tcpClient;
        private Dictionary<string, string> objects;
        private bool receiving;
        private string id, uuid, camera;
        private CallMethod callMethod;
        internal double speed { get; set; }
        internal double heartRate { get; set; }
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

        //Receive the response from the server, then do a specific thing with it
        private void GetJsonData(JObject json)
        {
            JObject tempdata = (JObject)json["data"];
            JObject data = (JObject)tempdata["data"];

            Console.WriteLine(data);

            //TODO make a switch case!
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

                    //TODO make this a switch case
                    if (name == "ground")
                    {
                        callMethod.AddTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/grass_diffuse.png", uuid, 0, 10, 1);
                    }
                    if (name == "tree")
                    {
                        callMethod.AddTexture("data/NetworkEngine/models/trees/fantasy/Tree_07.png", "", uuid, 0, 10, 1);
                    }
                    //If name equals to "panel", clear the panel for first use
                    if (name == "panel")
                    {
                        callMethod.ClearPanel(uuid);
                    }
                    if(name == "bike")
                    {
                        Console.WriteLine("ROUTE UUID TO FOLLOW: " + objects["route"]);
                        callMethod.FollowRoute(objects["route"], objects["bike"], 5, true, new int[] { 0, 0, 0 });
                        if(this.camera != null)
                        {
                            callMethod.FollowRoute(objects["route"], this.camera, 5, true, new int[] { 0, 0, 0 });
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error when adding node: {0}", (string)data["status"]);
                }
            }
            //Get scene, if scene found then reset the scene
            else if ((string)data["id"] == "scene/get")
            {
                 Console.WriteLine("GOT A SCENE");
                 Console.WriteLine("Get scene status: " + data["status"]);
                 //callMethod.ResetScene();

            }
            //If the scene is reset, find the groundplane node
            else if((string)data["id"] == "scene/reset")
            {
                callMethod.FindNode("GroundPlane");
            }
            //If a route is correctly added, add objects to the VR scene
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
                    }
                    catch (ArgumentException e)
                    {

                    }

                    //Console.WriteLine("Added route to dictionary\nName: {0}\nuuid: {1}", name, uuid);
                    callMethod.AddRoad("data/NetworkEngine/textures/tarmac_normal.png", "data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_specular.png", uuid);
                }
                else
                {
                    Console.WriteLine("Error when adding node: {0}", (string)data["status"]);
                }
            }
            else if ((string)data["id"] == "scene/node/update")
            {
                Console.WriteLine();
            }

            //If the textures are loaded correctly, set a new route
            else if ((string)data["id"] == "scene/node/addlayer")
            {
                if ((string)data["status"] == "ok")
                {
                    SetRoute();
                }
            }
            //If a road was added correctly, add random tree objects
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
                        //callMethod.AddObjectNode("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, new int[] { x, y, 0 }, new int[] { 0, 0, 0 }, false, "no");
                    }

                    callMethod.AddObjectNode("data/NetworkEngine/models/bike/bike.fbx", "bike", new int[] { 0, 100, 0 }, new int[] { 0, 0, 0 }, true, "data/NetworkEngine/models/bike/bike_anim.fbx");
                }
            }
            //If the panel was cleared correctly, then add text on the panel
            else if ((string)data["id"] == "scene/panel/clear")
            {
                Console.WriteLine("GOT THROUGH THE CLEARING PANEL RETURN STATUS");
                callMethod.SetText("Welcome", objects["panel"], new double[] { 20, 50 }, 30.0);

                string speedValue = this.speed.ToString();
                string heartRateValue = this.heartRate.ToString();
                string distanceTravelled = this.DT.ToString();
                string elapsedTime = this.elapsedTime.ToString();
                string resistance = this.resistance.ToString();

                callMethod.SetText("Speed in km/h: " + speedValue, objects["panel"], new double[] { 20, 100 }, 30.0);
                callMethod.SetText("Heart rate in bpm: " + heartRateValue, objects["panel"], new double[] { 20, 150 }, 30.0);
                callMethod.SetText("Distance travelled in meters: " + distanceTravelled, objects["panel"], new double[] { 20, 200 }, 30.0);
                callMethod.SetText("Elapsed time in seconds: " + elapsedTime, objects["panel"], new double[] { 20, 250 }, 30.0);
                callMethod.SetText("Resistance in %: " + resistance, objects["panel"], new double[] { 20, 300 }, 30.0);

                callMethod.SwapPanel(objects["panel"]);
            }
            //If the node was found correctly,and if it was the groundplane node, then delete it
            else if ((string)data["id"] == "scene/node/find")
            {
                if ((string)data["status"] == "ok")
                {
                    JArray nodeData = (JArray)data["data"];

                    uuid = nodeData[0]["uuid"].ToString();
                    string name = nodeData[0]["name"].ToString();
                    Console.WriteLine("UUID found: " + uuid);

                    if (name == "GroundPlane")
                    {
                        callMethod.DeleteNode(uuid);
                    }
                    if (name == "Camera")
                    {
                        this.camera = uuid;
                        
                    }
                }
            }        
        }

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
                //callMethod.SetTime(12);
                
                callMethod.AddGroundNode("ground", new int[] { -100, 0, -100 }, new int[] { 0, 0, 0 });    
                callMethod.AddTerrain();
                callMethod.AddPanelNode("panel", new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 1, 1 }, new int[] { 512, 512 }, new int[] { 1, 1, 1, 1 });
                callMethod.FindNode("Camera");
               
            }         
        }

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
            callMethod.NewRoutePoints(new int[] { 80, 0, 0 }, new int[] { 10, 0, 0 });
            callMethod.NewRoutePoints(new int[] { 80, 0, -80 }, new int[] { 0, 0, -10 });
            callMethod.NewRoutePoints(new int[] { 0, 0, -80 }, new int[] { 0, 0, 10 });

            callMethod.AddRoute();
        }
    }
}
