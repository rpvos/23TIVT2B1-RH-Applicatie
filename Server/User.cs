using SharedItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
       public class User
    {
        private string name;
        //private birthdate
        private string username;
        private string password;
        private Role role;

        public User(string name, string username, string password, Role role)
        {
            this.name = name;
            this.username = username;
            this.password = password;
            this.role = role;
        }

        public bool checkPassword(string password)
        {
            return password == this.password;
        }

        internal Role getRole()
        {
            return this.role;
        }

        internal object getUsername()
        {
            return this.username;
        }
    }
}
