using System;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aura.Models
{
    public class ThemeScheme : Scheme
    {
        private StyleScheme _style;
        private ColorScheme _colorScheme;
        private FontScheme _fontScheme;

        public StyleScheme StyleScheme
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        public ColorScheme ColorScheme
        {
            get
            {
                return _colorScheme;
            }
            set
            {
                _colorScheme = value;
            }
        }

        public FontScheme FontScheme
        {
            get
            {
                return _fontScheme;
            }
            set
            {
                _fontScheme = value;
            }
        }


        public override void Apply()
        {
            ColorScheme.Apply();
            FontScheme.Apply();
            StyleScheme.Apply();
        }
    }
}
