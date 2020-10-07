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
        private Program program;
        public LoginForm(Program program)
        {
            this.program = program;
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Epstein didn't kill himself");
            this.Hide();
            this.program.start();
        }
    }
}
