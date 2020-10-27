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
        private Dictionary<string, string> panelId;
        private ArrayList routePoints;
        private int minutes;

        //This class generates all the messages that will be send to the server
        public CallMethod(TcpClientVR tcpClient, Dictionary<string, string> objects)
        {
            this.tcpClient = tcpClient;
            this.panelId = objects;
            this.routePoints = new ArrayList();
            this.minutes = 0;
        }

        #region Scene
        //Get the VR scene that is in use
        internal void GetScene()
        {
            TunnelMessage getSceneMessage = this.tcpClient.GetTunnelMessage("SceneGet.json");

            dynamic payload = new
            {
                id = "scene/get"
            };

            this.tcpClient.SendMessage(getSceneMessage.SendDataPacket(payload));
        }

        //Reset the whole VR scene
        internal void ResetScene()
        {
            Console.WriteLine("RESET SCENE CALLED");
            TunnelMessage resetMessage = this.tcpClient.GetTunnelMessage("SceneReset.json");

            dynamic payloadData = new
            {
                id = "scene/reset"
            };

            this.tcpClient.SendMessage(resetMessage.SendDataPacket(payloadData));
        }       
        #endregion 

        #region Time
        //Set the time in the VR scene
        internal void SetTime(double setTime)
        {
            TunnelMessage timeMessage = this.tcpClient.GetTunnelMessage("TimeSetMessage.json");

            dynamic payloadData = new
            {
                time = setTime
            };

            this.tcpClient.SendMessage(timeMessage.SendDataPacket(payloadData));
        }
        #endregion

        #region Nodes
        //Add a ground node to the VR scene on which you can add texture
        internal void AddGroundNode(string nodeName, int[] pos, int[] rot)
        {
            TunnelMessage nodeMessage = this.tcpClient.GetTunnelMessage("NodeAdd.json");

            dynamic payloadData = new
            {
                id = "scene/node/add",
                data = new
                {
                    name = nodeName,
                    components = new
                    {
                        transform = new
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

            this.tcpClient.SendMessage(nodeMessage.SendDataPacket(payloadData));
        }

        
        //Add a panel to the VR scene
        internal void AddPanelNode(string nodeName, double[] pos, double[] rot, double[] panelSize, int[] res, int[] backgrColor)
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
                        transform = new
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

            this.tcpClient.SendMessage(nodeMessage.SendDataPacket(payloadData));           
        }

        //Find a specific node from the VR scene
        internal void FindNode(string nodeName)
        {
            TunnelMessage findNodeMessage = this.tcpClient.GetTunnelMessage("NodeFind.json");

            dynamic payloadData = new
            {
                id = "scene/node/find",
                data = new
                {
                    name = nodeName
                }
            };
            
            Console.WriteLine(findNodeMessage.SendDataPacket(payloadData));
            this.tcpClient.SendMessage(findNodeMessage.SendDataPacket(payloadData));
        }

        //Delete a specific node from the VR scene
        internal void DeleteNode(string nodeId)
        {
            TunnelMessage deleteNodeMessage = this.tcpClient.GetTunnelMessage("NodeDelete.json");

            dynamic payloadData = new
            {
                id = "scene/node/delete",
                data = new
                {
                    id = nodeId
                }
            };

            this.tcpClient.SendMessage(deleteNodeMessage.SendDataPacket(payloadData));
        }

        //Add a new object with node to the VR scene
        internal void AddObjectNode(string fileNameModel, string objectNodeName, int[] pos, int[] rot, bool anim, string hasAnimation, double objectScale)
        {
            TunnelMessage addObjectMessage = this.tcpClient.GetTunnelMessage("NodeAdd.json");
            
            dynamic payloadData = new
            {
                id = "scene/node/add",
                data = new
                {
                    name = objectNodeName,
                    components = new
                    {
                        transform = new
                        {
                            position = pos,
                            scale = objectScale,
                            rotation = rot
                        },
                        model = new
                        {
                            file = fileNameModel,
                            cullbackfaces = true,
                            animated = anim,
                            animation = hasAnimation
                        }
                    }
                }
            };

            this.tcpClient.SendMessage(addObjectMessage.SendDataPacket(payloadData));
        }

        //Bind a specific node to a parent node, and update it
        internal void UpdateNode(string nodeId, string parentId, double[] pos,double transScale, double[] rot)
        {
            TunnelMessage updateMessage = this.tcpClient.GetTunnelMessage("NodeUpdate.json");

            dynamic payload = new
            {
                id = "scene/node/update",
                data = new
                {
                    id = nodeId,
                    parent = parentId,
                    transform = new
                    {
                        position = pos,
                        scale = transScale,
                        rotation = rot
                    }
                }
            };

            this.tcpClient.SendMessage(updateMessage.SendDataPacket(payload));
        }
        #endregion

        #region Terrain
        //Add a height map to the terrain
        internal void AddTerrain(int size)
        {
            TunnelMessage timeMessage = this.tcpClient.GetTunnelMessage("TerrainAdd.json");

            double[] heights = new double[size];
            Random random = new Random();
            double lastInt = 0.0;
            double reduction = 1;

            //Randomization for all the Terrain heights
            for (int i = 0; i < heights.Length; i++)
            {
                if (i > (int)Math.Sqrt(size))
                {
                    lastInt = (heights[i - (int)Math.Sqrt(size)] + heights[i - 1]) / 2;
                }
                if (i % 1000 == 0)
                {
                    reduction += ((random.NextDouble() * 2) - 1) / 50;
                    
                }
                heights[i] = (lastInt + ((random.NextDouble() * 2 - reduction) / 10));
                if(heights[i] < 0)
                {
                    heights[i] = 0;
                    reduction -= 0.2;
                    
                }
                if(heights[i] >= 6)
                {
                    heights[i] = 6;
                    reduction += 0.2;
                    
                }
                lastInt = heights[i];
            }

            JArray heightsArray = new JArray(heights);

            dynamic payloadData = new
            {
                id = "scene/terrain/add",
                data = new
                {
                    size = new int[] { (int)Math.Sqrt(size), (int)Math.Sqrt(size) },
                    heights = heightsArray
                }

            };
            
            this.tcpClient.SendMessage(timeMessage.SendDataPacket(payloadData));
        }

        

        //Add texture to the VR scene
        internal void AddTexture(string fileNormal, string fileDiffuse, string uuid, int minimumHeight, int maximumHeight, int fadeDistance)
        {
            TunnelMessage textureMessage = this.tcpClient.GetTunnelMessage("AddTexture.json");

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

            this.tcpClient.SendMessage(textureMessage.SendDataPacket(payloadData));
        }

        //Add a new road to the VR scene
        internal void AddRoad(string normalTexture, string diffuseTexture, string specularTexture, string uuid)
        {
            TunnelMessage roadMessage = this.tcpClient.GetTunnelMessage("RoadSetMessage.json");

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

            this.tcpClient.SendMessage(roadMessage.SendDataPacket(payloadData));
        }
        #endregion

        #region Route
        //Add a new route to the VR scene
        internal void AddRoute()
        {
            ArrayList points = routePoints;
            TunnelMessage routeMessage = this.tcpClient.GetTunnelMessage("RouteSetMessage.json");
            JArray nodesArray = new JArray();

            foreach (RoutePoint p in points)
            {
                JObject point = JObject.Parse("{\"pos\": [], \"dir\": []}");
                point["pos"] = new JArray(p.Pos);
                point["dir"] = new JArray(p.Dir);

                nodesArray.Add(point);
            }

            dynamic payloadData = new
            {
                id = "route/add",
                data = new
                {
                    nodes = nodesArray
                }
            };

            this.tcpClient.SendMessage(routeMessage.SendDataPacket(payloadData));
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
        internal void FollowRoute(string routeId, string nodeId, double followSpeed, bool heightFollow, double[]rotOff, int[] posOff)
        {
            TunnelMessage followRouteMessage = this.tcpClient.GetTunnelMessage("FollowRoute.json");

            dynamic payloadData = new
            {
                id = "route/follow",
                data = new
                {
                    route = routeId,
                    node = nodeId,
                    speed = followSpeed,
                    offset = 0.0,
                    rotate = "XZ",
                    smoothing = 1.0,
                    followHeight = heightFollow,
                    //(Z, X, Y)
                    rotateOffset = rotOff,
                    positionOffset = posOff
                }
            };

            Console.WriteLine(followRouteMessage.SendDataPacket(payloadData));
            this.tcpClient.SendMessage(followRouteMessage.SendDataPacket(payloadData));          
        }
        #endregion

        #region Panel
        //Clear a panel in the vr simulator for first use
        internal void ClearPanel(string uuid)
        {
            TunnelMessage clearPannelMessage = this.tcpClient.GetTunnelMessage("ClearPanel.json");

            dynamic payloadData = new
            {
                id = "scene/panel/clear",
                data = new
                {
                    id = uuid
                }
            };

            Console.WriteLine("ID of the clear data: " + uuid);
            this.tcpClient.SendMessage(clearPannelMessage.SendDataPacket(payloadData));
        }

        //Draw text on a panel in the vr simulator
        internal void SetText(string textToShow, string uuid, double[] coord, double textSize)
        {
            TunnelMessage setTextMessage = this.tcpClient.GetTunnelMessage("SetText.json");

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

            this.tcpClient.SendMessage(setTextMessage.SendDataPacket(payloadData));
        } 
        
        //Swap the buffered panel to show the text
        internal void SwapPanel(string uuid)
        {
            TunnelMessage swapPanelMessage = this.tcpClient.GetTunnelMessage("SwapPanel.json");

            dynamic payloadData = new
            {
                id = "scene/panel/swap",
                data = new
                {
                    id = uuid
                }
            };

            this.tcpClient.SendMessage(swapPanelMessage.SendDataPacket(payloadData));
        }

        //Updates the panel with the new values
        internal void UpdatePanel(string panelId)
        {
            int textSize = 45;
            int x = 20;

            var timeSpan = TimeSpan.FromSeconds((double)this.tcpClient.elapsedTime);
            
            SetText("Speed in km/h: " + this.tcpClient.speed.ToString(), panelId, new double[] { x, 100 }, textSize);
            SetText("Heart rate in bpm: " + this.tcpClient.heartRate.ToString(), panelId, new double[] { x, 150 }, textSize);
            SetText("Distance travelled in meters: ", panelId, new double[] { x, 200 }, textSize);
            SetText(this.tcpClient.DT.ToString() + "m", panelId, new double[] { x, 250 }, textSize);
            SetText("Elapsed time in seconds: ", panelId, new double[] { x, 300 }, textSize);
            SetText(timeSpan.ToString(@"hh\:mm\:ss"), panelId, new double[] { x, 350 }, textSize);
            SetText("Resistance in %: " + this.tcpClient.resistance.ToString(), panelId, new double[] { x, 400 }, textSize);

            SwapPanel(panelId);
        }
        #endregion
    }
}
