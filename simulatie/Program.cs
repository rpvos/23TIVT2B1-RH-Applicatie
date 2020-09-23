﻿using Newtonsoft.Json.Linq;
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
        private Dictionary<string, string> objects;
        private bool receiving;
        private string id;
        public TcpClientVR(string ip, int port)
        {
            objects = new Dictionary<string, string>();
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
        public void sendTunnelRequest(string id)
        {
            string jsonS = "{\"id\" : \"tunnel/create\", \"data\" : {\"session\" : \"" + id + "\", \"key\" : \"\"}}";
            sendMessage(jsonS);
        }

        private void setTime(int time)
        {
            TunnelMessage timeMessage = GetTunnelMessage("TimeSetMessage.json");
            timeMessage.getDataContent()["time"] = time;

            sendMessage(timeMessage.ToString());
        }

        private void addNode()
        {
            TunnelMessage timeMessage = GetTunnelMessage("NodeAdd.json");

            sendMessage(timeMessage.ToString());
        }

        private void addObject(string fileNameModel, string objectName, int x, int y, int z, float scale)
        {
            TunnelMessage addObjectMessage = GetTunnelMessage("AddObjectMessage.json");
            JObject data = addObjectMessage.getDataContent();
            data["name"] = objectName;

            JObject components = (JObject)data["components"];

            JObject transform = (JObject)components["transform"];
            transform["position"] = new JArray(new int[] { y, z, x });
            transform["scale"] = scale;

            JObject model = (JObject)components["model"];
            model["file"] = fileNameModel;
            
            sendMessage(addObjectMessage.ToString());
        }

        private void addTerrain(int height)
        {
            TunnelMessage timeMessage = GetTunnelMessage("TerrainAdd.json");

            double[] heights = new double[40000];
            Random random = new Random();
            for (int i = 0; i < heights.Length; i++)
            {
                heights[i] = 0.03 * random.Next(10);
            }

            JArray jArray = new JArray(heights);
            timeMessage.getDataContent()["heights"] = jArray;
            sendMessage(timeMessage.ToString());
        }

        private void addTexture(string fileNormal, string fileDiffuse, string uuid)
        {
            TunnelMessage textureMessage = GetTunnelMessage("AddTexture.json");
            JObject data = textureMessage.getDataContent();
            data["id"] = uuid;
            data["normal"] = fileNormal;
            data["diffuse"] = fileDiffuse;
            sendMessage(textureMessage.ToString());
        }


        private void addRoute(RoutePoint[] points)
        {
            TunnelMessage routeMessage = GetTunnelMessage("RouteSetMessage.json");
            JObject data = routeMessage.getDataContent();
            JArray nodesArray = (JArray)data["nodes"];
            foreach (RoutePoint p in points)
            {
                JObject point = JObject.Parse("{\"pos\": [], \"dir\": []}");

                point["pos"] = new JArray(p.Pos);
                point["dir"] = new JArray(p.Dir);

                nodesArray.Add(point);
            }
            sendMessage(routeMessage.ToString());
        }

        private void addRoad(string normalTexture, string diffuseTexture, string specularTexture, string uuid)
        {
            TunnelMessage roadMessage = GetTunnelMessage("RoadSetMessage.json");
            JObject data = roadMessage.getDataContent();
            data["route"] = uuid;
            data["diffuse"] = diffuseTexture;
            data["normal"] = normalTexture;
            data["specular"] = specularTexture;

            sendMessage(roadMessage.ToString());
        }

        private struct RoutePoint
        {
            public int[] Dir { get; }
            public int[] Pos { get; }
            public RoutePoint(int[] pos, int[] dir)
            {
                this.Pos = pos;
                this.Dir = dir;
            }
        }

        private void sendMessage(string message)
        {
            byte[] length = BitConverter.GetBytes(message.Length);
            stream.Write(length);
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer);
        }

        public byte[] receiveBytes(int count)
        {
            byte[] buffer = new byte[count];
            int receivedBytes = 0;
            while(receivedBytes < count)
                receivedBytes += stream.Read(buffer, receivedBytes, count-receivedBytes);
            return buffer;
        }

        public void receive()
        {



            while (receiving)
            {
                byte[] lenghtBuffer = receiveBytes(4);/* new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    lenghtBuffer[i] = (byte)stream.ReadByte();
                }*/

                int length = BitConverter.ToInt32(lenghtBuffer);
                var buffer = receiveBytes(length); /* new List<byte>(length);

                for (int i = 0; i < length; i++)
                {
                    buffer.Add((byte)stream.ReadByte());
                }*/

                string jsonS = Encoding.ASCII.GetString(buffer);
                JObject json = JObject.Parse(jsonS);

                receiveMessage(json);
            }
        }

        private void receiveMessage(JObject json)
        {
            string id = (string)json["id"];

            if (id == "session/list")
            {
                printUsers(json);
            }
            else if (id == "tunnel/create")
            {
                checkTunnelStatus(json);
            }
            else if (id == "tunnel/send")
            {
                receiveNodeNameAndUuid(json);
            }
        }

        private void receiveNodeNameAndUuid(JObject json)
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
                    objects.Add(name, uuid);
                    Console.WriteLine("Added node to dictionary\nName: {0}\nuuid: {1}", name, uuid);

                    //TEMP
                    if (name == "ground")
                    {
                        addTexture("data/NetworkEngine/textures/grass_normal.png", "data/NetworkEngine/textures/grass_diffuse.png", uuid);
                        
                    }
                    if (name == "tree")
                    {
                        //addTexture("data/NetworkEngine/models/trees/fantasy/Tree_07.png", "", uuid);
                    }

                    //addRoute(new RoutePoint[] { new RoutePoint(new int[] { 0, 0, 0 }, new int[] { 10, 0, -10 }),
                    //        new RoutePoint(new int[] { 30, 0, 0 }, new int[] { 20, 0, 5 }),
                    //        new RoutePoint(new int[] { 0, 0, 15 }, new int[] { -15, 0, -10 }),
                    //        new RoutePoint(new int[] { 7, 0, 0 }, new int[] { 8, 0, -5 }),
                    //        new RoutePoint(new int[] { 0, 0, 20 }, new int[] { 13, 0, 25 })});


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
                    objects.Add(name, uuid);
                    Console.WriteLine("Added route to dictionary\nName: {0}\nuuid: {1}", name, uuid);
                    addRoad("data/NetworkEngine/textures/tarmac_normal.png", "data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_specular.png", uuid);
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
                    sendMessage(GetTunnelMessage("RouteSetMessage.json").ToString());
                }
            } else if((string)data["id"] == "scene/road/add")
            {
                if ((string)data["status"] == "ok")
                {
                    Random random = new Random();
                    for(int i = 0; i < 1000; i++)
                    {
                        int x = -100 + random.Next(200);
                        int y = -100 + random.Next(200);
                        int treeNumber = random.Next(11);
                        float scale = (float)(0.1 * random.Next(20));
                        addObject("data/NetworkEngine/models/trees/fantasy/tree" + treeNumber + ".obj", "tree" + i, x, y, 0, scale);
                    }
                }
            }
        }
        private void checkTunnelStatus(JObject json)
        {
            JObject data = (JObject)json["data"];
            string status = (string)data["status"];

            string id = (string)data["id"];

            if (status == "ok")
            {
                this.id = id;
                //setTime(1);
                addTerrain(2);
                addNode();
                //followRoute("");
            }

            Console.WriteLine("Status for tunnel: {0}\nid: {1}", status, id);
        }

        private TunnelMessage GetTunnelMessage(string jsonName)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string pathFile = currentPath + @"\Json files\" + jsonName;
            JObject message = JObject.Parse(File.ReadAllText(pathFile));
            TunnelMessage tunnelMessage = new TunnelMessage(message, id);
            return tunnelMessage;
        }

        private void printUsers(JObject json)
        {
            JArray data = (JArray)json["data"];
            Console.WriteLine("USERS:");

            for (int i = 0; i < data.Count; i++)
            {
                JObject clientInfo = (JObject)data[i]["clientinfo"];
                Console.WriteLine(clientInfo);

                if ((string)clientInfo["host"] == Environment.MachineName)
                {
                    sendTunnelRequest((string)data[i]["id"]);
                }
            }
        }
        private void followRoute(String node)
        {
            TunnelMessage followRouteMessage = GetTunnelMessage("FollowRoute.json");
            JObject data = followRouteMessage.getDataContent();
            data["routeid"] = objects["route"];
            data["nodeid"] = objects["tree1"];

            sendMessage(GetTunnelMessage("FollowRoute.json").ToString());
        }
    }
}




