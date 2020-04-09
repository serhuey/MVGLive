// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGLive
{
    /// ************************************************************************************************
    /// <summary>
    /// 
    /// </summary>
    public class MainWindowTypeIvc : IValueConverter
    {
        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MainWindowType mainWindowType)) return false;
            bool parseSuccess = Enum.TryParse((string)parameter, true, out MainWindowType paramMainWindowType);
            return parseSuccess && mainWindowType == paramMainWindowType;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool b && b)) return null;
            Enum.TryParse((string)parameter, true, out MainWindowType mainWindowType);
            return (object)mainWindowType;
        }
    }
}
