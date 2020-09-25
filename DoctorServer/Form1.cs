using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoctorServer
{
    public partial class Form1 : Form
    {

        private List<ServerClient> serverClients;

        public Form1()
        {
            Thread checkingSelectedBikeThread = new Thread(checkSelectedBike);

            this.serverClients = new List<ServerClient>();
            InitializeComponent();
            checkingSelectedBikeThread.Start();

        }

        public void checkSelectedBike()
        {
            int i = -1;
            while (true)
            {
                i = BikeListBox.SelectedIndex;
                if(i != -1)
                {
                    foreach (ServerClient v in serverClients)
                        v.setSelected(false);

                    this.serverClients[i].setSelected(true);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void setSpeed(string speed)
        {
            this.SpeedLabel.Text = speed;
        }

        public void setHeartrate(string heartrate)
        {
            this.HeartRateLabel.Text = heartrate;
        }

        public void addBike(ServerClient serverClient)
        {
            BikeListBox.Items.Add(serverClient);
            this.serverClients.Add(serverClient);
            
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
