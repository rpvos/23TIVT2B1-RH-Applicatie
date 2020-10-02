using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.form = new SimulationForm(this.bikeSimulator);

            Application.Run(form);
        }

        public SimulationForm getForm()
        {
            return this.form;
        }

        public void setResistance(int resistance)
        {
            this.form.setResistance(resistance);
        }

    }
}
