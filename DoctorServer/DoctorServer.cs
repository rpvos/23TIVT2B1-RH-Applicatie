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

        private DoctorForm mainForm;
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

            this.mainForm = new DoctorForm();

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

        public void setDT(string DT)
        {
            this.mainForm.setDT(DT);
        }

        public void setAP(string AP)
        {
            this.mainForm.setAP(AP);
        }

        public void setElapsedTime(string elapsedTime)
        {
            this.mainForm.setElapsedTime(elapsedTime);
        }


        public void addClient(ServerClient serverClient)
        {
            this.mainForm.addBike(serverClient);
        }


    }
}
