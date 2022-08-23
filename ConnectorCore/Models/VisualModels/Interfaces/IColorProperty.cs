using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IColorProperty
    {
        public string Name { get; set; }
        public string ColorKeyName { get { return Name + "Color"; } }
        public string Color { get; set; }
    }
}
