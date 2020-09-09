using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FietsDemo
{

    class GUI
    {

        private MainForm form;

       public void run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            this.form = new MainForm();

            Application.Run(form);
        }

        public MainForm getForm()
        {
            return this.form;
        }
    }
}
