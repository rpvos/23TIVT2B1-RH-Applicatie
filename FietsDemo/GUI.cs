using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FietsDemo
{

    public class GUI
    {

        private MainForm form;
        public BluetoothBike bluetoothBike { get; set; }

        public GUI(BluetoothBike program)
        {
            this.bluetoothBike = program;
        }

        public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.form = new MainForm(this);

            Application.Run(form);
        }

        public MainForm getForm()
        {
            return this.form;
        }

        public void stopSimulator()
        {
            this.bluetoothBike.stopSimulator();
        }

        public void startSimulator()
        {
            this.bluetoothBike.startSimulator();
        }

        public void setResistance(int resistance)
        {
            this.form.setResistance(resistance);
        }

        public void addTextMessage(string message)
        {
            this.form.addMessage(message);
        }


    }
}
