using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IFontScheme
    {
        public string Font { get; set; }
        public double? FontMultiplierPercent { get; set; }
    }
}
