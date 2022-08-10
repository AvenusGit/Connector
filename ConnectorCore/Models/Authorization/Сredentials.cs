using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.Authorization
{
    public class Сredentials
    {
        public Сredentials(string login)
        {
            Login = login;
        }
        public Сredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public string Login { get; set; }
        public string? Password { get; set; }
    }
}
