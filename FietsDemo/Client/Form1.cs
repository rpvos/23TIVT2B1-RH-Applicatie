using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FietsDemo
{
    public partial class MainForm : Form
    {

        private GUI gui;
        public MainForm(GUI gui)
        {
            this.gui = gui;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.gui.bluetoothBike.client.disconnect();
            Application.Exit();
        }

        public void setSpeed(double speed)
        {
            SpeedValue.Invoke((MethodInvoker)(() =>
            {
                SpeedValue.Text = speed + " km/h";
            }));
        }

        public void setHeartrate(double heartrate)
        {
            HeartrateValue.Invoke((MethodInvoker)(() =>
            {
                HeartrateValue.Text = heartrate + " bpm";
            }));
        }

        public void setAP(double AP)
        {
            APLabel.Invoke((MethodInvoker)(() =>
            {
                APLabel.Text = AP + " Watt";
            }));
        }

        public void setDT(double DT)
        {
            DistanceTraveledValue.Invoke((MethodInvoker)(() =>
            {
                DistanceTraveledValue.Text = DT + " km";
            }));
        }

        public void setElapsedTime(double elapsedTime)
        {
            var timeSpan = TimeSpan.FromSeconds(elapsedTime);
            ElapsedTimeValue.Invoke((MethodInvoker)(() =>
            {
                ElapsedTimeValue.Text = timeSpan.ToString(@"hh\:mm\:ss");
            }));
        }
        public void setResistance(double resistance)
        {
            ResistanceValue.Invoke((MethodInvoker)(() =>
            {
                ResistanceValue.Text = resistance+"%";
            }));
        }

        private void ESPSimulatorButton_Click(object sender, EventArgs e)
        {
            SoftwareSimulatorButton.BackColor = Color.White;
            ESPSimulatorButton.BackColor = Color.LightBlue;
            this.gui.stopSimulator();
        }

        private void SoftwareSimulatorButton_Click(object sender, EventArgs e)
        {
            SoftwareSimulatorButton.BackColor = Color.LightBlue;
            ESPSimulatorButton.BackColor = Color.White;
            this.gui.startSimulator();
        }

        private void Speed_Click(object sender, EventArgs e)
        {

        }

        public void addMessage(string message)
        {
            doctorChat.Invoke((MethodInvoker)(() =>
            {
                doctorChat.Items.Add(message);

            }));
        }

        private void sendDocButton_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text != "")
            {
                doctorChat.Items.Add(chatTextBox.Text);
                this.gui.bluetoothBike.sendPrivateMessage(chatTextBox.Text);
                chatTextBox.Text = "";

            }
        }
    }

}
