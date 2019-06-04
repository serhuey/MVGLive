using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MVGTimeTable
{
    /// <summary>
    /// Set graphic logo
    /// </summary>
    public class ProductLabelMVC : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4) return null;

            Uri uriSource = null;
            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();
            string destination = values[2].ToString().ToUpperInvariant();
            bool sev = (bool)values[3];

            bool bus = product.Contains("BUS");
            bool night = label.Length > 0 && label[0] == 'N';
            bool tram = product.Contains("TRAM");
            bool ubahn = product.Contains("UBAHN");
            bool sbahn = product.Contains("SBAHN");

            if (bus)
            {
                if (sev)
                {
                    if (night)
                    {
                        uriSource = new Uri(Common.ImagePath + "NSevBus.png");
                    }
                    else
                    {
                        // SEV BUS
                        uriSource = new Uri(Common.ImagePath + "SevBus.png");
                    }
                }
                else
                {
                    if (night)
                    {
                        // NIGHT BUS
                        uriSource = new Uri(Common.ImagePath + "NBus.png");
                    }
                    else
                    {
                        // BUS
                        uriSource = new Uri(Common.ImagePath + "Bus.png");
                    }
                }
            }

            if (tram)
            {
                if (night)
                {
                    // NIGHT TRAM
                    uriSource = new Uri(Common.ImagePath + "NTram.png");
                }
                else
                {
                    // TRAM
                    uriSource = new Uri(Common.ImagePath + "Tram.png");
                }
            }

            if (ubahn)
            {
                switch (label)
                {
                    case "U1":
                        uriSource = new Uri(Common.ImagePath + "U1.png");
                        break;
                    case "U2":
                        uriSource = new Uri(Common.ImagePath + "U2.png");
                        break;
                    case "U3":
                        uriSource = new Uri(Common.ImagePath + "U3.png");
                        break;
                    case "U4":
                        uriSource = new Uri(Common.ImagePath + "U4.png");
                        break;
                    case "U5":
                        uriSource = new Uri(Common.ImagePath + "U5.png");
                        break;
                    case "U6":
                        uriSource = new Uri(Common.ImagePath + "U6.png");
                        break;
                    case "U7":
                        uriSource = new Uri(Common.ImagePath + "U7.png");
                        break;
                    case "U8":
                        uriSource = new Uri(Common.ImagePath + "U8.png");
                        break;
                    default:
                        uriSource = new Uri(Common.ImagePath + "UBahn.png");
                        break;
                }
            }

            if (sbahn)
            {
                switch (label)
                {
                    case "S1":
                        if (destination.Contains("FLUGHAFEN"))
                            uriSource = new Uri(Common.ImagePath + "S1FH.png");
                        else
                            uriSource = new Uri(Common.ImagePath + "S1.png");
                        break;
                    case "S2":
                        uriSource = new Uri(Common.ImagePath + "S2.png");
                        break;
                    case "S3":
                        uriSource = new Uri(Common.ImagePath + "S3.png");
                        break;
                    case "S4":
                        uriSource = new Uri(Common.ImagePath + "S4.png");
                        break;
                    case "S6":
                        uriSource = new Uri(Common.ImagePath + "S6.png");
                        break;
                    case "S7":
                        uriSource = new Uri(Common.ImagePath + "S7.png");
                        break;
                    case "S8":
                        if (destination.Contains("FLUGHAFEN"))
                            uriSource = new Uri(Common.ImagePath + "S8FH.png");
                        else
                            uriSource = new Uri(Common.ImagePath + "S8.png");
                        break;
                    case "S20":
                        uriSource = new Uri(Common.ImagePath + "S20.png");
                        break;
                    default:
                        uriSource = new Uri(Common.ImagePath + "SBahn.png");
                        break;
                }
            }

            if (uriSource != null)
            {
                BitmapImage bitmapImage = new BitmapImage(uriSource);
                return bitmapImage;
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
