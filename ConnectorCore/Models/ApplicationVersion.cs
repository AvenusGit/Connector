using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class ApplicationVersion
    {
        public string VersionSeries { get; set; }
        public int VersionNumber { get; set; }
        public override string ToString()
        {
            return VersionSeries + VersionNumber.ToString();
        }
    }
}
