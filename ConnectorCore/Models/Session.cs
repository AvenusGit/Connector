using ConnectorCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class Session
    {
        public Session() { }
        public TokenInfo? Token { get; set; }
        public AppUser? User { get; set; }
        public UnitedSettings? UnitedSettings { get; set; }
    }
}
