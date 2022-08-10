using System.Windows;
using System.Windows.Media;

namespace Aura.Models
{
    /// <summary>
    /// Цветовая схема
    /// </summary>
    public class ColorScheme : Scheme
    {
        private Color _foneColor;
        private Color _accentColor;
        private Color _subAccentColor;
        private Color _panelColor;
        private Color _pathColor;
        private Color _borderColor;
        private Color _textColor;
        private Color _selectionColor;
        private Color _errorColor;
        private Color _disableColor;

        private SolidColorBrush _foneBrush;
        private SolidColorBrush _accentBrush;
        private SolidColorBrush _subAccentBrush;
        private SolidColorBrush _panelBrush;
        private SolidColorBrush _pathBrush;
        private SolidColorBrush _borderBrush;
        private SolidColorBrush _textBrush;
        private SolidColorBrush _selectionBrush;
        private SolidColorBrush _errorBrush;
        private SolidColorBrush _disableBrush;

        #region Colors
        /// <summary>
        /// Фоновый цвет
        /// </summary>
        public Color FoneColor
        { 
            get
            {
                return _foneColor;
            }
            set
            {
                _foneColor = value;
                FoneBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Акцентный цвет
        /// </summary>
        public Color AccentColor
        {
            get
            {
                return _accentColor;
            }
            set
            {
                _accentColor = value;
                AccentBrush = new SolidColorBrush(value);
            }
        }
        /// <summary>
        /// Субакцентный цвет
        /// </summary>
        public Color SubAccentColor
        {
            get
            {
                return _subAccentColor;
            }
            set
            {
                _subAccentColor = value;
                SubAccentBrush = new SolidColorBrush(value);
            }
        }
        /// <summary>
        /// Цвет панелей
        /// </summary>
        public Color PanelColor
        {
            get
            {
                return _panelColor;
            }
            set
            {
                _panelColor = value;
                PanelBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет иконок
        /// </summary>
        public Color PathColor
        {
            get
            {
                return _pathColor;
            }
            set
            {
                _pathColor = value;
                PathBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет границ
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                BorderBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет текста
        /// </summary>
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                TextBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет выделения
        /// </summary>
        public Color SelectionColor
        {
            get
            {
                return _selectionColor;
            }
            set
            {
                _selectionColor = value;
                SelectionBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет ошибки
        /// </summary>
        public Color ErrorColor
        {
            get
            {
                return _errorColor;
            }
            set
            {
                _errorColor = value;
                ErrorBrush = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Цвет выключенного элемента
        /// </summary>
        public Color DisableColor
        {
            get
            {
                return _disableColor;
            }
            set
            {
                _disableColor = value;
                DisableBrush = new SolidColorBrush(value);
            }
        }
        #endregion
        #region Brushes
        /// <summary>
        /// Кисть с фоновым цветом
        /// </summary>
        public SolidColorBrush FoneBrush
        {
            get
            {
                return _foneBrush;
            }
            set 
            {
                _foneBrush = value;
                _foneColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с акцентным цветом
        /// </summary>
        public SolidColorBrush AccentBrush
        {
            get
            {
                return _accentBrush;
            }
            set
            {
                _accentBrush = value;
                _accentColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с субакцентным цветом
        /// </summary>
        public SolidColorBrush SubAccentBrush
        {
            get
            {
                return _subAccentBrush;
            }
            set
            {
                _subAccentBrush = value;
                _subAccentColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом панели
        /// </summary>
        public SolidColorBrush PanelBrush
        {
            get
            {
                return _panelBrush;
            }
            set
            {
                _panelBrush = value;
                _panelColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом иконок
        /// </summary>
        public SolidColorBrush PathBrush
        {
            get
            {
                return _pathBrush;
            }
            set
            {
                _pathBrush = value;
                _pathColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом границ
        /// </summary>
        public SolidColorBrush BorderBrush
        {
            get
            {
                return _borderBrush;
            }
            set
            {
                _borderBrush = value;
                _borderColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом текста
        /// </summary>
        public SolidColorBrush TextBrush
        {
            get
            {
                return _textBrush;
            }
            set
            {
                _textBrush = value;
                _textColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом выделения
        /// </summary>
        public SolidColorBrush SelectionBrush
        {
            get
            {
                return _selectionBrush;
            }
            set
            {
                _selectionBrush = value;
                _selectionColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом ошибки
        /// </summary>
        public SolidColorBrush ErrorBrush
        {
            get
            {
                return _errorBrush;
            }
            set
            {
                _errorBrush = value;
                _errorColor = value.Color;
            }
        }
        /// <summary>
        /// Кисть с цветом выключенного элемента
        /// </summary>
        public SolidColorBrush DisableBrush
        {
            get
            {
                return _disableBrush;
            }
            set
            {
                _disableBrush = value;
                _disableColor = value.Color;
            }
        }
        #endregion

        /// <summary>
        /// Применить схемы прямо сейчас
        /// </summary>
        public override void Apply()
        {
            Application.Current.Resources["CFone"] = FoneColor;
            Application.Current.Resources["FoneBrush"] = FoneBrush;

            Application.Current.Resources["CAccent"] = AccentColor;
            Application.Current.Resources["AccentBrush"] = AccentBrush;

            Application.Current.Resources["CSubAccent"] = SubAccentColor;
            Application.Current.Resources["SubAccentBrush"] = SubAccentBrush;

            Application.Current.Resources["CBorder"] = BorderColor;
            Application.Current.Resources["BorderBrush"] = BorderBrush;

            Application.Current.Resources["CPanel"] = PanelColor;
            Application.Current.Resources["PanelBrush"] = PanelBrush;

            Application.Current.Resources["CPath"] = PathColor;
            Application.Current.Resources["PathBrush"] = PathBrush;

            Application.Current.Resources["CText"] = TextColor;
            Application.Current.Resources["TextBrush"] = TextBrush;

            Application.Current.Resources["CSelect"] = SelectionColor;
            Application.Current.Resources["SelectBrush"] = SelectionBrush;

            Application.Current.Resources["CError"] = ErrorColor;
            Application.Current.Resources["ErrorBrush"] = ErrorBrush;

            Application.Current.Resources["CDisable"] = DisableColor;
            Application.Current.Resources["DisableBrush"] = DisableBrush;
        }

        /// <summary>
        /// Привести схему к строке (получить ее имя)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
