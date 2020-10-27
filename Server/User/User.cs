﻿using Newtonsoft.Json;
using SharedItems;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Server
{
    public class User
    {
        public string name { get; set; }
        //private birthdate
        public string username { get; set; }
        public string password { get; set; }
        public Role role { get; set; }
        public bool loggedIn { get; set; }
        public UserDataStorage userDataStorage { get; set; }

        public User()
        {

        }

        public User(string name, string username, string password, Role role)
        {
            this.loggedIn = false;
            this.name = name;
            this.username = username;
            this.password = password;
            this.role = role;
            this.userDataStorage = new UserDataStorage();
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
        public string GetSaveFormat()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
