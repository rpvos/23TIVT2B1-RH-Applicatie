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
            this.resistanceTextbox.MouseWheel += new MouseEventHandler(changeResistance);

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
                ResistanceValue.Text = resistance + " %";
            }));
        }


        public void changeResistance(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                double i = Int32.Parse(resistanceTextbox.Text);
                i++;
                if (i > 100)
                {
                    i = 100;
                }
                resistanceTextbox.Text = i + "";
                



            }
            else if (e.Delta < 0)
            {
                double i = Int32.Parse(resistanceTextbox.Text);
                i--;
                if (i < 0)
                {
                    i = 0;
                }
                resistanceTextbox.Text = i + "";



            }

        }

        private void minResistance_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(resistanceTextbox.Text);
            i -= 5;
            if (i < 0)
            {
                i = 0;
            }
            resistanceTextbox.Text = i + "";
        }

        private void plusResistance_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(resistanceTextbox.Text);
            i += 5;
            if (i > 100)
            {
                i = 100;
            }
            resistanceTextbox.Text = i + "";
        }
    }
}
