using System.Windows.Forms;


namespace FietsDemo
{
    public class Simulator
    {

        private SimulationForm form;
        private BikeSimulator bikeSimulator;

        public Simulator(BikeSimulator bikeSimulator)
        {
            this.bikeSimulator = bikeSimulator;
        }

        public void run(GUI gui)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new SimulationForm(bikeSimulator, gui);

            Application.Run(form);
        }

        public SimulationForm getForm()
        {
            return form;
        }

        public void setResistance(int resistance)
        {
            form.setResistance(resistance);
        }

    }
}
