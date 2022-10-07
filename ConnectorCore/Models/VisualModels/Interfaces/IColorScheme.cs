using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConnectorCore.Models.VisualModels.Interfaces
{
    public interface IColorScheme<colorType>
    {
        public colorType Fone { get; set; }
        public colorType Accent { get; set; }
        public colorType SubAccent { get; set; }
        public colorType Panel { get; set; }
        public colorType Border { get; set; }
        public colorType Path { get; set; }
        public colorType Text { get; set; }
        public colorType Select { get; set; }
        public colorType Error { get; set; }
        public colorType Disable { get; set; }

        public static bool IsValueCorrect(string color)
        {
            return !String.IsNullOrWhiteSpace(color) || Regex.IsMatch(color, "#[0-9a-fA-F]{8}");
        }
        public Dictionary<string, colorType> GetColorProperties();
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
