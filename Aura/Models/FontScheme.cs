using System.Windows;
using System.Windows.Media;

namespace Aura.Models
{
    public class FontScheme : Scheme
    {
        private FontFamily _font;
        private double _multiplier;

        /// <summary>
        /// Выбранный шрифт
        /// </summary>
        public FontFamily Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        /// <summary>
        /// Множитель размера шрифта (от 0,5 до 1,5)
        /// </summary>
        public double Multiplier
        {
            get
            {
                return _multiplier;
            }
            set
            {
                _multiplier = value;
            }
        }

        /// <summary>
        /// Применить схему прямо сейчас
        /// </summary>
        public override void Apply()
        {
            Application.Current.Resources["Font"] = Font;
            ChangeMultiplier(Multiplier);
        }

        /// <summary>
        /// Изменить множитель масштаба шрифта прямо сейчас
        /// </summary>
        /// <param name="newValue">Новое значение множителя</param>
        private void ChangeMultiplier(double newValue)
        {
            double multiplier = newValue / 100;
            Application.Current.Resources["FontSize8"] = 8 * multiplier;
            Application.Current.Resources["FontSize12"] = 12 * multiplier;
            Application.Current.Resources["FontSize14"] = 14 * multiplier;
            Application.Current.Resources["FontSize16"] = 16 * multiplier;
            Application.Current.Resources["FontSize18"] = 18 * multiplier;
            Application.Current.Resources["FontSize20"] = 20* multiplier;
            Application.Current.Resources["FontSize24"] = 20 * multiplier;
            Application.Current.Resources["FontSize36"] = 20 * multiplier;
            Application.Current.Resources["FontSize48"] = 48 * multiplier;
            Application.Current.Resources["FontMultiplier"] = newValue;
        }

        /// <summary>
        /// Привести схему к строке
        /// </summary>
        /// <returns>"Название шрифта"("Имя схемы")</returns>
        public override string ToString()
        {
            return Font.Source + "(" +  Name + ")";
        }

    }
}
