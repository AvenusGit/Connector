using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aura.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraLoading.xaml
    /// </summary>
    public partial class AuraLoading : UserControl
    {
        public AuraLoading()
        {
            InitializeComponent();
        }

        string _loadingText = "Подождите, идет загрузка...";

        public string LoadingText
        {
            get 
            {
                return _loadingText;
            }
            set
            {
                _loadingText = value;
            }
        }

        public void ShowLoadingScreen()
        {
            Visibility = Visibility.Visible;
        }
        public void HideLoadingScreen()
        {
            Visibility = Visibility.Collapsed;
        }

        private void CloseLoading_Click(object sender, RoutedEventArgs e)
        {
            HideLoadingScreen();
        }
    }
}
