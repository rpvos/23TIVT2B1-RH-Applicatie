using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FietsDemo
{
    class Simulator
    {

        private SimulationForm form;

        public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.form = new SimulationForm();

            Application.Run(form);
        }

        public SimulationForm getForm()
        {
            return this.form;
        }
    }
}
