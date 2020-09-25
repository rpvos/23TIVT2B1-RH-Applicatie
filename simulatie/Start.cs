
using Newtonsoft.Json;

namespace TCP_naar_VR
{
    class Program
    {
        
        static void Main(string[] args)
        {
            TcpClientVR tcpClientVR = new TcpClientVR("145.48.6.10", 6666);
            tcpClientVR.SendKickOff();
        }
    }
}
