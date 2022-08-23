using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IColorScheme
    {
        public ColorProperty Fone { get; set; }
        public ColorProperty Accent { get; set; }
        public ColorProperty SubAccent { get; set; }
        public ColorProperty Panel { get; set; }
        public ColorProperty Border { get; set; }
        public ColorProperty Path { get; set; }
        public ColorProperty Text { get; set; }
        public ColorProperty Select { get; set; }
        public ColorProperty Error { get; set; }
        public ColorProperty Disable { get; set; }
    }
}
