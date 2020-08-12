// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Converst delay value to the delay image
    /// </summary>
    public class DelayImageMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] == null) return null;
            string delay = values[0].ToString();

            Match m = Regex.Match(delay, @"^-?\d*");
            if (m.Success && int.TryParse(m.Value, out int iMinutes) && iMinutes > Common.DelayThreshold1)
            {
                int iconIndex = (iMinutes - 5) / 5 + 1;

                if (iconIndex < 0)
                {
                    iconIndex = 0;
                }

                if (iconIndex > Common.DelayIconKey.Length - 1)
                {
                    iconIndex = Common.DelayIconKey.Length - 1;
                }

                return Common.icons[Common.DelayIconKey[iconIndex].ToUpperInvariant()];
            }
            else return Common.icons[Common.Delay0IconKey.ToUpperInvariant()];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}