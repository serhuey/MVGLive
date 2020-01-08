using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MVGTimeTable
{
    /// <summary>
    /// Set graphic logo
    /// </summary>
    public class ProductLabelMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length < 4 ||
                values[0] == null ||
                values[1] == null ||
                values[2] == null ||
                values[3] == null)
                return null;

            string iconKey = null;
            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();
            string destination = values[2].ToString().ToUpperInvariant();
            bool sev = (bool)values[3];

            bool bus = product.Contains("BUS");
            bool night = label.Length > 0 && label[0] == 'N';
            bool tram = product.Contains("TRAM");
            bool ubahn = product.Contains("UBAHN");
            bool sbahn = product.Contains("SBAHN");
            bool xbus = label.Contains("X") && label[0] == 'X';
            bool noConnection = product.Contains("NO_CONNECTION");
            bool warning = product.Contains("WARNING");

            if (bus)
            {
                if (sev)
                {
                    if (night)
                    {
                        //uriSource = "NSevBus";
                        iconKey = "NSevBus";
                    }
                    else
                    {
                        // SEV BUS
                        iconKey = "SevBus";
                    }
                }
                else
                {
                    if (night)
                    {
                        // NIGHT BUS
                        iconKey = "NBus";
                    }
                    else
                    {
                        // ExpressBUS
                        if (xbus)
                        {
                            iconKey = "ExpressBus";
                        }
                        // BUS
                        else
                        {
                            iconKey = "Bus";
                        }
                    }
                }
            }

            if (tram)
            {
                if (night)
                {
                    // NIGHT TRAM
                    iconKey = "NTram";
                }
                else
                {
                    // TRAM
                    iconKey = "Tram";
                }
            }

            if (ubahn)
            {
                switch (label)
                {
                    case "U1":
                        iconKey = "U1";
                        break;
                    case "U2":
                        iconKey = "U2";
                        break;
                    case "U3":
                        iconKey = "U3";
                        break;
                    case "U4":
                        iconKey = "U4";
                        break;
                    case "U5":
                        iconKey = "U5";
                        break;
                    case "U6":
                        iconKey = "U6";
                        break;
                    case "U7":
                        iconKey = "U7";
                        break;
                    case "U8":
                        iconKey = "U8";
                        break;
                    default:
                        iconKey = "UBahn";
                        break;
                }
            }

            if (sbahn)
            {
                switch (label)
                {
                    case "S1":
                        if (destination.Contains("FLUGHAFEN"))
                            iconKey = "S1FH";
                        else
                            iconKey = "S1";
                        break;
                    case "S2":
                        iconKey = "S2";
                        break;
                    case "S3":
                        iconKey = "S3";
                        break;
                    case "S4":
                        iconKey = "S4";
                        break;
                    case "S6":
                        iconKey = "S6";
                        break;
                    case "S7":
                        iconKey = "S7";
                        break;
                    case "S8":
                        if (destination.Contains("FLUGHAFEN"))
                            iconKey = "S8FH";
                        else
                            iconKey = "S8";
                        break;
                    case "S20":
                        iconKey = "S20";
                        break;
                    default:
                        iconKey = "SBahn";
                        break;
                }
            }

            if(warning)
            {
                iconKey = "Warning";
            }

            if(noConnection)
            {
                iconKey = "NoConnection";
            }

            if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey = iconKey.ToLowerInvariant()))
            {
                return Common.icons[iconKey];
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
