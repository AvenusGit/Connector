using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AuraS.Interfaces
{
    public interface IColorProperty
    {
        public string Name { get; set; }
        public string ColorKeyName { get { return Name + "Color"; } }
        public Color? ColorValue { get; set; }
    }
}
