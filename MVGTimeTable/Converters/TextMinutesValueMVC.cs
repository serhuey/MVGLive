// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class TextMinutesValueMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 1) return null;
            string minutes = values[0].ToString();
            string addition = "  ";
            if (minutes.Contains(Common.HoursSign))
            {
                return minutes;
            }

            // 
            Match m = Regex.Match(minutes, @"^-?\d*");
            bool parseResult = int.TryParse(m.Value, out int iMinutes);

            if (m.Success && parseResult)
            {
                // Something is wrong in received data - the vehicle had to departure more than three minutes ago.
                if (iMinutes <= Common.UndefinedSignThreshold)
                {
                    return addition + Common.UndefinedTimeSign;
                }

                // "Jetzt" picture is showing, no need to display string with Min.
                if (iMinutes <= 0 && iMinutes > Common.UndefinedSignThreshold)
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
