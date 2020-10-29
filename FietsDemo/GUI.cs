using System.Windows.Forms;


namespace FietsDemo
{
    public class GUI
    {

        private MainForm Form;
        public BluetoothBike BluetoothBike { get; set; }

        public GUI(BluetoothBike program)
        {
            BluetoothBike = program;
        }

        public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form = new MainForm(this);

            Application.Run(Form);
        }

        public MainForm getForm()
        {
            return Form;
        }

        public void stopSimulator()
        {
            BluetoothBike.stopSimulator();
        }

        public void startSimulator()
        {
            BluetoothBike.startSimulator();
        }

        public void setResistance(int resistance)
        {
            Form.setResistance(resistance);
        }

        public void addTextMessage(string message)
        {
            Form.addMessage(message);
        }


    }
}
