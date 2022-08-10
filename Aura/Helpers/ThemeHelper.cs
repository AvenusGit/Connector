using Aura.Models;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Helpers
{
    /// <summary>
    /// Класс занимается загрузкой и сохранением всех визуальных параметров приложения в формате xml.
    /// Если интересует этот функционал рекомендую глянуть контрол VisualSettingsControl.
    /// </summary>
    public static class ThemeHelper
    {
        // базовое расположение директории с темами
        private static string folderPath = Directory.GetCurrentDirectory() + @"\VisualSchemes";

        /// <summary>
        /// Сохранение визуальной схемы в файл.
        /// </summary>
        /// <param name="scheme">Визуальная схема для сохранения</param>
        /// <param name="vsname">Имя схемы</param>
        public static XDocument GetXMLThemeSchemes(ThemeScheme scheme, string vsname)
        {
            XDocument document = new XDocument();
            XElement root = new XElement("VisualScheme");
            XElement name = new XElement("Name", vsname);
            XElement colorScheme = new XElement("ColorScheme");
            XElement colorSchemeName = new XElement("Name", vsname);
            XElement colorSchemeFone = new XElement("FoneColor", scheme.ColorScheme.FoneColor.ToString());
            XElement colorSchemeAccent = new XElement("AccentColor", scheme.ColorScheme.AccentColor.ToString());
            XElement colorSchemeSubAccent = new XElement("SubAccentColor", scheme.ColorScheme.SubAccentColor.ToString());
            XElement colorSchemePanel = new XElement("PanelColor", scheme.ColorScheme.PanelColor.ToString());
            XElement colorSchemePath = new XElement("PathColor", scheme.ColorScheme.PathColor.ToString());
            XElement colorSchemeBorder = new XElement("BorderColor", scheme.ColorScheme.BorderColor.ToString());
            XElement colorSchemeText = new XElement("TextColor", scheme.ColorScheme.TextColor.ToString());
            XElement colorSchemeSelection = new XElement("SelectionColor", scheme.ColorScheme.SelectionColor.ToString());
            XElement colorSchemeError = new XElement("ErrorColor", scheme.ColorScheme.ErrorColor.ToString());
            XElement colorSchemeDisable = new XElement("DisableColor", scheme.ColorScheme.DisableColor.ToString());
            colorScheme.Add(colorSchemeName,
                colorSchemeFone,
                colorSchemeAccent,
                colorSchemeSubAccent,
                colorSchemePanel,
                colorSchemePath,
                colorSchemeBorder,
                colorSchemeText,
                colorSchemeSelection,
                colorSchemeError,
                colorSchemeDisable);

            XElement style = new XElement("Style",
                new XElement("Name", scheme.StyleScheme.Name),
                new XElement("ResourceDictionary", scheme.StyleScheme.ResourceDictionary));

            XElement fontScheme = new XElement("FontScheme",
                new XElement("Name", vsname),
                new XElement("Font", scheme.FontScheme.Font.Source),
                new XElement("Multiplier", scheme.FontScheme.Multiplier.ToString())
            );
            XElement isService = new XElement("IsServiceScheme", scheme.IsServiceScheme.ToString());
            //
            root.Add(name, colorScheme, style, fontScheme, isService);
            document.Add(root);
            return document;
        }

        /// <summary>
        /// Загрузка темы из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static List<ThemeScheme> GetThemeSchemesFromXML(string path)
        {
            List<ThemeScheme> result = new List<ThemeScheme>();
            XDocument document = XDocument.Load(path);
            // для каждой схемы
            foreach (XElement scheme in document.Elements())
            {
                ThemeScheme themeScheme = new ThemeScheme();
                //для каждого элемента в схеме
                foreach (XElement parameter in scheme.Elements())
                {
                    if (parameter.Name == "Name")
                        themeScheme.Name = parameter.Value;
                    if (parameter.Name == "ColorScheme")
                    {
                        themeScheme.ColorScheme = new ColorScheme();
                        foreach (XElement colorSchemeParameter in parameter.Elements())
                        {
                            if (colorSchemeParameter.Name == "Name")
                                themeScheme.ColorScheme.Name = colorSchemeParameter.Value;
                            if (colorSchemeParameter.Name == "FoneColor")
                                themeScheme.ColorScheme.FoneColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "AccentColor")
                                themeScheme.ColorScheme.AccentColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "SubAccentColor")
                                themeScheme.ColorScheme.SubAccentColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "PanelColor")
                                themeScheme.ColorScheme.PanelColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "PathColor")
                                themeScheme.ColorScheme.PathColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "BorderColor")
                                themeScheme.ColorScheme.BorderColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "TextColor")
                                themeScheme.ColorScheme.TextColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "SelectionColor")
                                themeScheme.ColorScheme.SelectionColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "ErrorColor")
                                themeScheme.ColorScheme.ErrorColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                            if (colorSchemeParameter.Name == "DisableColor")
                                themeScheme.ColorScheme.DisableColor =
                                    (Color)ColorConverter.ConvertFromString(colorSchemeParameter.Value);
                        }
                    }
                    if (parameter.Name == "Style")
                    {
                        themeScheme.StyleScheme = new StyleScheme();
                        foreach (XElement styleParameter in parameter.Elements())
                        {
                            if (styleParameter.Name == "Name")
                                themeScheme.StyleScheme.Name = styleParameter.Value;
                            if (styleParameter.Name == "ResourceDictionary")
                                themeScheme.StyleScheme.ResourceDictionary = styleParameter.Value;
                        }
                    }
                    if (parameter.Name == "FontScheme")
                    {
                        themeScheme.FontScheme = new FontScheme();
                        foreach (XElement fontSchemeParameter in parameter.Elements())
                        {
                            if (fontSchemeParameter.Name == "Name")
                                themeScheme.FontScheme.Name = fontSchemeParameter.Value;
                            if (fontSchemeParameter.Name == "Font")
                                themeScheme.FontScheme.Font = new FontFamily(fontSchemeParameter.Value);
                            if (fontSchemeParameter.Name == "Multiplier")
                                themeScheme.FontScheme.Multiplier = double.Parse(fontSchemeParameter.Value);
                        }
                    }
                    if (parameter.Name == "IsServiceScheme")
                        themeScheme.IsServiceScheme = bool.Parse(parameter.Value);
                }
                result.Add(themeScheme);
            }
            return result;
        }

        /// <summary>
        /// Загрузка визуальной схемы из файла. Если тем в файле несколько - загрузит первую.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Лист визуальных схем</returns>
        public static ThemeScheme LoadVisualScheme(string path)
        {
            List<ThemeScheme> vsList = new List<ThemeScheme>();

            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            ThemeScheme scheme = new ThemeScheme();
            XmlElement root = xml.DocumentElement;
            if (root != null)
            {
                foreach (XmlElement xnode in root)
                {
                    ThemeScheme loadedVs = new ThemeScheme();

                    foreach (XmlNode visualSchemeParameter in xnode.ChildNodes)
                    {
                        if (visualSchemeParameter.Name == "Name")
                        {
                            loadedVs.Name = visualSchemeParameter.InnerText;
                        }
                        if (visualSchemeParameter.Name == "ColorScheme")
                        {
                            ColorScheme loadedColorScheme = new ColorScheme();

                            foreach (XmlNode colorSchemeParameter in visualSchemeParameter)
                            {
                                switch (colorSchemeParameter.Name)
                                {
                                    case "Name":
                                        loadedColorScheme.Name = colorSchemeParameter.InnerText;
                                        break;
                                    case "FoneColor":
                                        loadedColorScheme.FoneColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "AccentColor":
                                        loadedColorScheme.AccentColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "SubAccentColor":
                                        loadedColorScheme.SubAccentColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "PanelColor":
                                        loadedColorScheme.PanelColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "PathColor":
                                        loadedColorScheme.PathColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "BorderColor":
                                        loadedColorScheme.BorderColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "TextColor":
                                        loadedColorScheme.TextColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "SelectionColor":
                                        loadedColorScheme.SelectionColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                    case "ErrorColor":
                                        loadedColorScheme.ErrorColor = (Color)ColorConverter.ConvertFromString(colorSchemeParameter.InnerText);
                                        break;
                                }
                            }
                            loadedVs.ColorScheme = loadedColorScheme;
                        }
                        if (visualSchemeParameter.Name == "FontScheme")
                        {
                            FontScheme loadedFontScheme= new FontScheme();

                            foreach (XmlNode loadedFontParameter in visualSchemeParameter)
                            {
                                switch (loadedFontScheme.Name)
                                {
                                    case "Name":
                                        loadedFontScheme.Name = loadedFontParameter.InnerText;
                                        break;
                                    case "Font":
                                        loadedFontScheme.Font = new FontFamily(loadedFontParameter.InnerText);
                                        break;
                                    case "Multiplier":
                                        loadedFontScheme.Multiplier = double.Parse(loadedFontParameter.InnerText);
                                        break;                                    
                                }
                            }
                            loadedVs.FontScheme = loadedFontScheme;
                        }
                        if (visualSchemeParameter.Name == "IsServiceScheme")
                        {
                            loadedVs.IsServiceScheme = bool.Parse(visualSchemeParameter.InnerText);
                        }
                    }
                    vsList.Add(loadedVs);
                }
                return vsList[0];
            }
            else return null;
        }

        /// <summary>
        /// Получение текущей схемы
        /// </summary>
        /// <returns></returns>
        public static ThemeScheme GetCurrentThemeScheme()
        {
            ThemeScheme current = new ThemeScheme();
            current.Name = "current";
            current.ColorScheme = ColorHelper.GetCurrentColorScheme();
            current.FontScheme = FontHelper.GetCurrentFontScheme();
            current.StyleScheme = StyleHelper.GetCurrentStyleScheme();
            current.IsServiceScheme = false;
            return current;
        }

        /// <summary>
        /// Проверка существования базовой директории тем или ее создание
        /// </summary>
        public static void CheckThemeDirectory()
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static List<ThemeScheme> LoadAllUserThemeSchemes()
        {
            List<ThemeScheme> result = new List<ThemeScheme>();
            CheckThemeDirectory();
            string[] fileList = Directory.GetFiles(folderPath);
            foreach (string themeFIleName in fileList)
            {
                if (themeFIleName.EndsWith(".aurascheme"))
                {
                    result.Add(ThemeHelper.GetThemeSchemesFromXML(themeFIleName)[0]);
                }
            }
            return result;
        }
    }
}
