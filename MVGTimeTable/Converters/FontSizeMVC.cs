using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class FontSizeMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length == 0 ||
                parameter == null ||
                values[0] == null ||
                !double.TryParse(values[0].ToString(), out double fontSize))
                return 0.01;            // 0 is not acceptable for FontSize property


            switch (parameter.ToString())
            {
                case "labelDelay":
                    fontSize = fontSize * 0.55; break;
            }
            return fontSize;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}


