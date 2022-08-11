using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class ServerUser
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public Сredentials? Credentials { get; set; }
        public Roles Role { get; set; }
        public enum Roles
        {
            User,
            Support,
            Administrator
        }
    }
}
