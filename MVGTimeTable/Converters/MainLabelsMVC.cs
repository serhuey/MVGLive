using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class MainLabelsMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 || values[0] == null || values[1] == null || values[2] == null) return null;

            return ParseDestination.GetDestinationImage(ParseDestination.GetMainDestination(values[0].ToString()), values[1].ToString(), values[2].ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
