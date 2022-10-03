using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IColorScheme
    {
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

        public static bool IsValueCorrect(string color)
        {
            return !String.IsNullOrWhiteSpace(color) || Regex.IsMatch(color, "#[0-9a-fA-F]{8}");
        }
        public Dictionary<string, string> GetColorProperties();
        public static string ColorFieldName(string colorName)
        {
            switch (colorName)
            {
                case "Fone":
                    return "Фон";
                case "Accent":
                    return "Акцент";
                case "SubAccent":
                    return "Субакцент";
                case "Panel":
                    return "Панели";
                case "Border":
                    return "Границы";
                case "Path":
                    return "Иконки";
                case "Text":
                    return "Текст";
                case "Select":
                    return "Выделение";
                case "Error":
                    return "Ошибка";
                case "Disable":
                    return "Выключено";
                default:
                    return string.Empty;
            }
        }
    }
}
