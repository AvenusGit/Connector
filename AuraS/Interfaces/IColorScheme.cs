using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraS.Interfaces
{
    public interface IColorScheme : IScheme<IColorScheme>
    {
        public IColorProperty Fone { get; set; }
        public IColorProperty Accent { get; set; }
        public IColorProperty SubAccent { get; set; }
        public IColorProperty Panel { get; set; }
        public IColorProperty Border { get; set; }
        public IColorProperty Path { get; set; }
        public IColorProperty Text { get; set; }
        public IColorProperty Select { get; set; }
        public IColorProperty Error { get; set; }
        public IColorProperty Disable { get; set; }

        public void Apply();
    }
}
