using System;
using System.Windows.Forms;

namespace FietsDemo
{
    public partial class LoginForm : Form
    {
        private BluetoothBike bluetoothBike;
        public LoginForm(BluetoothBike bluetoothBike)
        {
            this.bluetoothBike = bluetoothBike;
            InitializeComponent();
            this.bluetoothBike.startClient();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            if (UsernameTextbox.Text != "" && PasswordTextbox.Text != "")
            {
                bluetoothBike.client.sendUserCredentials(UsernameTextbox.Text, PasswordTextbox.Text);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void loginFailed()
        {

            UsernameTextbox.Invoke((MethodInvoker)(() =>
            {
                UsernameTextbox.Text = "Login failed";
                PasswordTextbox.Text = "";
            }));


        }

        public void loginSucceeded()
        {

            Invoke((MethodInvoker)(() =>
            {
                Hide();
            }));
        }


    }
}
