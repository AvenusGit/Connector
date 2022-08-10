using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using System.Linq;
using System.Collections.Generic;
using Aura.Models;
using Aura.Helpers;
using Microsoft.Win32;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace Aura.Controls
{
    /// <summary>
    /// Контрол для управления темами и цветовыми схемами
    /// </summary>
    public partial class AuraColorSettings : UserControl
    {
        // шаг изменения размера шрифта (например 0,05 == 5%)
        private double _changeStep = 0.05;

        // доступные для контрола словари, в них необходимо инициализировать доступные схемы перед показом контрола
        List<ColorScheme> _availableColorScheme = new List<ColorScheme>();
        List<FontScheme> _availableFontScheme = new List<FontScheme>();
        List<ThemeScheme> _availableThemeScheme = new List<ThemeScheme>();
        List<StyleScheme> _availableStyleScheme = new List<StyleScheme>();

        // базовое расположение директории с темами
        string folderPath = Directory.GetCurrentDirectory() + @"\VisualSchemes";
        //служебная переменная для определения програмно ли меняется параметр или это делает пользователь из интерфейса
        private bool _autoChanges = false;
        //буфер цветовой темы для аварийного режима
        private ColorScheme buffer = null;

        /// <summary>
        /// Конструктор AuraColorSettings
        /// </summary>
        public AuraColorSettings()
        {
            InitializeComponent();
            ReInitialize();
            Theme.SelectedIndex = 0;
            FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";
        }

       
        /// <summary>
        /// Перезагрузка всех элементов интерфейса
        /// </summary>
        public void ReInitialize()
        {
            //Очистка хранилищ текущих схем
            _availableThemeScheme.Clear();
            _availableColorScheme.Clear();
            _availableFontScheme.Clear();


            // добавление встроенных тем
            _availableThemeScheme.AddRange(Schemes.ThemeSchemes.DefaultThemeShemes.DefaultThemeSchemes());
            // добавление пользовательских тем из папки
            _availableThemeScheme.AddRange(GetUserThemeSchemes());
            // обновление комбобокса тем
            Theme.ItemsSource = _availableThemeScheme;
            Theme.Items.Refresh();

            // обновление хранилища стилей
            _availableStyleScheme = Aura.Schemes.Styles.StyleSchemes.GetApplicationStyleScheme();
            Styles.ItemsSource = _availableStyleScheme;

            // добавление встроенных схем из темы
            foreach (ThemeScheme scheme in _availableThemeScheme)
            {
                _availableColorScheme.Add(scheme.ColorScheme);
                _availableFontScheme.Add(scheme.FontScheme);
            }
            // обновление комбобокса цветовых схем
            Scheme.ItemsSource = _availableColorScheme;
            Scheme.Items.Refresh();

            // добавление системных шрифтов
            foreach (FontFamily fontFamily in GetOnlyCyrillicFontFamyly(System.Windows.Media.Fonts.SystemFontFamilies))
            {
                _availableFontScheme.Add(
                    new FontScheme
                    {
                        Name = fontFamily.Source,
                        Font = fontFamily,
                        Multiplier = 100,
                        IsServiceScheme = false
                    });

            }

            //обновление комбобокса шрифтовых схем
            Font.ItemsSource = _availableFontScheme;
            Font.Items.Refresh();
            // обновление колорпикеров по назначенную тему
            UpdateColorPickers();
        }

        /// <summary>
        /// Метод фильтрует список шрифтов, оставляя только шрифты с поддержкой кириллицы
        /// </summary>
        /// <param name="fontFamilyList">Исходный лист шрифтов</param>
        /// <returns>Лист шрифтов с кириллицей</returns>
        private List<FontFamily> GetOnlyCyrillicFontFamyly(ICollection<FontFamily> fontFamilyList)
        {
            var cyrillicFamilies = new List<FontFamily>();
            const char RUS_CHAR = 'ъ';
            var lang = System.Windows.Markup.XmlLanguage.GetLanguage("en-us");

            foreach (System.Windows.Media.FontFamily fontFamily in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                ICollection<Typeface> typeFaces = fontFamily.GetTypefaces();
                foreach (Typeface typeFace in typeFaces)
                {
                    GlyphTypeface glyph;
                    if (!typeFace.TryGetGlyphTypeface(out glyph)) continue;
                    ushort temporary;
                    if (glyph.CharacterToGlyphMap.TryGetValue((int)RUS_CHAR, out temporary))
                    {
                        cyrillicFamilies.Add(fontFamily);
                        break;
                    }
                }
            }
            return cyrillicFamilies;
        }

        /// <summary>
        /// Обновляет текущие значения ColorPicker до актуальных
        /// </summary>
        private void UpdateColorPickers(ColorScheme newColorScheme = null)
        {
            if(newColorScheme is null)
            {
                newColorScheme = Aura.Helpers.ColorHelper.GetCurrentColorScheme();
            }

            if (Fone != null)
            {                
                Fone.SelectedColor = newColorScheme.FoneColor;
                Accent.SelectedColor = newColorScheme.AccentColor;
                SubAccent.SelectedColor = newColorScheme.SubAccentColor;
                Panel.SelectedColor = newColorScheme.PanelColor;
                Border.SelectedColor = newColorScheme.BorderColor;
                Path.SelectedColor = newColorScheme.PathColor;
                Text.SelectedColor = newColorScheme.TextColor;
                Select.SelectedColor = newColorScheme.SelectionColor;
                Error.SelectedColor = newColorScheme.ErrorColor;
                Disable.SelectedColor = newColorScheme.DisableColor;
            }
        }

        /// <summary>
        /// Событие изменения цвета на ColorPicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerColorChanged(object sender, RoutedEventArgs e)
        {
            if (!_autoChanges)
            {                
                Scheme.SelectedIndex = -1;
                Theme.SelectedIndex = -1;
            }
            switch ((sender as ColorPicker).Name)
            {
                case "Fone":
                    Application.Current.Resources["CFone"] = Fone.SelectedColor.Value;
                    Application.Current.Resources["FoneBrush"] = new SolidColorBrush(Fone.SelectedColor.Value);
                    break;
                case "Accent":
                    Application.Current.Resources["CAccent"] = Accent.SelectedColor.Value;
                    Application.Current.Resources["AccentBrush"] = new SolidColorBrush(Accent.SelectedColor.Value);
                    break;
                case "SubAccent":
                    Application.Current.Resources["CSubAccent"] = SubAccent.SelectedColor.Value;
                    Application.Current.Resources["SubAccentBrush"] = new SolidColorBrush(SubAccent.SelectedColor.Value);
                    break;
                case "Border":
                    Application.Current.Resources["CBorder"] = Border.SelectedColor.Value;
                    Application.Current.Resources["BorderBrush"] = new SolidColorBrush(Border.SelectedColor.Value);
                    break;
                case "Panel":
                    Application.Current.Resources["CPanel"] = Panel.SelectedColor.Value;
                    Application.Current.Resources["PanelBrush"] = new SolidColorBrush(Panel.SelectedColor.Value);
                    break;
                case "Path":
                    Application.Current.Resources["CPath"] = Path.SelectedColor.Value;
                    Application.Current.Resources["PathBrush"] = new SolidColorBrush(Path.SelectedColor.Value);
                    break;
                case "Text":
                    Application.Current.Resources["CText"] = Text.SelectedColor.Value;
                    Application.Current.Resources["TextBrush"] = new SolidColorBrush(Text.SelectedColor.Value);
                    break;
                case "Select":
                    Application.Current.Resources["CSelect"] = Select.SelectedColor.Value;
                    Application.Current.Resources["SelectBrush"] = new SolidColorBrush(Select.SelectedColor.Value);
                    break;
                case "Error":
                    Application.Current.Resources["CError"] = Error.SelectedColor.Value;
                    Application.Current.Resources["ErrorBrush"] = new SolidColorBrush(Error.SelectedColor.Value);
                    break;
                case "Disable":
                    Application.Current.Resources["CDisable"] = Disable.SelectedColor.Value;
                    Application.Current.Resources["DisableBrush"] = new SolidColorBrush(Disable.SelectedColor.Value);
                    break;
            }
        }

        /// <summary>
        /// Событие изменения цветовой схемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeChange(object sender, EventArgs e)
        {  
            if(Scheme.SelectedIndex != -1)
            { 
                _autoChanges = true;            
                if (Scheme.SelectedItem != null)
                {
                        ((ColorScheme)Scheme.SelectedItem).Apply();
                        UpdateColorPickers();
                        _autoChanges = false;
                }
                if (Scheme.SelectedIndex != Theme.SelectedIndex)
                    Theme.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Событие изменения шрифта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FontChange(object sender, EventArgs e)
        {
            if (Font.SelectedIndex != -1)
            {
                ((FontScheme)Font.SelectedItem).Apply();
                SetFontSizeMode(((FontScheme)Font.SelectedItem).Multiplier);
                FontSizeForUser.Text = (((FontScheme)Font.SelectedItem).Multiplier.ToString() + "%");

                if (Font.SelectedIndex != Theme.SelectedIndex)
                    Theme.SelectedIndex = -1;

                SetFontSizeMode((double)Application.Current.Resources["FontMultiplier"]);
                FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";
            }
        }

        /// <summary>
        /// Событие смены темы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Theme.SelectedIndex != -1)
            {
                _autoChanges = true;
                ((ThemeScheme)Theme.SelectedItem).Apply();
                Font.SelectedIndex = Theme.SelectedIndex;
                Scheme.SelectedIndex = Theme.SelectedIndex;
                Styles.SelectedItem = _availableStyleScheme
                    .Where(style => style.Name == ((ThemeScheme)Theme.SelectedItem).StyleScheme.Name).FirstOrDefault();
                _autoChanges = false;
                UpdateColorPickers();
            }
        }

        /// <summary>
        /// Включение аварийного режима
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertMode_Checked(object sender, RoutedEventArgs e)
        {
            buffer = Aura.Helpers.ColorHelper.GetCurrentColorScheme();
            Aura.Schemes.ColorsSchemes.ServiceColorShemes.ErrorColorScheme().Apply();
        }

        /// <summary>
        /// Выключение аварийного режима
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (buffer != null)
                buffer.Apply();
            else
                Aura.Schemes.ColorsSchemes.DefaultColorShemes.StandartColorScheme().Apply();
            buffer = null;
        }

        /// <summary>
        /// Кнопка увеличения шрифта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FonSizePlusButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["FontMultiplier"] = (double)Application.Current.Resources["FontMultiplier"] + (_changeStep * 100);
            Application.Current.Resources["FontSize8"] = (double)Application.Current.Resources["FontSize8"] * (1 + _changeStep);
            Application.Current.Resources["FontSize12"] = (double)Application.Current.Resources["FontSize12"] * (1 + _changeStep);
            Application.Current.Resources["FontSize14"] = (double)Application.Current.Resources["FontSize14"] * (1 + _changeStep);
            Application.Current.Resources["FontSize16"] = (double)Application.Current.Resources["FontSize16"] * (1 + _changeStep);
            Application.Current.Resources["FontSize18"] = (double)Application.Current.Resources["FontSize18"] * (1 + _changeStep);
            Application.Current.Resources["FontSize20"] = (double)Application.Current.Resources["FontSize20"] * (1 + _changeStep);
            Application.Current.Resources["FontSize24"] = (double)Application.Current.Resources["FontSize24"] * (1 + _changeStep);
            Application.Current.Resources["FontSize36"] = (double)Application.Current.Resources["FontSize36"] * (1 + _changeStep);
            Application.Current.Resources["FontSize48"] = (double)Application.Current.Resources["FontSize48"] * (1 + _changeStep);
            SetFontSizeMode((double)Application.Current.Resources["FontMultiplier"]);
            FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";
        }

        /// <summary>
        /// Изменение множителя размера шрифта
        /// </summary>
        private void ChangeMultiplier(double newValue)
        {
            double multiplier = newValue / 100;
            Application.Current.Resources["FontMultiplier"] = newValue;
            Application.Current.Resources["FontSize8"] = (double)Application.Current.Resources["FontSize8"] * multiplier;
            Application.Current.Resources["FontSize12"] = (double)Application.Current.Resources["FontSize12"] * multiplier;
            Application.Current.Resources["FontSize14"] = (double)Application.Current.Resources["FontSize14"] * multiplier;
            Application.Current.Resources["FontSize16"] = (double)Application.Current.Resources["FontSize16"] * multiplier;
            Application.Current.Resources["FontSize18"] = (double)Application.Current.Resources["FontSize18"] * multiplier;
            Application.Current.Resources["FontSize20"] = (double)Application.Current.Resources["FontSize20"] * multiplier;
            Application.Current.Resources["FontSize24"] = (double)Application.Current.Resources["FontSize24"] * multiplier;
            Application.Current.Resources["FontSize36"] = (double)Application.Current.Resources["FontSize36"] * multiplier;
            Application.Current.Resources["FontSize48"] = (double)Application.Current.Resources["FontSize48"] * multiplier;
            SetFontSizeMode((double)Application.Current.Resources["FontMultiplier"]);
            FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";
        }

        /// <summary>
        /// Кнопка уменьшения шрифта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FonSizeMinusButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["FontMultiplier"] = (double)Application.Current.Resources["FontMultiplier"] - (_changeStep * 100);
            Application.Current.Resources["FontSize8"] = (double)Application.Current.Resources["FontSize8"] * (1 - _changeStep);
            Application.Current.Resources["FontSize12"] = (double)Application.Current.Resources["FontSize12"] * (1 - _changeStep);
            Application.Current.Resources["FontSize14"] = (double)Application.Current.Resources["FontSize14"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize16"] = (double)Application.Current.Resources["FontSize16"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize18"] = (double)Application.Current.Resources["FontSize18"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize20"] = (double)Application.Current.Resources["FontSize20"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize24"] = (double)Application.Current.Resources["FontSize24"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize36"] = (double)Application.Current.Resources["FontSize36"] * ( 1 - _changeStep);
            Application.Current.Resources["FontSize48"] = (double)Application.Current.Resources["FontSize48"] * ( 1 - _changeStep);
            FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";
            SetFontSizeMode((double)Application.Current.Resources["FontMultiplier"]);
        }

        /// <summary>
        /// Проверка на возможность дальнейшего увеличения/уменьшения шрифта
        /// </summary>
        /// <param name="multiplier"></param>
        private void SetFontSizeMode(double multiplier)
        {
            if(multiplier > 50)
                FontSizeMinusButton.IsEnabled = true;
            else
                FontSizeMinusButton.IsEnabled = false;
            if (multiplier < 150)
                FontSizePlusButton.IsEnabled = true;
            else
                FontSizePlusButton.IsEnabled = false;
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Aura.Controls.AuraStringDialog SchemeNameDialog = new AuraStringDialog("Введите название схемы:");
            SchemeNameDialog.ShowDialog();

            if (SchemeNameDialog.Result == null)
            {
                return;
            }

            string schemeName = SchemeNameDialog.Result;
            SaveXML(ThemeHelper.GetXMLThemeSchemes(ThemeHelper.GetCurrentThemeScheme(), schemeName), schemeName);
            ReInitialize();
        }


        /// <summary>
        /// Обработчик кнопки "Импорт"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog themePath = new OpenFileDialog();
            ThemeScheme loadedScheme = new ThemeScheme();
            if (themePath.ShowDialog() == true)
                loadedScheme = ThemeHelper.GetThemeSchemesFromXML(themePath.FileName)[0];
            else
                return;
            loadedScheme.Apply();
            Theme.SelectedIndex = -1;
            Font.SelectedIndex = -1;
            Scheme.SelectedIndex = -1;
            UpdateColorPickers();
            AuraMessage messager = new AuraMessage();
            root.Children.Add(messager);
            messager.ShowMessage(
                "Тема была успешно применена. Чтобы сохранить ее в списке выберите <Сохранить>",
                "Применение темы",
                "Отлично",
                AuraMessage.MessageType.Information
                );
            Aura.Controls.AuraMessage message = new AuraMessage();
            FontSizeForUser.Text = ((double)Application.Current.Resources["FontMultiplier"]).ToString() + "%";

        }

        /// <summary>
        /// Обработчик кнопки "Экспорт"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Aura.Controls.AuraStringDialog SchemeNameDialog = new AuraStringDialog("Введите название схемы:");
            SchemeNameDialog.ShowDialog();

            if (SchemeNameDialog.Result == null)
            {
                return;
            }

            string schemeName = SchemeNameDialog.Result;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = ".aurascheme";
            saveDialog.ValidateNames = true;
            saveDialog.AddExtension = true;
            saveDialog.CheckPathExists = true;
            saveDialog.FileName = schemeName;
            if(saveDialog.ShowDialog() == true)
            {
                ExportXML(ThemeHelper.GetXMLThemeSchemes(ThemeHelper.GetCurrentThemeScheme(), schemeName), saveDialog.FileName);
            }
            
        }

        /// <summary>
        /// Обработчик кнопки "Открыть папку"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(folderPath);
        }

        /// <summary>
        /// Сохранение темы
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="filename"></param>
        private void SaveXML(XDocument theme, string filename)
        {
            ThemeHelper.CheckThemeDirectory();
            string endPoint = folderPath + @"\" + filename + @".aurascheme";
            if (File.Exists(endPoint))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(
                    "Схема с таким именем уже существует, перезаписать?",
                    "Предупреждение", System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning,
                    System.Windows.MessageBoxResult.Yes);
                switch (result)
                {
                    case System.Windows.MessageBoxResult.Yes:
                        SaveXMLonDisk(endPoint, theme);
                        break;
                    case System.Windows.MessageBoxResult.No:
                        return;
                }
            }
            else
            {
                SaveXMLonDisk(endPoint, theme);
            }
        }

        /// <summary>
        /// Выгрузка темы
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="path"></param>
        private void ExportXML(XDocument theme, string path)
        {
            if (File.Exists(path))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(
                    "Схема с таким именем уже существует, перезаписать?",
                    "Предупреждение", System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning,
                    System.Windows.MessageBoxResult.Yes);
                switch (result)
                {
                    case System.Windows.MessageBoxResult.Yes:
                        SaveXMLonDisk(path, theme);
                        break;
                    case System.Windows.MessageBoxResult.No:
                        return;
                }
            }
            else
            {
                SaveXMLonDisk(path, theme);
            }
        }

        /// <summary>
        /// Сохранение темы на диск
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xmlDocument"></param>
        private static void SaveXMLonDisk(string filePath, XDocument xmlDocument)
        {
            FileStream stream = new FileStream(filePath, FileMode.Create);
            xmlDocument.Save(stream);
            stream.Close();
        }


        /// <summary>
        /// Получение листа тем сохраненным пользователем в базовой директории
        /// </summary>
        /// <returns></returns>
        private List<ThemeScheme> GetUserThemeSchemes()
        {
            List<ThemeScheme> result = new List<ThemeScheme>();
            Helpers.ThemeHelper.CheckThemeDirectory();
            string[] fileList = Directory.GetFiles(folderPath);
            foreach (string path in fileList)
            {
                if(path.EndsWith(".aurascheme"))
                {
                    result.Add(ThemeHelper.GetThemeSchemesFromXML(path)[0]);
                }
            }
            return result;
        }

        /// <summary>
        /// Применение выбранного стиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Styles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Styles.SelectedIndex != -1)
            {
                    ((StyleScheme)Styles.SelectedItem).Apply();
            }
        }
    }
}
