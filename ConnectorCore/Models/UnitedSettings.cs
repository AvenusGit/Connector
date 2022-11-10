using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ConnectorCore.Cryptography;

namespace ConnectorCore.Models
{
    public class UnitedSettings
    {        
        public const int JwtTokenLifeTimeMinutes = 10;// нельзя ставить меньше 2 минут!
        public bool AllowConnectionInfo { get; set; }
        public bool UsePasswordHash { get; set; }
        public PasswordCryptography.HashTypes HashType { get; set; }
        public bool DoItGood { get; set; }
        // some settings
        public static UnitedSettings GetDefault()
        {
            return new UnitedSettings()
            {
                DoItGood = false,
                AllowConnectionInfo = false,
                UsePasswordHash = true,
                HashType = PasswordCryptography.HashTypes.MD5
            };
        }
    }
}
