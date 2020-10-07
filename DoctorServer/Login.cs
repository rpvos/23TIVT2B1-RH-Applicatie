using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoctorServer
{
    class Login
    {
        private LoginForm loginForm;
        public Login(DoctorClient doctorServer)
        {
            this.loginForm = new LoginForm(doctorServer);
            //this.loginForm.ShowDialog();
            Application.Run(this.loginForm);
        }
    }
}
