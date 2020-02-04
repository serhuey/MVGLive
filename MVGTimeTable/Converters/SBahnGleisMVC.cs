// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class SBahnGleisMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || 
                values.Length < 3 || 
                values[0] == null || 
                values[1] == null || 
                values[2] == null)
                return null;

            string product = values[0].ToString();
            string gleis = values[1].ToString();
            string station = values[2].ToString().ToUpperInvariant();

            if ((Common.MultiplatformSbahnStations.Contains(station) &&
                product.Contains(Common.SBahnMarker)) ||
                (Common.MultiplatformTramStations.Contains(station) &&
                product.Contains(Common.TramMarker)) &&
                !string.IsNullOrEmpty(gleis))
            {
                string gleisSymbol = null;

                if (int.TryParse(Regex.Match(gleis, @"\d+").Value, out int index) && index > 0 && index < Common.PlatformSign.Length)
                {
                    gleisSymbol = Common.PlatformSign[index];
                }
                return gleisSymbol; 
            }
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
