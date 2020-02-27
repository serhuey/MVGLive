// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Convert minutes string to picture "Jetzt" if value of minutes is less than 1 and greater than -3
    /// </summary>
    public class ImageMinutesValueMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] == null) return null;
            string minutes = values[0].ToString();

            Match m = Regex.Match(minutes, @"^-?\d*");
            if (m.Success && int.TryParse(m.Value, out int iMinutes) && iMinutes <= 0 && iMinutes > Common.UndefinedSignThreshold)
            {
                return Common.icons[Common.NowIconKey.ToUpperInvariant()];
            }
            else return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
}
