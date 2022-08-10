﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Connector.ViewModels;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для ServerListControl.xaml
    /// </summary>
    public partial class ServerListControl : UserControl
    {
        public ServerListControl(ServerListControlViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}