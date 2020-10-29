using System;
using System.Drawing;
using System.Windows.Forms;

namespace FietsDemo
{
    public partial class MainForm : Form
    {

        private GUI gui;
        public MainForm(GUI gui)
        {
            this.gui = gui;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            gui.BluetoothBike.client.disconnect();
            System.Environment.Exit(0);
            Application.Exit();
        }

        public void setSpeed(double speed)
        {
            SpeedValue.Invoke((MethodInvoker)(() =>
            {
                SpeedValue.Text = speed + " km/h";
            }));
        }

        public void setHeartRate(double heartRate)
        {
            HeartrateValue.Invoke((MethodInvoker)(() =>
            {
                HeartrateValue.Text = heartRate + " bpm";
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
                ResistanceValue.Text = resistance + "%";
            }));
        }

        private void ESPSimulatorButton_Click(object sender, EventArgs e)
        {
            //Changes colors of the selected and non-selected option.
            SoftwareSimulatorButton.BackColor = Color.White;
            ESPSimulatorButton.BackColor = Color.LightBlue;
            gui.stopSimulator();
        }

        private void SoftwareSimulatorButton_Click(object sender, EventArgs e)
        {
            //Changes colors of the selected and non-selected option.
            SoftwareSimulatorButton.BackColor = Color.LightBlue;
            ESPSimulatorButton.BackColor = Color.White;
            gui.startSimulator();
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
            //Checks if text box isn't empty and sends a chatmessage.
            if (chatTextBox.Text != "")
            {
                doctorChat.Items.Add(chatTextBox.Text);
                gui.BluetoothBike.sendPrivateMessage(chatTextBox.Text);
                chatTextBox.Text = "";

            }
        }
    }

}
