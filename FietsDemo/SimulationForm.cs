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
    public partial class SimulationForm : Form
    {
        private BikeSimulator BikeSimulator;
        public SimulationForm(BikeSimulator bikeSimulator)
        {
            this.BikeSimulator = bikeSimulator;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
            this.HeartrateTextBox.MouseWheel += new MouseEventHandler(changeHeartrate);
            this.SpeedTextBox.MouseWheel += new MouseEventHandler(changeSpeed);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           int i = Int32.Parse(SpeedTextBox.Text);
           i+=5;
            if (i > 144)
            {
                i = 144;
            }
           SpeedTextBox.Text = i + "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(SpeedTextBox.Text);
            i-=5;
            if (i < 0)
            {
                i = 0;
            }
            SpeedTextBox.Text = i + "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void plusHeartrate_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(HeartrateTextBox.Text);
            i += 5;
            if (i > 228)
            {
                i = 228;
            }
            HeartrateTextBox.Text = i + "";
        }

        private void minHeartrate_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(HeartrateTextBox.Text);
            i -= 5;
            if (i < 50)
            {
                i = 50;
            }
            HeartrateTextBox.Text = i + "";
        }

        void changeHeartrate(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int i = Int32.Parse(HeartrateTextBox.Text);
                i++;
                if (i > 228)
                {
                    i = 228;
                }
                HeartrateTextBox.Text = i + "";
                this.BikeSimulator.setHeartRate((byte)i);


            }
            else if(e.Delta < 0)
            {
                int i = Int32.Parse(HeartrateTextBox.Text);
                i--;
                if (i < 50)
                {
                    i = 50;
                }
                HeartrateTextBox.Text = i + "";
                this.BikeSimulator.setHeartRate((byte)i);


            }

        }

        void changeSpeed(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int i = Int32.Parse(SpeedTextBox.Text);
                i++;
                if (i > 144)
                {
                    i = 144;
                }
                SpeedTextBox.Text = i + "";
                this.BikeSimulator.setSpeed((byte)i);

            }
            else if (e.Delta < 0)
            {
                int i = Int32.Parse(SpeedTextBox.Text);
                i--;
                if (i < 0)
                {
                    i = 0;
                }
                SpeedTextBox.Text = i + "";
                this.BikeSimulator.setSpeed((byte)i);


            }

        }



    }
}
