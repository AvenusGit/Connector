using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using static ConnectorCore.Interfaces.IUser;

namespace ConnectorCore.Models.Authorization
{
    public class RdpUser : IUser
    {
        public string Name { get; set; }
        public Сredentials Credentials { get; set; }
        public RdpRoles Role { get; set; }
        public enum RdpRoles
        {
            User,
            Support,
            Administrator
        }
    }
}
