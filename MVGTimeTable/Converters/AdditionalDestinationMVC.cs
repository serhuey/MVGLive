using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Return string with additional destination, beginning with "via" word
    /// </summary>
    public class AdditionalDestinationMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0) return null;

            string destination = values[0].ToString();
            string[] splittedDestination = destination.Split(' ');
            StringBuilder outputString = new StringBuilder();
            bool startBuilding = false;
            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA" || startBuilding)
                {
                    startBuilding = true;
                    outputString.Append(str + " ");
                }
            }
            if (outputString.Length > 2)
            {
                return outputString.ToString(0, outputString.Length - 1);
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
