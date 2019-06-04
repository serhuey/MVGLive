using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MVGTimeTable
{
    class MarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness th;

            if (values.Length == 0) return null;
            if (parameter == null) return null;
            if (!double.TryParse((values[0].ToString()), out double fontSize)) return null;

            switch(parameter.ToString())
            {
                case "stackPanelProductLabel":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 1.5, 0); break;
                case "stackPanelMainDestination":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 1.5, 0);  break;
                case "stackPanelMinutes":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 2.5, 0); break;
                default: return null;
            }
            return th;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
