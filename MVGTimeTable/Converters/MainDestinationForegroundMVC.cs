using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVGTimeTable
{
    public class MainDestinationForegroundMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return null;

            SolidColorBrush brush;
            string foregroundColor;
            string product = values[0].ToString();
            switch (product)
            {
                case ("WARNING"):
                    foregroundColor = "#FFFFEB85"; break;
                case ("NO_CONNECTION"):
                    foregroundColor = "#FFFF4E48"; break;
                default:
                    foregroundColor = "#FFE8E8E8"; break;
            }
            brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
