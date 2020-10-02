using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DoctorApplication;

namespace DoctorServer
{
    public class DoctorClient
    {

        private DoctorForm mainForm;
        private TcpClient server;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private List<Bike> bikes;
        private Random random;

        [STAThread]
        static void Main()
        {
            DoctorClient doctorServer = new DoctorClient();
            doctorServer.Start();
              
        }

        public void Start()
        {
            this.random = new Random();
            startClient();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();

            this.mainForm = new DoctorForm(this);

            Application.Run(mainForm);
        }

  

        public void startClient()
        {
            this.bikes = new List<Bike>();
            this.server = new TcpClient("127.0.0.1", 1330);

            this.streamWriter = new StreamWriter(server.GetStream(), Encoding.ASCII, 1, true);
            this.streamReader = new StreamReader(server.GetStream(), Encoding.ASCII);
            WriteTextMessageToServer("DOCTOR");

            Thread receiveMessages = new Thread(ReadTextMessageFromServer);
            receiveMessages.Start();
        }



        public void WriteTextMessageToServer(string message)
        {
            try
            {
                this.streamWriter.WriteLine(message);
                this.streamWriter.Flush();
            }
            catch { }

        }

        public void ReadTextMessageFromServer()
        {
            while (true)
            {
                try
                {
                    String a = this.streamReader.ReadLine();

                    
                }
                catch
                {
                    break;
                }
            }
        }


    }

    
}
