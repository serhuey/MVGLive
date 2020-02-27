// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Return string with additional destination, beginning with "via" word
    /// </summary>
    public class AdditionalDestinationMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] == null) return null;

            return ParseDestination.GetAdditionalDestination(values[0].ToString(), remove_U_S: true);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
