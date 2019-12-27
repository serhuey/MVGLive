using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MVGTimeTable
{
    /// <summary>
    /// Convert minutes string to picture "Jetzt" if value of minutes is less than 1 and greater than -3
    /// </summary>
    public class ImageMinutesValueMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1) return null;
            string minutes = values[0].ToString();
            int iMinutes;

            Match m = Regex.Match(minutes, @"^-?\d*");
            if (m.Success && int.TryParse(m.Value, out iMinutes) && iMinutes <= 0 && iMinutes > -3)
            {
                return new BitmapImage(new Uri(Common.ImagePath + "Jetzt.png"));
            }
            else return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
}
