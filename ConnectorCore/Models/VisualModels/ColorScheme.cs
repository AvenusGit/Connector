using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ConnectorCore.Models.VisualModels.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConnectorCore.Models.VisualModels
{
    public class ColorScheme : IColorScheme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public ColorProperty? Fone { get; set; }
        public ColorProperty? Accent { get; set; }
        public ColorProperty? SubAccent { get; set; }
        public ColorProperty? Panel { get; set; }
        public ColorProperty? Border { get; set; }
        public ColorProperty? Path { get; set; }
        public ColorProperty? Text { get; set; }
        public ColorProperty? Select { get; set; }
        public ColorProperty? Error { get; set; }
        public ColorProperty? Disable { get; set; }

        public Dictionary<string, IColorProperty> GetColorProperties()
        {
            Dictionary<string, IColorProperty> result = new Dictionary<string, IColorProperty>();
            result.Add("Fone", Fone!);
            result.Add("Accent", Accent!);
            result.Add("SubAccent", SubAccent!);
            result.Add("Panel", Panel!);
            result.Add("Border", Border!);
            result.Add("Path", Path!);
            result.Add("Text", Text!);
            result.Add("Select", Select!);
            result.Add("Error", Error!);
            result.Add("Disable", Disable!);
            return result;
        }
        public ColorScheme Clone()
        {
            return new ColorScheme()
            {
                Fone = Fone,
                Accent = Accent,
                SubAccent = SubAccent,
                Border = Border,
                Path = Path,
                Text = Text,
                Select = Select,
                Error = Error,
                Disable = Disable
            };
        }
    }
}
