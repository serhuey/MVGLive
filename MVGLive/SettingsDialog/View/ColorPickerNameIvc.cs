using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MVGLive
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerNameIvc : IValueConverter
    {
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
            if (parameter == null) return "";

            string key = parameter.ToString();
            if(SettingsViewModel.ColorPickerNames.ContainsKey(key))
            {
                return SettingsViewModel.ColorPickerNames[key];
            }
            return "";
        }

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
