// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Convert delay value to the delay image
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
                string iconKey;
                iconKey = iMinutes < Common.DelayThreshold2 ? Common.Delay1IconKey :
                          (iMinutes < Common.DelayThreshold3 ? Common.Delay2IconKey : Common.Delay3IconKey);
                return Common.icons[iconKey.ToUpperInvariant()];
            }
            else return Common.icons[Common.Delay0IconKey.ToUpperInvariant()];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}