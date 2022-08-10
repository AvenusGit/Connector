using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using AuraS.Models;
using AuraS.Interfaces;

namespace AuraS.Controls.ControlsViewModels
{
    public class AuraSettingControlViewModel : INotifyPropertyChanged
    {
        public AuraSettingControlViewModel(IVisualScheme scheme)
        {
            ColorScheme = scheme.ColorScheme;
            FontScheme = scheme.FontScheme;
            SelectedFont = FontScheme.Font;
        }
        private FontFamily _selectedFont;
        public IColorScheme ColorScheme { get; set; }
        public IFontScheme FontScheme { get; set; }
        public int SelectedFontSize 
        {
            get
            {
                return FontScheme.FontMultiplierPercent.HasValue ? (int)FontScheme.FontMultiplierPercent.Value : 0;
            }
            set
            {
                FontScheme.FontMultiplierPercent = value;
                FontScheme.Apply();
                OnPropertyChanged("SelectedFontSize");
                OnPropertyChanged("SelectedFontSizeString");
            }
        }
        public string SelectedFontSizeString 
        {
            get
            {
                return SelectedFontSize.ToString() + "%";
            }
        }
        public FontFamily SelectedFont
        {
            get { return _selectedFont; }
            set
            {
                _selectedFont = value;
                FontScheme.Font = _selectedFont;
                FontScheme.Apply();
            }
        }
        public IEnumerable<FontFamily> CyrrylicFonts
        {
            get
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
