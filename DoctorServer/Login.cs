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
            this.loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            //this.loginForm.ShowDialog();
        }

        public void run()
        {
            Application.Run(this.loginForm);

        }

        public void loginFailed()
        {
            this.loginForm.loginFailed();
        }

        public void loginSucceeded()
        {
            this.loginForm.loginSucceeded();
        }
    }
}
