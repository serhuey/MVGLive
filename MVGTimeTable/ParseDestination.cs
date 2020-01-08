using System;
using System.Collections.Generic;
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
            if (string.IsNullOrEmpty(destination)) return null;

            StringBuilder outputstring = new StringBuilder();

            if (remove_U_S) destination = Remove_U_S(destination);

            string[] splittedDestination = destination.Split(' ');

            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA") break;
                outputstring.Append(str + " ");
            }
            if (outputstring.Length > 2)
            {
                return (outputstring.ToString(0, outputstring.Length - 1)).TrimEnd(' ');
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
            if (string.IsNullOrEmpty(destination)) return null;

            if (remove_U_S) destination = Remove_U_S(destination);

            string[] splittedDestination = destination.Split(' ');

            StringBuilder outputstring = new StringBuilder();
            bool startBuilding = false;
            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA" || startBuilding)
                {
                    startBuilding = true;
                    outputstring.Append(str + " ");
                }
            }
            if (outputstring.Length > 2)
            {
                return (outputstring.ToString(0, outputstring.Length - 1)).TrimEnd(' ');
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get both destinations if they are all needed. It's little bit faster than getting them separetely
        /// </summary>
        /// <param name="destination">Full destination string</param>
        /// <param name="mainDestination">Reference to the string with main destination</param>
        /// <param name="additionalDestination">Reference to the string with additional destination</param>
        /// <param name="remove_U_S_main">Remove letters U and S in main destination if true</param>
        /// <param name="remove_U_S_additional">Remove letters U and S in additional destination if true</param>
        static public void GetBothDestinations(string destination, out string mainDestination, out string additionalDestination, bool remove_U_S_main = false, bool remove_U_S_additional = false)
        {

            mainDestination = "";
            additionalDestination = "";

            if (string.IsNullOrEmpty(destination)) return;

            string[] splittedDestination = destination.Split(' ');

            StringBuilder outputMainstring = new StringBuilder();
            StringBuilder outputAdditionalstring = new StringBuilder();

            bool startBuildingAdditionalstring = false;
            bool endBuildingMainstring = false;

            foreach (string str in splittedDestination)
            {
                if (str.ToUpperInvariant() == "VIA" || startBuildingAdditionalstring)
                {
                    startBuildingAdditionalstring = true;
                    outputAdditionalstring.Append(str + " ");
                }

                if (!endBuildingMainstring)
                {
                    if (str.ToUpperInvariant() == "VIA") endBuildingMainstring = true;
                    outputMainstring.Append(str + " ");
                }
            }

            if (outputAdditionalstring.Length > 2)
            {
                additionalDestination = (outputAdditionalstring.ToString(0, outputAdditionalstring.Length - 1)).TrimEnd(' ');
                if (remove_U_S_additional) additionalDestination = Remove_U_S(additionalDestination);
            }

            if (outputMainstring.Length > 2)
            {
                mainDestination = (outputMainstring.ToString(0, outputMainstring.Length - 1)).TrimEnd(' ');
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

            if (!string.IsNullOrEmpty(destination))
            {
                destination += " ";
                result = (destination.IndexOf(" S ") > 0) |
                            (destination.IndexOf(" U ") > 0) |
                            (destination.IndexOf(" US ") > 0) |
                            (destination.IndexOf(" SU ") > 0);
            }

            return result;
        }


        /// <summary>
        /// Get image with "U", "S", "U S", Ball oder Zoo for destination
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns></returns>
        static public BitmapImage GetDestinationImage(string destination, string label = "", string currentStation = "", bool mainLabel = true)
        {
            if (string.IsNullOrEmpty(destination)) return null;

            string iconKey = null;

            if (!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(currentStation))
            {
                if (IsFussballDestination(currentStation, destination, label))
                {
                    iconKey = "Fussball";
                }
                if (IsZooDestination(currentStation, destination, label))
                {
                    iconKey = "Zoo";
                }
                if (IsMesseDestination(currentStation, destination, label))
                {
                    iconKey = "Messe";
                }
                if (IsOlympiaDestination(currentStation, destination, label))
                {
                    iconKey = "Olympia";
                }
            }

            if (string.IsNullOrEmpty(iconKey))
            {
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

                    if (str.ToUpperInvariant() == "SU" || str.ToUpperInvariant() == "US")
                    {
                        SbahnPresent = true;
                        UbahnPresent = true;
                    }
                }

                products = (UbahnPresent ? 1 : 0) + (SbahnPresent ? 2 : 0);

                switch (products)
                {
                    case 1: iconKey = mainLabel ? "UBahn_100" : "UBahn_50"; break;
                    case 2: iconKey = mainLabel ? "UBahn_100" : "SBahn_50"; break;
                    case 3: iconKey = mainLabel ? "USBahns_100" : "USBahns_50"; break;
                    default: break;
                }
            }

            if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey = iconKey.ToLowerInvariant()))
            {
                return Common.icons[iconKey];
            }
            else
                return null;
        }


        /// <summary>
        /// Remove Letter U and S from destination string
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns>New string without U and S</returns>
        static private string Remove_U_S(string destination)
        {
            if (string.IsNullOrEmpty(destination)) return null;

            destination += " ";
            destination = destination.Replace(" US ", " ");
            destination = destination.Replace(" SU ", " ");
            destination = destination.Replace(" U S ", " ");
            destination = destination.Replace(" S U ", " ");
            destination = destination.Replace(" U ", " ");
            destination = destination.Replace(" S ", " ");
            destination = destination.TrimEnd(' ');

            return destination;
        }

        /// <summary>
        /// Check football destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="label">MVV Line number</param>
        /// <returns>True if line is going to the Alliance Arena</returns>
        static private bool IsFussballDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestination(currentStation, destination, label, Common.FussballDestinations);
        }

        /// <summary>
        /// Check Zoo destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="label">MVV Line number</param>
        /// <returns>True if line is going to the Tierpark Hellabrunn</returns>
        static private bool IsZooDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestination(currentStation, destination, label, Common.ZooDestinations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentStation"></param>
        /// <param name="destination"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        static private bool IsMesseDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestination(currentStation, destination, label, Common.MesseDestinations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentStation"></param>
        /// <param name="destination"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        static private bool IsOlympiaDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestination(currentStation, destination, label, Common.OlympiaDestinations);
        }


        /// <summary>
        /// Check target destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="label">MVV Line number</param>
        /// <param name="targetDictionary">Target Dictionary</param>
        /// <returns></returns>
        static private bool IsTargetDestination(string currentStation, string destination, string label, Dictionary<string, string[]> targetDictionary)
        {
            bool destinationDetected = false;
            bool targetPassed = false;
            foreach (string key in targetDictionary.Keys)
            {
                if (string.Compare(key.ToUpperInvariant(), label.ToUpperInvariant()) != 0) continue;
                foreach (string targetDestination in targetDictionary[key])
                {
                    if (string.Compare(currentStation.ToUpperInvariant(), targetDestination.ToUpperInvariant()) == 0) targetPassed = true;
                    if (string.Compare(targetDestination.ToUpperInvariant(), destination.ToUpperInvariant()) == 0) destinationDetected = true;
                }
            }
            return !targetPassed & destinationDetected;
        }
    }
}
