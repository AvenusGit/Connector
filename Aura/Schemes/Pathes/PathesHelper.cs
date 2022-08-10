using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.Pathes
{
    public static class Pathes
    {

        public static Geometry GetPath(string geometry)
        {
            return Geometry.Parse(geometry);
        }

        public static string ErrorPath = "M13,13H11V7H13M13,17H11V15H13M12," +
           " 2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10, 10 0 0,0 22,12A10,10 0 0,0 12,2Z";

        public static string WarningPath = "M13 14H11V9H13M13 18H11V16H13M1 21H23L12 2L1 21Z";

        public static string InfoPath = "M13.5,4A1.5,1.5 0 0,0 12,5.5A1.5,1.5 0 0,0 13.5,7A1.5," +
            "1.5 0 0,0 15,5.5A1.5,1.5 0 0,0 13.5,4M13.14,8.77C11.95,8.87 8.7,11.46 8.7,11.46C8.5," +
            "11.61 8.56,11.6 8.72,11.88C8.88,12.15 8.86,12.17 9.05,12.04C9.25,11.91 9.58,11.7 10.13," +
            "11.36C12.25,10 10.47,13.14 9.56,18.43C9.2,21.05 11.56,19.7 12.17,19.3C12.77,18.91 14.38," +
            "17.8 14.54,17.69C14.76,17.54 14.6,17.42 14.43,17.17C14.31,17 14.19,17.12 14.19,17.12C13.54," +
            "17.55 12.35,18.45 12.19,17.88C12,17.31 13.22,13.4 13.89,10.71C14,10.07 14.3,8.67 13.14,8.77Z";

        public static string JokePath = "M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10" +
            " 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20M10,9.5C10,10.3" +
            " 9.3,11 8.5,11C7.7,11 7,10.3 7,9.5C7,8.7 7.7,8 8.5,8C9.3,8 10,8.7 10,9.5M12,17.23C10.25,17.23" +
            " 8.71,16.5 7.81,15.42L9.23,14C9.68,14.72 10.75,15.23 12,15.23C13.25,15.23 14.32,14.72 14.77," +
            "14L16.19,15.42C15.29,16.5 13.75,17.23 12,17.23M17,10H13V9H17V10Z";
    }
}
