using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    /// <summary>
    /// Версия приложения
    /// </summary>
    public class ApplicationVersion
    {
        /// <summary>
        /// Конструктор класса версии приложения
        /// </summary>
        /// <param name="series">Серия приложения (только заглавные латинские)></param>
        /// <param name="number">Номер версии (число)</param>
        /// <param name="subSeries">Подсерия приложения (только заглавные латинские)</param>
        public ApplicationVersion(string series, int number, string? subSeries = null)
        {
            if (Regex.IsMatch(series.ToUpper() + number.ToString() + subSeries?.ToUpper(), @"^[A-Z]+[0-9]+[A-Z]*$"))
            {
                Series = series.ToUpper();
                Number = number;
                SubSeries = subSeries?.ToUpper();
            }
            else throw new Exception("Не удалось создать экземпляр класса версии приложения. Неверные аргументы");
        }
        public string Series { get; set; }
        public int Number { get; set; }
        public string? SubSeries { get; set; }
        public static ApplicationVersion Parse(string versionString)
        {
            string series;
            Match seriesMatch = Regex.Match(versionString.ToUpper(), @"^[A-Z]+");
            if (seriesMatch.Success)
                series = seriesMatch.Groups[0].Value;
            else
                throw new Exception("ApplicationVersion Parsing: Series not found");

            int number;
            Match numberMatch = Regex.Match(versionString.ToUpper(), @"[0-9]+");
            if (numberMatch.Success)
                number = Int32.Parse(seriesMatch.Groups[0].Value);
            else
                throw new Exception("ApplicationVersion Parsing: Number not found");

            string? subSeries;
            Match subSeriesMatch = Regex.Match(versionString.ToUpper(), @"[A-Z]*$");
            if (subSeriesMatch.Success)
            {
                subSeries = seriesMatch.Groups[0].Value;
            }
            else subSeries = null;

            return new ApplicationVersion(series, number, subSeries);
        }
        public static bool TryParse(string versionString, out ApplicationVersion? result)
        {
            try
            {
                Regex regex = new Regex(@"^[A-Z]+[0-9]+[A-Z]*$");
                if (regex.IsMatch(versionString.ToUpper()))
                {
                    result = ApplicationVersion.Parse(versionString);
                    return true;
                }
                result = null;
                return false;
            }
            catch
            {
                result = null;
                return false;
            }            
        }
        public override string ToString()
        {
            return Series + Number.ToString() + SubSeries;
        }
    }
}
