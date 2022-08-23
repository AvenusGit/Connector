using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IVisualScheme<ColorSch,FontShm>
        where ColorSch : IColorScheme
        where FontShm : IFontScheme
    {
        public ColorSch ColorScheme { get; set; }
        public FontShm FontScheme { get; set; }
    }
}
