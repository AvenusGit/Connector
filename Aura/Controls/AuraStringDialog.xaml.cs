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
    /// Контрол выводит на экран небольшое окно с просьбой заполнить текстовое поле.
    /// Необходимо например, когда пользователь должен ввести имя элемента.
    /// </summary>
    public partial class AuraStringDialog : Window
    {
        private string _result;
        /// <summary>
        /// Пустой конструктор запроса строки у пользователя.
        /// </summary>
        public AuraStringDialog()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Запрос строки у пользователя. Результат пишется в поле Result.
        /// Если там null - пользователь нажал отмену.
        /// </summary>
        /// <param name="request">Текст запроса</param>
        public AuraStringDialog(string request)
        {
            InitializeComponent();
            RequestDescription = request;
        }

        /// <summary>
        /// Текст запроса строки у пользователя. Например: "Назовите вашу собаку".
        /// </summary>
        public string RequestDescription
        {
            get
            {
                return Request.Text;
            }
            set
            {
                Request.Text = value;
            }
        }

        /// <summary>
        /// Результат диалога с пользователем - полученая строка.
        /// </summary>
        public string Result
        {
            get
            {
                return _result;
            }
        }

        private void Ok_click(object sender, EventArgs e)
        {
            _result = UserText.Text;
            this.Close();
        }
        private void Cancel_click(object sender, EventArgs e)
        {
            _result = null;
            this.Close();
        }

        public void ShowDialog(string request)
        {
            RequestDescription = request;
            this.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
