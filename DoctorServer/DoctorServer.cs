using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoctorServer
{
    public class DoctorServer
    {

        private Form1 mainForm;
        private Server server;

        [STAThread]
        static void Main()
        {
            DoctorServer doctorServer = new DoctorServer();
            doctorServer.Start();
              
        }

        public void Start()
        {
            Thread serverThread = new Thread(StartServer);
            serverThread.Start();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();

            this.mainForm = new Form1();

            Application.Run(mainForm);
        }

        public void StartServer()
        {
            server = new Server(this);
        }

        public void setSpeed(string speed)
        {
            this.mainForm.setSpeed(speed);
        }

        public void setHeartRate(string heartrate)
        {
            this.mainForm.setHeartrate(heartrate);
        }

        public void addClient(ServerClient serverClient)
        {
            this.mainForm.addBike(serverClient);
        }


    }
}
