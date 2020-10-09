using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FietsDemo.VRSimulatie.commands
{
    static class Terrain
    {
        public static dynamic Add(int[] size, double[]heightMap)
        {
            dynamic packet = new
            {
                size = size,
                heights = heightMap
            };

            return packet;
        }

        public static dynamic Delete()
        {
            dynamic packet = new
            {

            };

            return packet;
        }

        public static dynamic GetHeight(double[] pos, double[,] positions)
        {
            dynamic packet = new
            {
                position = pos,
                postions = positions
            };

            return packet;
        }
    }
}
