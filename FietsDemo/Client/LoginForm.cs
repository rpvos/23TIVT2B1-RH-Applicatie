using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            if(UsernameTextbox.Text != "" && PasswordTextbox.Text != "")
            this.bluetoothBike.startClient(UsernameTextbox.Text, PasswordTextbox.Text);
            
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void loginFailed()
        {

            UsernameTextbox.Invoke((MethodInvoker)(() =>
            {
                this.UsernameTextbox.Text = "Login failed";
                this.PasswordTextbox.Text = "";
            }));
           

        }

        public void loginSucceeded()
        {

            this.Invoke((MethodInvoker)(() =>
            {
                this.Hide();
            }));
        }

     
    }
}
