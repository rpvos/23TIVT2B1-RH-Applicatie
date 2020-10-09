using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace FietsDemo.VRSimulatie
{
    static class Node
    {
        public static dynamic AddTerrain(string terrainName, int[] pos, int[] rot)
        {
            dynamic data = new
            {
                name = terrainName,
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
            };

            return data;
        }

    }
}
