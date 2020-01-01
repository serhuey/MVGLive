using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Return destination string without additional destination if it exists
    /// </summary>
    public class MainDestinationMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return null;

            return ParseDestination.GetMainDestination(values[0].ToString(), true) + " ";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
