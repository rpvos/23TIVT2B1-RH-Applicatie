using System.Windows.Forms;

namespace FietsDemo
{
    internal class Login
    {
        private LoginForm loginForm;
        public Login(BluetoothBike bluetoothBike)
        {
            loginForm = new LoginForm(bluetoothBike);
            loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        public void run()
        {
            Application.Run(loginForm);

        }

        public void loginFailed()
        {
            loginForm.loginFailed();
        }

        public void loginSucceeded()
        {
            loginForm.loginSucceeded();
        }
    }
}
