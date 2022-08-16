using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class UserSettings
    {
        public long Id { get; set; }
        public static UserSettings GetDefault()
        {
            return new UserSettings();
        }
    }
}
