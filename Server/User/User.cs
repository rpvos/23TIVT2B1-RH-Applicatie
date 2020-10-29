using Newtonsoft.Json;
using SharedItems;

namespace Server
{
    /// <summary>
    /// Collection of the user data
    /// </summary>
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
            loggedIn = false;
            this.name = name;
            this.username = username;
            this.password = password;
            this.role = role;
            userDataStorage = new UserDataStorage();
        }

        public bool checkPassword(string password)
        {
            return password == this.password;
        }

        internal Role getRole()
        {
            return role;
        }

        internal string getUsername()
        {
            return username;
        }
        public string GetSaveFormat()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
