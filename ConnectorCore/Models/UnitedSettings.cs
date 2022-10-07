using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConnectorCore.Models
{
    public class UnitedSettings
    {
        public bool DoItGood { get; set; }
        // some settings
        public static UnitedSettings GetDefault()
        {
            return new UnitedSettings()
            {
                DoItGood = false
            };
        }
    }
}
