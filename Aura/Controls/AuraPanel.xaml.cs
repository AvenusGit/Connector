using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace Aura.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraPanel.xaml
    /// </summary>
    public partial class AuraPanel : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty ShadowColorProperty;
        public static DependencyProperty ShadowDirectionProperty;
        public static DependencyProperty ShadowRadiusProperty;

        private Color _shadowColor = Colors.DimGray;
        private int _shadowDirection = 300;
        private int _shadowRadius = 30;


        public AuraPanel()
        {
            //RegisterDependencyProperty();
            ShadowColor = Colors.DimGray;
            ShadowDirection = 300;
            ShadowRadius = 30;
            Background = new SolidColorBrush(Colors.WhiteSmoke);
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Указывает цвет тени. По умолчанию темно серый.
        /// </summary>
        public Color ShadowColor
        {
            get
            {
                return _shadowColor;
            }
            set
            {
                _shadowColor = value;
                OnPropertyChanged("ShadowColor");
            }
        }

        /// <summary>
        /// Направление тени
        /// </summary>
        public int ShadowDirection
        {
            get
            {
                return _shadowDirection;
            }
            set
            {
                _shadowDirection = value;
                OnPropertyChanged("ShadowDirection");
            }
        }

        /// <summary>
        /// Радиус тени
        /// </summary>
        public int ShadowRadius
        {
            get
            {
                return _shadowRadius;
            }
            set
            {
                _shadowRadius = value;
                OnPropertyChanged("ShadowRadius");
            }
        }

        public static readonly DependencyProperty InnerContentProperty =
                DependencyProperty.Register("InnerContent", typeof(object), typeof(AuraPanel));

        public object InnerContent
        {
            get { return (object)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        private void OnShadowColorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AuraPanel panel = (AuraPanel)sender;
            if (e.Property == ShadowColorProperty)
            {
                panel.ShadowColor = (Color)e.NewValue;
            }
        }

        private void OnShadowDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AuraPanel panel = (AuraPanel)sender;
            if (e.Property == ShadowDirectionProperty)
            {
                panel.ShadowDirection = (int)e.NewValue;
            }
        }

        private void OnShadowRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AuraPanel panel = (AuraPanel)sender;
            if (e.Property == ShadowRadiusProperty)
            {
                panel.ShadowRadius = (int)e.NewValue;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
namespace System.Runtime.CompilerServices
{
    sealed class CallerMemberNameAttribute : Attribute { }
}
