﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Cache;
using System.Text;
using TCP_naar_VR;

namespace simulatie
{
    class CallMethod
    {
        private TcpClientVR tcpClient;
        private Dictionary<string, string> objects;
        private ArrayList routePoints;

        public CallMethod(TcpClientVR tcpClient, Dictionary<string, string> objects)
        {
            this.tcpClient = tcpClient;
            this.objects = objects;
            this.routePoints = new ArrayList();
        }

        internal void DeleteGroundPlane(string uuid)
        {
            DeleteNode(uuid);
        }

        internal void GetScene()
        {
            TunnelMessage getSceneMessage = tcpClient.GetTunnelMessage("SceneGet.json");
            tcpClient.SendMessage(getSceneMessage.ToString());
        }
        
        internal void SetTime(int time)
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("TimeSetMessage.json");
            timeMessage.GetDataContent()["time"] = time;
            tcpClient.SendMessage(timeMessage.ToString());

        }

        internal void AddNode()
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("NodeAdd.json");
            tcpClient.SendMessage(timeMessage.ToString());
        }

        internal void AddUniversalNode(string name, int[] pos, int[] rotation)
        {
            TunnelMessage universalNodeAdd = tcpClient.GetTunnelMessage("UniversalNodeAdd.json");
            JObject data = universalNodeAdd.GetDataContent();
            data["name"] = name;

            JObject components = (JObject)data["components"];
            JObject transform = (JObject)components["transform"];
            transform["position"] = new JArray(pos);
            transform["rotate"] = new JArray(rotation);

            //Console.WriteLine(universalNodeAdd.ToString());

            tcpClient.SendMessage(universalNodeAdd.ToString());
        }

        internal void FindNode(string name)
        {
            TunnelMessage findNodeMessage = tcpClient.GetTunnelMessage("NodeFind.json");
            JObject data = findNodeMessage.GetDataContent();
            data["name"] = name;

            tcpClient.SendMessage(findNodeMessage.ToString());
            Console.WriteLine(findNodeMessage.GetDataContent());


        }

        internal void DeleteNode(string id)
        {
            TunnelMessage deleteNodeMessage = tcpClient.GetTunnelMessage("NodeDelete.json");
            JObject data = deleteNodeMessage.GetDataContent();
            data["id"] = id;

            tcpClient.SendMessage(deleteNodeMessage.ToString());
        }

        internal void AddObject(string fileNameModel, string objectName, int x, int y, int z, float scale)
        {
            TunnelMessage addObjectMessage = tcpClient.GetTunnelMessage("AddObjectMessage.json");
            JObject data = addObjectMessage.GetDataContent();
            data["name"] = objectName;

            JObject components = (JObject)data["components"];

            JObject transform = (JObject)components["transform"];
            transform["position"] = new JArray(new int[] { y, z, x });
            transform["scale"] = scale;

            JObject model = (JObject)components["model"];
            model["file"] = fileNameModel;

            tcpClient.SendMessage(addObjectMessage.ToString());
        }

        internal void AddTerrain(int size)
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("TerrainAdd.json");

            double[] heights = new double[size];
            Random random = new Random();
            double lastInt = 0.0;
            double reduction = 1;
            for (int i = 0; i < heights.Length; i++)
            {
                //After one row completed, the height will look at the previous row and the int next to it.
                if (i > Math.Sqrt(size))
                {
                    lastInt = (heights[i - (int)Math.Sqrt(size)] + heights[i - 1]) / 2;
                }
                //Extra randomness
                if (i % 1000 == 0)
                {
                    reduction += ((random.NextDouble() * 2) - 1) / 50;
                }
                //Calculates height
                heights[i] = (lastInt + ((random.NextDouble() * 2 - reduction) / 10));
                lastInt = heights[i];

                //Height cant be below zero
                if (heights[i] < 0)
                {
                    heights[i] = 0;
                    reduction -= 0.2;
                }
                //Height cant be above six.
                if (heights[i] > 6)
                {
                    reduction += 0.2;
                }
            }
            JArray jArraySize = new JArray((int)Math.Sqrt(size), (int)Math.Sqrt(size));
            JArray jArrayHeights = new JArray(heights);
            timeMessage.GetDataContent()["heights"] = jArrayHeights;
            timeMessage.GetDataContent()["size"] = jArraySize;
            tcpClient.SendMessage(timeMessage.ToString());
        }

        internal void AddTexture(string fileNormal, string fileDiffuse, string uuid)
        {
            TunnelMessage textureMessage = tcpClient.GetTunnelMessage("AddTexture.json");
            JObject data = textureMessage.GetDataContent();
            data["id"] = uuid;
            data["normal"] = fileNormal;
            data["diffuse"] = fileDiffuse;
            tcpClient.SendMessage(textureMessage.ToString());
        }

        internal void AddRoad(string normalTexture, string diffuseTexture, string specularTexture, string uuid)
        {
            TunnelMessage roadMessage = tcpClient.GetTunnelMessage("RoadSetMessage.json");
            JObject data = roadMessage.GetDataContent();
            data["route"] = uuid;
            data["diffuse"] = diffuseTexture;
            data["normal"] = normalTexture;
            data["specular"] = specularTexture;
            tcpClient.SendMessage(roadMessage.ToString());
        }

        internal void AddRoute()
        {
            ArrayList points = routePoints;
            TunnelMessage routeMessage = tcpClient.GetTunnelMessage("RouteSetMessage.json");
            JObject data = routeMessage.GetDataContent();
            JArray nodesArray = (JArray)data["nodes"];
            foreach (RoutePoint p in points)
            {
                JObject point = JObject.Parse("{\"pos\": [], \"dir\": []}");

                point["pos"] = new JArray(p.Pos);
                point["dir"] = new JArray(p.Dir);

                nodesArray.Add(point);
            }
            tcpClient.SendMessage(routeMessage.ToString());
        }

        internal void NewRoutePoints(int[] coord, int[] coord2)
        {
            routePoints.Add(new RoutePoint(coord, coord2));
        }



        internal struct RoutePoint
        {
            public int[] Dir { get; }
            public int[] Pos { get; }
            public RoutePoint(int[] pos, int[] dir)
            {
                this.Pos = pos;
                this.Dir = dir;
            }
        }

        internal void FollowRoute(String node)
        {
            TunnelMessage followRouteMessage = tcpClient.GetTunnelMessage("FollowRoute.json");
            JObject data = followRouteMessage.GetDataContent();
            data["routeid"] = objects["route"];
            data["nodeid"] = objects["tree1"];

            tcpClient.SendMessage(followRouteMessage.ToString());
        }

        //Clear a panel in the vr simulator for first use
        internal void ClearPanel(string uuid)
        {
            Console.WriteLine("CLEARING PANEL");
            TunnelMessage clearPannelMessage = tcpClient.GetTunnelMessage("ClearPanel.json");
            JObject data = clearPannelMessage.GetDataContent();
            
            data["id"] = uuid;

            Console.WriteLine("ID of the clear data: " + (string)data["id"]);

            tcpClient.SendMessage(clearPannelMessage.ToString());
        }

        //Draw text on a panel in the vr simulator
        internal void SetText(string text, string uuid, double[] coord)
        {
            Console.WriteLine("REACHED SETTEXT METHOD!!!");
            Console.WriteLine("TEXT: {0}", text);
            TunnelMessage setTextMessage = tcpClient.GetTunnelMessage("SetText.json");
            JObject data = setTextMessage.GetDataContent();

            data["id"] = uuid;
            data["text"] = text;
            data["position"] = new JArray(coord);
            tcpClient.SendMessage(setTextMessage.ToString());
        } 
        
        //Swap the buffered panel to show the text
        internal void SwapPanel(string uuid)
        {
            Console.WriteLine("REACHED SWAPPING PANEL METHOD");
            TunnelMessage swapPanelMessage = tcpClient.GetTunnelMessage("SwapPanel.json");
            JObject data = swapPanelMessage.GetDataContent();

            data["id"] = uuid;

            Console.WriteLine(swapPanelMessage.ToString());
            tcpClient.SendMessage(swapPanelMessage.ToString());
        }        
    }
}
