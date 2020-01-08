using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class SBahnGleisMVC : IMultiValueConverter
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

            if ((Common.multiplatformSbahnStations.Contains(station) &&
                product.Contains("SBAHN")) ||
                (Common.multiplatformTramStations.Contains(station) &&
                product.Contains("TRAM")) &&
                !string.IsNullOrEmpty(gleis))
            {
                int index;
                string gleisSymbol = null;
                
                if(int.TryParse(Regex.Match(gleis, @"\d+").Value, out index) && index > 0 && index < Common.digits.Length)
                {
                    gleisSymbol = Common.digits[index];
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
