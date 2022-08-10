using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Authorization;

namespace ConnectorCore.Interfaces
{
    public interface IUser
    {
        public string Name { get; set; }
        public Сredentials Credentials { get; set; }
    }
}
