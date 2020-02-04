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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc")]
    public class AdditionalDestinationMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return null;
            ParseDestination.GetBothDestinations(values[0].ToString(), out string mainDestination, out string additionalDestination, false, true);
            if (!string.IsNullOrEmpty(additionalDestination) && !string.IsNullOrEmpty(mainDestination) && ParseDestination.IsStringContains_U_S(mainDestination))
            {
                //Double space after label in main destination (if it presents)
                additionalDestination = "  " + additionalDestination;
            }
            //Space before label in additional destination
            return additionalDestination + " ";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
