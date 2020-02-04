// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVGTimeTable
{
    public class MainDestinationForegroundMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return null;

            SolidColorBrush brush;
            string foregroundColor;
            string product = values[0].ToString();

            if (product == Common.WarnMessageType[MessageType.Warning])
            {
                foregroundColor = Common.WarningForegroundColor;
            }
            else if (product == Common.WarnMessageType[MessageType.NoConnection])
            {
                foregroundColor = Common.NoConnectionForegroundColor;
            }
            else
            {
                foregroundColor = Common.DefaultForegroundColor;
            }

            brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
