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
        public bool loggedIn { get; set; }

        public User(string name, string username, string password, Role role)
        {
            this.loggedIn = false;
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

        internal string getUsername()
        {
            return this.username;
        }
    }
}
