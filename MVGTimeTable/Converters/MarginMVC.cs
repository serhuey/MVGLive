// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MVGTimeTable
{
    class MarginMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness th = new Thickness(0);

            if (values == null ||
                values.Length == 0 ||
                parameter == null ||
                values[0] == null || 
                !double.TryParse(values[0].ToString(), out double fontSize))
                return th;

            switch (parameter.ToString())
            {
                case "stackPanelProductLabel":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 1.5, 0); break;
                case "stackPanelMainDestination":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 1.5, 0); break;
                case "stackPanelMinutes":
                    th = new Thickness(fontSize / 2.5, 0, fontSize / 2.5, 0); break;
                case "labelDelay":
                    th = new Thickness(0, fontSize * 0.17, 0, 0); break;
                default: return th;
            }
            return th;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
