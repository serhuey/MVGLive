using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class TextMinutesValueMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1) return null;
            string minutes = values[0].ToString();
            int iMinutes;

            Match m = Regex.Match(minutes, @"^-?\d*");
            if(m.Success && int.TryParse(m.Value, out iMinutes) && iMinutes <= 0)
            {
                if (iMinutes > -3)
                {
                    return null;
                }
                else return "???";
            }
            else return minutes;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
}
