using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraS.Interfaces
{
    public interface IVisualScheme : IScheme<IVisualScheme>
    {
        public IColorScheme ColorScheme { get; set; }
        public IFontScheme FontScheme { get; set; }
    }
}
