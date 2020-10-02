using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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

        internal void AddTerrain()
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("TerrainAdd.json");

            double[] heights = new double[40000];
            Random random = new Random();
            for (int i = 0; i < heights.Length; i++)
            {
                heights[i] = 0.03 * random.Next(10);
            }

            JArray jArray = new JArray(heights);
            timeMessage.GetDataContent()["heights"] = jArray;
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

            tcpClient.SendMessage(tcpClient.GetTunnelMessage("FollowRoute.json").ToString());
        }
    }
}
