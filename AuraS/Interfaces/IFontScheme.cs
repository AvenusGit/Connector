using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AuraS.Interfaces
{
    public interface IFontScheme : IScheme<IFontScheme>
    {
        public FontFamily Font { get; set; }
        public double? FontMultiplierPercent { get; set; }
    }
}
