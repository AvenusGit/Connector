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
        public long VisualSchemeId { get; set; }
        public VisualScheme VisualScheme { get; set; }
        public string Fone { get; set; }
        public string Accent { get; set; }
        public string SubAccent { get; set; }
        public string Panel { get; set; }
        public string Border { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }
        public string Select { get; set; }
        public string Error { get; set; }
        public string Disable { get; set; }
        public static ColorScheme GetDefault()
        {
            return new ColorScheme()
            {
                Accent = "#363636FF",
                Fone = "#FFFFFFFF",
                SubAccent = "#ADADADFF",
                Panel = "#FFFFFFFF",
                Border = "#DEDEDEFF",
                Path = "#3B3B3BFF",
                Text = "#3B3B3BFF",
                Select = "#A0A0A0FF",
                Error = "#FF0000AA",
                Disable = "#F2F2F2FF"
            };
        }
        private string WpfToCssColor(string colorName, string wpfColor)
        {
            if (!IColorScheme.IsValueCorrect(wpfColor))
                throw new Exception($"Ошибка при попытке перевода WPF цвета <{colorName}> в HEX. Строка цвета не валидна");
            string opacity = wpfColor.Substring(1, 2);
            wpfColor = wpfColor.Remove(1, 2);
            return wpfColor + opacity;
        }
        private string CssToWpfColor(string colorName, string wpfColor)
        {
            if (!IColorScheme.IsValueCorrect(wpfColor))
                throw new Exception($"Ошибка при попытке перевода HEX цвета <{colorName}> в WPF. Строка цвета не валидна");
            string opacity = wpfColor.Substring(wpfColor.Length - 2, 2);
            wpfColor = wpfColor.Remove(wpfColor.Length - 2, 2);
            wpfColor = wpfColor.Insert(1, opacity);
            return wpfColor + opacity;
        }
        public Dictionary<string, string> GetColorProperties()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
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
