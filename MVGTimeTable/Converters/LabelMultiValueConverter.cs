using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Return string with line number for lines without graphic logo like most of U- and S-Bahn
    /// </summary>
    public class LabelMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return null;

            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();

            if (product.Contains("TRAM") || product.Contains("BUS")) return label;
            if (product == "UBAHN")
            {
                switch (label)
                {
                    case "U":
                    case "U1":
                    case "U2":
                    case "U3":
                    case "U4":
                    case "U5":
                    case "U6":
                    case "U7":
                    case "U8":
                        return null;
                    default:
                        return label;
                }
            }
            if (product == "SBAHN")
            {
                switch (label)
                {
                    case "S1":
                    case "S2":
                    case "S3":
                    case "S4":
                    case "S6":
                    case "S7":
                    case "S8":
                    case "S20":
                        return null;
                    default:
                        return label;
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
