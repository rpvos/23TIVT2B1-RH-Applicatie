using DoctorApplication;
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


namespace DoctorApplication
{
    public partial class DoctorForm : Form
    {

        public string selectedBike { get; set; }
        public int selectedIndex { get; set; }
        private DoctorClient doctorClient;

        public DoctorForm(DoctorClient doctorClient)
        {
            this.doctorClient = doctorClient;
            this.selectedIndex = -1;
            InitializeComponent();
            this.resistanceTextbox.MouseWheel += new MouseEventHandler(changeResistance);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public void setSpeed(string speed)
        {

            SpeedValue.Invoke((MethodInvoker)(() =>
            {
                this.SpeedValue.Text = speed;
            }));
        }

        public void setHeartrate(string heartrate)
        {
            HeartrateValue.Invoke((MethodInvoker)(() =>
            {
                this.HeartrateValue.Text = heartrate;
            }));
        }

        public void setDT(string DT)
        {
            DistanceTraveledValue.Invoke((MethodInvoker)(() =>
            {
                this.DistanceTraveledValue.Text = DT;
            }));
        }

        public void setAP(string AP)
        {
            APLabel.Invoke((MethodInvoker)(() =>
            {
                this.APLabel.Text = AP;
            }));

        }

        public void setElapsedTime(string elapsedTime)
        {
            ElapsedTimeValue.Invoke((MethodInvoker)(() =>
            {
                this.ElapsedTimeValue.Text = elapsedTime;
            }));
        }

        public void addBike(string username)
        {

            BikeListBox.Invoke((MethodInvoker)(() =>
            {
                BikeListBox.Items.Add(username);
            }));
            
            
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

                if (this.selectedIndex != -1)
                {
                    //this.doctorClient.WriteTextMessageToServer("RSTE" + selectedBike.ID + i);
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

                if (this.selectedIndex != -1)
                {
                    //this.doctorClient.WriteTextMessageToServer("RSTE" + selectedBike.ID + i);

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

            if (this.selectedIndex != -1)
            {
                //this.doctorClient.WriteTextMessageToServer("RSTE" + selectedBike.ID + i);

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

            if (this.selectedIndex != -1)
            {
                //this.doctorClient.WriteTextMessageToServer("RSTE" + selectedBike.ID + i);

            }
        }

        private void privateChat_SelectedIndexChanged(object sender, EventArgs e)
        {
          

        }


        private void privSendButton_Click(object sender, EventArgs e)
        {
            if (this.selectedIndex != -1 && PrivateChatBox.Text != "") 
            {
         

            }
        }

        private void globalSendButton_Click(object sender, EventArgs e)
        {
            
        }

        private void BikeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedBike = (string)BikeListBox.SelectedItem;
            this.selectedIndex = BikeListBox.SelectedIndex;
            Console.WriteLine(selectedBike);
        }
    }
}
