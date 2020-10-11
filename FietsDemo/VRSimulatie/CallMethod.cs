using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
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

        //Get the VR scene that is in use
        internal void GetScene()
        {
            TunnelMessage getSceneMessage = tcpClient.GetTunnelMessage("SceneGet.json");
            tcpClient.SendMessage(getSceneMessage.ToString());
        }

        //Reset the whole VR scene
        internal void ResetScene()
        {
            Console.WriteLine("RESET SCENE CALLED");
            TunnelMessage resetMessage = tcpClient.GetTunnelMessage("SceneReset.json");
            dynamic payloadData = new
            {
                id = "scene/reset"               
            };

            tcpClient.SendMessage(resetMessage.SendDataPacket(payloadData));
        }
        
        //Set the time in the VR scene
        internal void SetTime(int setTime)
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("TimeSetMessage.json");


            dynamic payloadData = new
            {
                time = setTime
            };

            tcpClient.SendMessage(timeMessage.SendDataPacket(payloadData));
        }

        //Add a ground node to the VR scene on which you can add texture
        internal void AddGroundNode(string nodeName, int[] pos, int[] rot)
        {
            TunnelMessage nodeMessage = tcpClient.GetTunnelMessage("NodeAdd.json");
            
            dynamic payloadData = new
            {
                id = "scene/node/add",
                data = new
                {
                    name = nodeName,
                    components = new
                    {
                        transforms = new
                        {
                            position = pos,
                            scale = 1,
                            rotation = rot
                        },
                        terrain = new
                        {
                            smoothnormals = true
                        }
                    }
                }               
            };

            Console.WriteLine(payloadData);
            tcpClient.SendMessage(nodeMessage.SendDataPacket(payloadData));
            Console.WriteLine(nodeMessage.SendDataPacket(payloadData));
        }

        //Add a panel to the VR scene
        internal void AddPanelNode(string nodeName, int[] pos, int[] rot, int[] panelSize, int[] res, int[] backgrColor)
        {
            TunnelMessage nodeMessage = tcpClient.GetTunnelMessage("NodeAdd.json");
            
            dynamic payloadData = new
            {
                id = "scene/node/add",
                data = new
                {
                    name = nodeName,
                    components = new
                    {
                        transforms = new
                        {
                            position = pos,
                            scale = 1,
                            rotation = rot
                        },
                        panel = new
                        {
                            size = panelSize,
                            resolution = res,
                            background = backgrColor,
                            castShadows = true
                        }
                    }
                }
            };

            Console.WriteLine(payloadData);
            tcpClient.SendMessage(nodeMessage.SendDataPacket(payloadData));
            Console.WriteLine(nodeMessage.SendDataPacket(payloadData));            
        }

        //Find a specific node from the VR scene
        internal void FindNode(string nodeName)
        {
            TunnelMessage findNodeMessage = tcpClient.GetTunnelMessage("NodeFind.json");

            dynamic payloadData = new
            {
                id = "scene/node/find",
                data = new
                {
                    name = nodeName
                }
            };         
            Console.WriteLine(findNodeMessage.SendDataPacket(payloadData));
            tcpClient.SendMessage(findNodeMessage.SendDataPacket(payloadData));
        }

        //Delete a specific node from the VR scene
        internal void DeleteNode(string nodeId)
        {
            TunnelMessage deleteNodeMessage = tcpClient.GetTunnelMessage("NodeDelete.json");

            dynamic payloadData = new
            {
                id = "scene/node/delete",
                data = new
                {
                    id = nodeId
                }
            };

            tcpClient.SendMessage(deleteNodeMessage.SendDataPacket(payloadData));
        }

        //Add a new object with node to the VR scene
        internal void AddObjectNode(string fileNameModel, string objectNodeName, int[] pos, int[] rot, float scale, string hasAnimation)
        {
            TunnelMessage addObjectMessage = tcpClient.GetTunnelMessage("AddObjectMessage.json");
            
            dynamic payloadData = new
            {
                id = "scene/node/add",
                data = new
                {
                    name = objectNodeName,
                    components = new
                    {
                        transforms = new
                        {
                            position = pos,
                            scale = 1,
                            rotation = rot
                        },
                        model = new
                        {
                            file = fileNameModel,
                            cullbackfaces = true,
                            animated = false,
                            animation = hasAnimation
                        }
                    }
                }
            };

            tcpClient.SendMessage(addObjectMessage.SendDataPacket(payloadData));
        }

        //Add a height map to the terrain
        internal void AddTerrain()
        {
            TunnelMessage timeMessage = tcpClient.GetTunnelMessage("TerrainAdd.json");

            double[] heights = new double[40000];
            Random random = new Random();
            double lastInt = 0.0;
            double reduction = 1;

            for (int i = 0; i < heights.Length; i++)
            {
                if (i > 200)
                {
                    lastInt = (heights[i - 200] + heights[i - 1]) / 2;
                }
                if (i % 1000 == 0)
                {
                    reduction += ((random.NextDouble() * 2) - 1) / 50;
                }
                heights[i] = (lastInt + ((random.NextDouble() * 2 - reduction) / 10));
                lastInt = heights[i];
            }

            JArray jArray = new JArray(heights);
            timeMessage.GetDataContent()["heights"] = jArray;
            tcpClient.SendMessage(timeMessage.ToString());
        }

        //Add texture to the VR scene
        internal void AddTexture(string fileNormal, string fileDiffuse, string uuid, int minimumHeight, int maximumHeight, int fadeDistance)
        {
            TunnelMessage textureMessage = tcpClient.GetTunnelMessage("AddTexture.json");

            dynamic payloadData = new
            {
                id = "scene/node/addlayer",
                data = new
                {
                    id = uuid,
                    normal = fileNormal,
                    diffuse = fileDiffuse,
                    minHeight = minimumHeight,
                    maxHeight = maximumHeight,
                    fadeDist = fadeDistance
                }
            };
            tcpClient.SendMessage(textureMessage.SendDataPacket(payloadData));
        }

        //Add a new road to the VR scene
        internal void AddRoad(string normalTexture, string diffuseTexture, string specularTexture, string uuid)
        {
            TunnelMessage roadMessage = tcpClient.GetTunnelMessage("RoadSetMessage.json");

            dynamic payloadData = new
            {
                id = "scene/road/add",
                data = new
                {
                    route = uuid,
                    diffuse = diffuseTexture,
                    normal = normalTexture,
                    specular = specularTexture,
                    heightoffset = 0.01
                }
            };

            tcpClient.SendMessage(roadMessage.SendDataPacket(payloadData));
        }

        //Add a new route to the VR scene
        internal void AddRoute()
        {
            ArrayList points = routePoints;
            TunnelMessage routeMessage = tcpClient.GetTunnelMessage("RouteSetMessage.json");
                        
            //foreach (RoutePoint p in points)
            //{
            //    JObject point = JObject.Parse("{\"pos\": [], \"dir\": []}");

            //    point["pos"] = new JArray(p.Pos);
            //    point["dir"] = new JArray(p.Dir);

            //    nodesArray.Add(point);
            //}

            //dynamic payloadData = new
            //{
            //    id = "route/add",
            //    data = new
            //    {
            //       nodes = nodesArray
            //    }
            //};

            //For now still using old code
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

        //Add new routepoints to the routePoints array
        internal void NewRoutePoints(int[] coord, int[] coord2)
        {
            routePoints.Add(new RoutePoint(coord, coord2));
        }

        //Set new routepoints. Give them a direction and a position
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

        //Let an object follow a set route with a specific nodeID
        internal void FollowRoute(string nodeId, double followSpeed, int[] possOff)
        {
            TunnelMessage followRouteMessage = tcpClient.GetTunnelMessage("FollowRoute.json");

            dynamic payloadData = new
            {
                id = "route/follow",
                data = new
                {
                    route = objects["route"],
                    node = nodeId,
                    speed = followSpeed,
                    offset = 0.0,
                    rotate = "XZ",
                    smoothing = 1.0,
                    followHeight = false,
                    rotateOffset = new int[] { 0,0,0,},
                    positionOffset = possOff
                }
            };

            tcpClient.SendMessage(followRouteMessage.SendDataPacket(payloadData));          
        }

        //Clear a panel in the vr simulator for first use
        internal void ClearPanel(string uuid)
        {
            Console.WriteLine("CLEARING PANEL");
            TunnelMessage clearPannelMessage = tcpClient.GetTunnelMessage("ClearPanel.json");

            dynamic payloadData = new
            {
                id = "scene/panel/clear",
                data = new
                {
                    id = uuid
                }
            };

            Console.WriteLine("ID of the clear data: " + uuid);
            tcpClient.SendMessage(clearPannelMessage.SendDataPacket(payloadData));
        }

        //Draw text on a panel in the vr simulator
        internal void SetText(string textToShow, string uuid, double[] coord, int textSize)
        {
            Console.WriteLine("REACHED SETTEXT METHOD!!!");
            Console.WriteLine("TEXT: {0}", textToShow);
            TunnelMessage setTextMessage = tcpClient.GetTunnelMessage("SetText.json");

            dynamic payloadData = new
            {
                id = "scene/panel/drawtext",
                data = new
                {
                    id = uuid,
                    text = textToShow,
                    position = coord,
                    size = textSize,
                    color = new int[] { 0, 0, 0, 1 },
                    font = "segoeui"
                }
            };

            tcpClient.SendMessage(setTextMessage.SendDataPacket(payloadData));
        } 
        
        //Swap the buffered panel to show the text
        internal void SwapPanel(string uuid)
        {
            Console.WriteLine("REACHED SWAPPING PANEL METHOD");
            TunnelMessage swapPanelMessage = tcpClient.GetTunnelMessage("SwapPanel.json");

            dynamic payloadData = new
            {
                id = "scene/panel/swap",
                data = new
                {
                    id = uuid
                }
            };

            tcpClient.SendMessage(swapPanelMessage.SendDataPacket(payloadData));
        }        
    }
}
