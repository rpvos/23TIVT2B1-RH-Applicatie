//using System;
//using System.IO;
//using System.Net.Sockets;
//using System.Text;
//using System.Text.RegularExpressions;
//
//namespace DoctorServer
//{
//    public class ServerClient
//    {
//
//        private TcpClient client;
//        private StreamWriter streamWriter;
//        private StreamReader streamReader;
//        private bool selected = false;
//        public int resistance { set; get; }
//        public String username { get; set; }
//
//        public ServerClient(TcpClient client, String username)
//        {
//            this.resistance = 0;
//            this.username = username;
//            this.client = client;
//            this.streamWriter = new StreamWriter(client.GetStream(), Encoding.ASCII, -1, true);
//            this.streamReader = new StreamReader(client.GetStream(), Encoding.ASCII);
//        }
//
//
//        public TcpClient GetTcpClient()
//        {
//            return this.client;
//        }
//
//        public void setSelected(bool selected)
//        {
//            this.selected = selected;
//        }
//
//
//        public void WriteTextMessage(string message)
//        {
//            try
//            {
//                streamWriter.WriteLine(message);
//                streamWriter.Flush();
//            }
//            catch { }
//        }
//
//        public void ReadTextMessage(DoctorClient doctorServer)
//        {
//            while (true)
//            {
//                try
//                {
//                    if (selected)
//                    {
//                        String a = streamReader.ReadLine();
//                        if (a[0] == 's' && a[1] == 'p')
//                        {
//                            doctorServer.setSpeed(a.Substring(a.IndexOf(":")+2));
//                        }
//                        else if (a[0] == 'h' && a[1] == 'e')
//                        {
//                            doctorServer.setHeartRate(a.Substring(a.IndexOf(":")+2));
//                        }
//                        else if (a[0] == 'D' && a[1] == 'T')
//                        {
//                            doctorServer.setDT(a.Substring(a.IndexOf(":") + 2));
//                        }
//                        else if (a[0] == 'A' && a[1] == 'P')
//                        {
//                            doctorServer.setAP(a.Substring(a.IndexOf(":") + 2));
//                        }
//                        else if (a[0] == 'e' && a[1] == 'l')
//                        {
//                            doctorServer.setElapsedTime(a.Substring(a.IndexOf(":") + 2));
//                        }
//                    }
//                }
//                catch
//                {
//                    client.Close();
//                }
//            }
//
//        }
//
//        public override string ToString()
//        {
//            return this.username;
//        }
//    }
//}
