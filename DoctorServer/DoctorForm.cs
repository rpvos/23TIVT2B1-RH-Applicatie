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
    public partial class DoctorForm : Form
    {

        private List<ServerClient> serverClients;
        private int selectedClient = -1;

        public DoctorForm()
        {

            this.serverClients = new List<ServerClient>();
            InitializeComponent();
            this.resistanceTextbox.MouseWheel += new MouseEventHandler(changeResistance);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void setSpeed(string speed)
        {
            this.SpeedValue.Text = speed;
        }

        public void setHeartrate(string heartrate)
        {
            this.HeartrateValue.Text = heartrate;
        }

        public void setDT(string DT)
        {
            this.DistanceTraveledValue.Text = DT;
        }

        public void setAP(string AP)
        {
            this.APLabel.Text = AP;
        }

        public void setElapsedTime(string elapsedTime)
        {
            this.ElapsedTimeValue.Text = elapsedTime;
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

        private void HeartRateLabel_Click(object sender, EventArgs e)
        {

        }

        private void SpeedLabel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void BikeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedClient = BikeListBox.SelectedIndex;
            foreach (ServerClient v in serverClients)
                v.setSelected(false);

            this.serverClients[this.selectedClient].setSelected(true);
            this.resistanceTextbox.Text = this.serverClients[this.selectedClient].resistance + "";

        }

        private void Speed_Click(object sender, EventArgs e)
        {

        }


        public void changeResistance(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int i = Int32.Parse(resistanceTextbox.Text);
                i++;
                if (i > 100)
                {
                    i = 100;
                }
                resistanceTextbox.Text = i + "";

                if (this.selectedClient != -1)
                {
                    this.serverClients[selectedClient].WriteTextMessage(resistanceTextbox.Text);
                    this.serverClients[selectedClient].resistance = i;
                }
            }
            else if (e.Delta < 0)
            {
                int i = Int32.Parse(resistanceTextbox.Text);
                i--;
                if (i < 0)
                {
                    i = 0;
                }
                resistanceTextbox.Text = i + "";

                if (this.selectedClient != -1)
                {
                    this.serverClients[selectedClient].WriteTextMessage(resistanceTextbox.Text);
                    this.serverClients[selectedClient].resistance = i;
                }


            }

        }

        private void minResistance_Click_1(object sender, EventArgs e)
        {
            int i = Int32.Parse(resistanceTextbox.Text);
            i -= 5;
            if (i < 0)
            {
                i = 0;
            }
            resistanceTextbox.Text = i + "";

            if (this.selectedClient != -1)
            {
                this.serverClients[selectedClient].WriteTextMessage(resistanceTextbox.Text);
                this.serverClients[selectedClient].resistance = i;
            }

        }

        private void plusResistance_Click_1(object sender, EventArgs e)
        {
            int i = Int32.Parse(resistanceTextbox.Text);
            i += 5;
            if (i > 100)
            {
                i = 100;
            }
            resistanceTextbox.Text = i + "";

            if (this.selectedClient != -1)
            {
                this.serverClients[selectedClient].WriteTextMessage(resistanceTextbox.Text);
                this.serverClients[selectedClient].resistance = i;
            }
        }

        private void privateChat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
