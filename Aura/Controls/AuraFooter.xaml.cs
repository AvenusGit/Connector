using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
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
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class AuraFooter : UserControl
    {
        public AuraFooter()
        {
            InitializeComponent();
        }

        private void HideLists()
        {
            lists.IsEnabled = false;
            lists.Visibility = System.Windows.Visibility.Collapsed;
            HiderPath.Data = Geometry.Parse("M8.59,16.58L13.17,12L8.59,7.41L10,6L16,12L10,18L8.59,16.58Z");
        }
        private void ShowLists()
        {
            lists.IsEnabled = true;
            lists.Visibility = System.Windows.Visibility.Visible;
            HiderPath.Data = Geometry.Parse("M15.41,16.58L10.83,12L15.41,7.41L14,6L8,12L14,18L15.41,16.58Z");
        }

        private void Hider_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (lists.IsEnabled)
                HideLists();
            else
                ShowLists();
        }
    }
}
