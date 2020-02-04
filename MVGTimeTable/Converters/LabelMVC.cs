// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Return string with line number for lines without graphic logo like most of U- and S-Bahn
    /// </summary>
    public class LabelMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return null;

            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();

            if (product.Contains(Common.WarnMessageType[MessageType.NoConnection]) || product.Contains(Common.WarnMessageType[MessageType.Warning])) return null;
            if (product.Contains(Common.TramMarker) || product.Contains(Common.BusMarker)) return label;
            if (product.Contains(Common.UBahnMarker))
                return string.IsNullOrEmpty(Array.Find<string>(Common.UBahnIconKey, str => str.Contains(label))) ? label : null;
            if (product.Contains(Common.SBahnMarker))
                return string.IsNullOrEmpty(Array.Find<string>(Common.SBahnIconKey, str => str.Contains(label))) ? label : null;

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
