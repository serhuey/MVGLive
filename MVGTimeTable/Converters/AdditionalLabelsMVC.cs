using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class AdditionalLabelsMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return null;

            return ParseDestination.Get_U_S_Image(ParseDestination.GetAdditionalDestination(values[0].ToString()));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

