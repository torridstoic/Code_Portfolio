using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Data
{
    public class User : Table
    {
        int id;
        int lastLogin;
        String username;
        String email;
        String firstName;
        String lastName;
        String password;

        public User(String user, String Pass, String Email = "") : base("Users")
        {
            Username = user;
            Password = Pass;
        }

        public User(ResultData data) : base("Users")
        {
            this.SetData(data);
        }

        private void SetData(ResultData data)
        {
            this.id = Convert.ToInt32(data.Value("Id"));
            this.lastLogin = Convert.ToInt32(data.Value("LastLogin"));
            this.email = Convert.ToString(data.Value("Email"));
            this.firstName = Convert.ToString(data.Value("FirstName"));
            this.lastName = Convert.ToString(data.Value("LastName"));
            this.username = Convert.ToString(data.Value("Username"));
            this.password = Convert.ToString(data.Value("Password"));
        }
        
        public override int Id
        {
            get { return this.id; }
        }
        public int LastLogin
        {
            get { return this.lastLogin; }
            set { this.lastLogin = value; }
        }
        public String Email
        {
            get { return this.email; }
            set { this.email = value; }
        }
        public String FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }
        public String LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }
        public String Password
        {
            get { return this.password; }
            set { this.password = Hash(value); }
        }
        public String Username
        {
            get { return this.username; }
            set { this.username = value; }
        }
        public String Hash(String data)
        {
            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(result);
        }
        public Boolean PasswordValidate(String password)
        {
            if (this.Id > 0)
            {
                if (this.Password == Hash(password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        protected override void LoadFields()
        {
            AddField("FirstName");
            AddField("LastName");
            AddField("Email");
            AddField("LastLogin");
            AddField("Password");
            AddField("Username");
        }

        protected override List<Object> getValues()
        {
            List<Object> values = new List<Object>();

            foreach (String field in fields)
            {
                Object value = this.GetType().GetProperty(field).GetValue(this);
                values.Add(value);
            }

            return values;
        }

        public override bool Validation()
        {
            if (FirstName == String.Empty || LastName == String.Empty || Password == String.Empty || Username == String.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
