using ConnectorCore.Models.VisualModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IVisualScheme<ColorSch, FontSch, ColorType>
        where ColorSch : IColorScheme<ColorType>
        where FontSch : IFontScheme
    {
        public ColorSch ColorScheme { get; set; }
        public FontSch FontScheme { get; set; }
    }
}
