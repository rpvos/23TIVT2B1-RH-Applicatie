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
        public Login(Program program)
        {
            this.loginForm = new LoginForm(program);
            Application.Run(this.loginForm);
        }
    }
}
