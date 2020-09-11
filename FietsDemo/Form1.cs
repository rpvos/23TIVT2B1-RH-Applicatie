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

        public MainForm()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void setSpeed(double speed)
        {
            SpeedValue.Invoke((MethodInvoker)(() =>
            {
                SpeedValue.Text = speed + " km/h";
            }));
        }

        public void setHeartrate(String heartrate)
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

    }
}
