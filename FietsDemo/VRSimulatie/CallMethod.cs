using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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
        internal void DeleteGroundPlane()
        {
            Console.WriteLine(FindNode("GroundPlane"));
            //findNode returns a uuid of the given node, then use this uuid to delete the node
            //deleteNode takes a uuid of the desired node to delete
            DeleteNode(FindNode("GroundPlane"));
        }
        internal void GetScene()
        {
            TunnelMessage getSceneMessage = tcpClient.GetTunnelMessage("SceneGet.json");
            Console.WriteLine(getSceneMessage.ToString());
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

           
            //dynamic data = new
            //{
            //    id = "scene/node/add",
            //    data = new
            //    {
            //        name= name,
            //        components = new
            //        {

            //        }
            //    }
            //};

            JObject components = (JObject)data["components"];
            JObject transform = (JObject)components["transform"];
            transform["position"] = new JArray(pos);
            transform["rotate"] = new JArray(rotation);

            Console.WriteLine(universalNodeAdd.ToString());

            tcpClient.SendMessage(universalNodeAdd.ToString());
        }

        internal string FindNode(string name)
        {
            TunnelMessage findNodeMessage = tcpClient.GetTunnelMessage("NodeFind.json");
            JObject data = findNodeMessage.GetDataContent();
            JObject children = (JObject)data["children"];
            //JObject data2 -(JObject)children["components"];
            if((string)children["name"] == name)
            {
                Console.WriteLine("NAME: " + (string)children["name"] + "\t UUID: " + (string) children["uuid"]);

            }
            return (string)children["uuid"];
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
            data["postition"] = new JArray(coord);
            tcpClient.SendMessage(setTextMessage.ToString());
        } 
        
        //Swap the buffered panel to show the text
        internal void SwapPanel(string uuid)
        {
            Console.WriteLine("REACHED SWAPPING PANEL METHOD");
            TunnelMessage swapPanelMessage = tcpClient.GetTunnelMessage("SwapPanel.json");
            JObject data = swapPanelMessage.GetDataContent();

            data["id"] = uuid;

            tcpClient.SendMessage(swapPanelMessage.ToString());
        }        
    }
}
