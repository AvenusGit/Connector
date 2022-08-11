using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class Сredentials
    {
        public Сredentials() { }
        public Сredentials(string login) { }
        public Сredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public long Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
