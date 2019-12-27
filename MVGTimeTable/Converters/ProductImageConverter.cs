using System;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class ProductImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string product = value.ToString().ToUpperInvariant();
            switch (product)
            {
                case "UBAHN":
                    return "../Icons/UBahn.png";
                case "SBAHN":
                    return "../Icons/SBahn.png";
                case "BUS":
                case "REGIONAL-BUS":
                    return "../Icons/Bus.png";
                case "TRAM":
                    return "../Icons/Tram.png";
                default:
                    return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
