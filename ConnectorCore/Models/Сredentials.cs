using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ConnectorCore.Cryptography;

namespace ConnectorCore.Models
{
    public class Credentials
    {
        public Credentials() { }
        public Credentials(string login) { }
        public Credentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
        [XmlIgnore]
        [JsonIgnore]
        public long Id { get; set; }
        public string? Login { get; set; }
        public string? Password {get; set; }

        public bool IsIdentical(Credentials original)
        {
            return Login == original.Login && Password == original.Password;
        }
    }
}
