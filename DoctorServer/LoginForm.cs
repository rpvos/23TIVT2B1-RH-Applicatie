using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoctorApplication;

namespace DoctorServer
{
    public partial class LoginForm : Form
    {
        private DoctorClient doctorClient;
        public LoginForm(DoctorClient doctorClient)
        {
            this.doctorClient = doctorClient;
            InitializeComponent();
            
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            this.doctorClient.startClient(UsernameTextbox.Text, PasswordTextbox.Text);
            this.Hide();

        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
