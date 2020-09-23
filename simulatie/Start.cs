
using Newtonsoft.Json;

namespace TCP_naar_VR
{
    class Program
    {
        
        static void Main(string[] args)
        {
           // TcpClientVR tcpClientVR = new TcpClientVR("145.48.6.10", 6666);
           // tcpClientVR.sendKickOff();


            dynamic packetData = new
            {
                id = "scene/node/add",
                data = new
                {
                    arrayData = new [] { 1,2,3,4 },
                    floatData = 10.0f
                }
            };

            dynamic packet = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = "soijfsd",
                    data = packetData
                }
            };


            string jsonString = JsonConvert.SerializeObject(packet);
            System.Console.WriteLine(jsonString);

        }
    }
}
