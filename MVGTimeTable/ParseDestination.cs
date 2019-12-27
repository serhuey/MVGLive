using System;
using System.Text;
using System.Windows.Media.Imaging;

namespace MVGTimeTable
{
    static class ParseDestination
    {
        /// <summary>
        /// Get main destination string from full destination or null
        /// </summary>
        /// <param name="destination">>Full destination string</param>
        /// <param name="remove_U_S">Remove letters U and S if true</param>
        /// <returns>New string with main destination</returns>
        static public string GetMainDestination(string destination, bool remove_U_S = false)
        {
            if (String.IsNullOrEmpty(destination)) return null;

            StringBuilder outputString = new StringBuilder();

            if (remove_U_S) destination = Remove_U_S(destination);

            string[] splittedDestination = destination.Split(' ');

            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA") break;
                outputString.Append(str + " ");
            }
            if (outputString.Length > 2)
            {
                return (outputString.ToString(0, outputString.Length - 1)).TrimEnd(' ');
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get additional destination string from full destination or null
        /// </summary>
        /// <param name="destination">Full destination string</param>
        /// <param name="remove_U_S">Remove letters U and S if true</param>
        /// <returns>New string with additional destination</returns>
        static public string GetAdditionalDestination(string destination, bool remove_U_S = false)
        {
            if (String.IsNullOrEmpty(destination)) return null;

            if (remove_U_S) destination = Remove_U_S(destination);

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
                return (outputString.ToString(0, outputString.Length - 1)).TrimEnd(' ');
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get both destinations if they are all needed. It's little bit faster than getting them separetely
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="mainDestination"></param>
        /// <param name="additionalDestination"></param>
        /// <param name="remove_U_S_main"></param>
        /// <param name="remove_U_S_additional"></param>
        static public void GetBothDestinations(string destination, out string mainDestination, out string additionalDestination, bool remove_U_S_main = false, bool remove_U_S_additional = false)
        {

            mainDestination = "";
            additionalDestination = "";

            if (String.IsNullOrEmpty(destination)) return;

            string[] splittedDestination = destination.Split(' ');

            StringBuilder outputMainString = new StringBuilder();
            StringBuilder outputAdditionalString = new StringBuilder();

            bool startBuildingAdditionalString = false;
            bool endBuildingMainString = false;

            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA" || startBuildingAdditionalString)
                {
                    startBuildingAdditionalString = true;
                    outputAdditionalString.Append(str + " ");
                }

                if (!endBuildingMainString)
                {
                    if (str.ToUpperInvariant() == "VIA") endBuildingMainString = true;
                    outputMainString.Append(str + " ");
                }
            }

            if (outputAdditionalString.Length > 2)
            {
                additionalDestination = (outputAdditionalString.ToString(0, outputAdditionalString.Length - 1)).TrimEnd(' ');
                if (remove_U_S_additional) additionalDestination = Remove_U_S(additionalDestination);
            }

            if (outputMainString.Length > 2)
            {
                mainDestination = (outputMainString.ToString(0, outputMainString.Length - 1)).TrimEnd(' ');
                if (remove_U_S_main) mainDestination = Remove_U_S(mainDestination);
            }


        }

        /// <summary>
        /// True if string contains markers of U- and S-bahn
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns></returns>
        static public bool IsStringContains_U_S(string destination)
        {
            bool result = false;

            if (!String.IsNullOrEmpty(destination))
            {
                destination += " ";
                result = (destination.IndexOf(" S ") > 0) | (destination.IndexOf(" U ") > 0);
            }

            return result;
        }


        /// <summary>
        /// Get image with "U", "S" or "U S" for destination
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns></returns>
        static public BitmapImage Get_U_S_Image(string destination)
        {
            if (String.IsNullOrEmpty(destination)) return null;

            Uri uriSource = null;

            bool UbahnPresent = false;
            bool SbahnPresent = false;

            int products = 0;

            string[] splittedDestination = destination.Split(' ');

            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "U")
                {
                    UbahnPresent = true;
                }

                if (str.ToUpperInvariant() == "S")
                {
                    SbahnPresent = true;
                }
            }

            products = (UbahnPresent ? 1 : 0) + (SbahnPresent ? 2 : 0);

            switch (products)
            {
                case 1: uriSource = new Uri(Common.ImagePath + "UBahn_50.png"); break;
                case 2: uriSource = new Uri(Common.ImagePath + "SBahn_50.png"); break;
                case 3: uriSource = new Uri(Common.ImagePath + "USBahns_50.png"); break;
                default: break;
            }

            if (uriSource != null)
            {
                BitmapImage bitmapImage = new BitmapImage(uriSource);
                return bitmapImage;
            }
            else
                return null;
        }


        /// <summary>
        /// Remove Letter U and S from destination string
        /// </summary>
        /// <param name="destination"></param>
        /// <returns>New string without U and S</returns>
        static private string Remove_U_S(string destination)
        {
            if (String.IsNullOrEmpty(destination)) return null;

            destination += " ";
            destination = destination.Replace(" U S ", " ");
            destination = destination.Replace(" S U ", " ");
            destination = destination.Replace(" U ", " ");
            destination = destination.Replace(" S ", " ");
            destination = destination.TrimEnd(' ');

            return destination;
        }
    }
}
