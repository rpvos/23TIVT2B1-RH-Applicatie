using System.Windows.Forms;


namespace FietsDemo
{
    public class GUI
    {

        private MainForm form;
        public BluetoothBike bluetoothBike { get; set; }

        public GUI(BluetoothBike program)
        {
            bluetoothBike = program;
        }

        public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new MainForm(this);

            Application.Run(form);
        }

        public MainForm getForm()
        {
            return form;
        }

        public void stopSimulator()
        {
            bluetoothBike.stopSimulator();
        }

        public void startSimulator()
        {
            bluetoothBike.startSimulator();
        }

        public void setResistance(int resistance)
        {
            form.setResistance(resistance);
        }

        public void addTextMessage(string message)
        {
            form.addMessage(message);
        }


    }
}
