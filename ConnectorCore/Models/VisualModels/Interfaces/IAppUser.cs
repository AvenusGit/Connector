using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IAppUser
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public Сredentials? Credentials { get; set; }
        public IEnumerable<Connection> Connections { get; set; }
        public UserSettings? UserSettings { get; set; }
        public AppRoles Role { get; set; }
        public VisualScheme VisualScheme { get; set; }
        public enum AppRoles
        {
            User,
            Support,
            Administrator
        }
    }
}
