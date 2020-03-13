// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MVGTimeTable
{
    internal class HorizontalAlignmentMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object horizontalAlignment = HorizontalAlignment.Left;

            if (values != null && values.Length != 0 && values[0] != null && parameter != null)
            {
                string product = values[0].ToString();

                bool isHeader = product.ToUpperInvariant() == Common.HeaderProduct.ToUpperInvariant();

                switch (parameter.ToString())
                {
                    case "labelProduct":
                        horizontalAlignment = isHeader ? HorizontalAlignment.Left : HorizontalAlignment.Left;
                        break;
                    case "labelMainDestination":
                        horizontalAlignment = isHeader ? HorizontalAlignment.Left : HorizontalAlignment.Left;
                        break;
                    case "labelAdditionalDestination":
                        horizontalAlignment = isHeader ? HorizontalAlignment.Left : HorizontalAlignment.Left;
                        break;
                    case "labelGleis":
                        horizontalAlignment = isHeader ? HorizontalAlignment.Left : HorizontalAlignment.Left;
                        break;
                    case "labelTimeToDeparture":
                        horizontalAlignment = isHeader ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                        break;
                    case "labelDepartureTime":
                        horizontalAlignment = isHeader ? TextAlignment.Center : TextAlignment.Left; 
                        break;
                }
            }
            return horizontalAlignment;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}