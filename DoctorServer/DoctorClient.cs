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
        public int ID { get; set; }

        [STAThread]
        static void Main()
        {
            DoctorClient doctorServer = new DoctorClient();
            doctorServer.Start();
              
        }

        public void Start()
        {
            this.random = new Random();
            this.ID = generateID();
            startClient();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();

            this.mainForm = new DoctorForm(this);

            Application.Run(mainForm);
        }

        public int generateID()
        {
            return random.Next(10000, 99999);
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
                this.streamWriter.WriteLine(this.ID+message);
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

                    if (a.Substring(0, 3) == "NEW")
                    {
                        Bike bike = new Bike(a.Substring(3));
                        this.bikes.Add(bike);
                        this.mainForm.addBike(bike);
                    }

                    else if (mainForm.selectedIndex != -1)
                    {
                        if (a.Substring(0, 5) == mainForm.selectedBike.ID)
                        {

                            if (a[5] == 's' && a[6] == 'p')
                            {
                                this.mainForm.setSpeed(a.Substring(a.IndexOf(":") + 2));
                            }
                            else if (a[5] == 'h' && a[6] == 'e')
                            {
                                this.mainForm.setHeartrate(a.Substring(a.IndexOf(":") + 2));

                            }
                            else if (a[5] == 'D' && a[6] == 'T')
                            {
                                this.mainForm.setDT(a.Substring(a.IndexOf(":") + 2));

                            }
                            else if (a[5] == 'A' && a[6] == 'P')
                            {
                                this.mainForm.setAP(a.Substring(a.IndexOf(":") + 2));

                            }
                            else if (a[5] == 'e' && a[6] == 'l')
                            {
                                this.mainForm.setElapsedTime(a.Substring(a.IndexOf(":") + 2));

                            }
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
        }


    }

    
}
