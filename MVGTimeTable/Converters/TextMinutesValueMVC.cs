using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class TextMinutesValueMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 1) return null;
            string minutes = values[0].ToString();
            string addition = "  ";
            int iMinutes;

            if (minutes.Contains("Std."))
            {
                return minutes;
            }

            // 
            Match m = Regex.Match(minutes, @"^-?\d*");
            bool parseResult = int.TryParse(m.Value, out iMinutes);

            if (m.Success && parseResult)
            {
                // Something is wrong in received data - the vehicle had to departure more than three minutes ago.
                if (iMinutes <= -3)
                {
                    return addition + "???";
                }

                // "Jetzt" picture is showing, no need to display string with Min.
                if (iMinutes <= 0 && iMinutes > -3)
                {
                    return addition;
                }

                // Add one space instead of "0" before minutes - not very good idea, but with the "right to left flow" in flow panel the results are
                // very strange - the text changes order and looks like ".Min 2". 
                if (iMinutes < 10)
                {
                    return addition + minutes;
                }

            }
            return minutes;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
