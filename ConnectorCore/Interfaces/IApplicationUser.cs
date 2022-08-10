using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Interfaces
{
    public interface IApplicationUser : IUser
    {
        public IEnumerable<IConnection> Connections { get; set; }
        public IUserSettings UserSettings { get; set; }
        public enum AppRoles
        {
            User,
            Support,
            Administrator
        }
    }
}
