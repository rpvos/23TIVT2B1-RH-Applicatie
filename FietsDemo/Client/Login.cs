using FietsDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FietsDemo
{
    class Login
    {
        private LoginForm loginForm;
        public Login(BluetoothBike bluetoothBike)
        {
            this.loginForm = new LoginForm(bluetoothBike);
            this.loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
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
