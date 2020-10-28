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
        private GUI gui;
        public SimulationForm(BikeSimulator bikeSimulator, GUI gui)
        {
            this.gui = gui;
            this.BikeSimulator = bikeSimulator;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
            this.HeartrateTextBox.MouseWheel += new MouseEventHandler(changeHeartrate);
            this.SpeedTextBox.MouseWheel += new MouseEventHandler(changeSpeed);            
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this.gui.stopSimulator();
        }

        //Increases speed with 5 when clicked on plus button.
        private void plusSpeed_click(object sender, EventArgs e)
        {
           int i = Int32.Parse(SpeedTextBox.Text);
           i+=5;
            if (i > 40)
            {
                i = 40;
            }
           SpeedTextBox.Text = i + "";
           this.BikeSimulator.setSpeed((byte)i);
        }

        //Decreases speed with 5 when clicked on minus button.
        private void minusSpeed_click(object sender, EventArgs e)
        {
            int i = Int32.Parse(SpeedTextBox.Text);
            i-=5;
            if (i < 0)
            {
                i = 0;
            }
            SpeedTextBox.Text = i + "";
            this.BikeSimulator.setSpeed((byte)i);
        }

        //Increases heartrate with 5 when clicked on plus button.
        private void plusHeartrate_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(HeartrateTextBox.Text);
            i += 5;
            if (i > 228)
            {
                i = 228;
            }
            HeartrateTextBox.Text = i + "";
            this.BikeSimulator.setHeartRate((byte)i);
        }

        //Decreases heartrate with 5 when clicked on minus button.
        private void minHeartrate_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(HeartrateTextBox.Text);
            i -= 5;
            if (i < 50)
            {
                i = 50;
            }
            HeartrateTextBox.Text = i + "";
            this.BikeSimulator.setHeartRate((byte)i);
        }

        //This method handles the scrollwheel when scrolled of the heartrate textbox.
        void changeHeartrate(object sender, MouseEventArgs e)
        {
            //Handles scrollwheel up.
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

            //Handles scrollwheel down.
            else if (e.Delta < 0)
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

        //This method handles the scrollwheel when scrolled of the speed textbox.
        void changeSpeed(object sender, MouseEventArgs e)
        {
            //Handles scrollwheel up.
            if (e.Delta > 0)
            {
                int i = Int32.Parse(SpeedTextBox.Text);
                i++;
                if (i > 40)
                {
                    i = 40;
                }
                SpeedTextBox.Text = i + "";
                this.BikeSimulator.setSpeed((byte)i);

            }

            //Handles scrollwheel down.
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

        public void setResistance(int resistance)
        {
            ResistanceValue.Invoke((MethodInvoker)(() =>
            {
                ResistanceValue.Text = resistance + " %";
            }));
        }

    
    }
}
