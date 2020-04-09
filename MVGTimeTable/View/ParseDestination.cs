// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace MVGTimeTable
{
    internal static class ParseDestination
    {
        /// ************************************************************************************************
        /// <summary>
        /// Get main destination string from full destination or null
        /// </summary>
        /// <param name="destination">>Full destination string</param>
        /// <param name="removeUS">Remove letters U and S if true</param>
        /// <returns>New string with main destination</returns>
        static public string GetMainDestination(string destination, bool removeUS = false, bool removeSplitMarkers = false, bool removeBf = false, bool removeBrackets = false)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }

            StringBuilder outputStringBuilder = new StringBuilder();

            if (removeUS)
            {
                destination = RemoveUS(destination);
            }

            if (removeBf)
            {
                destination = RemoveBf(destination);
            }

            if (removeBrackets)
            {
                destination = RemoveBrackets(destination);
            }

            if (removeSplitMarkers)
            {
                destination = RemoveSplitMarkers(destination);
            }

            string[] splittedDestination = destination.Split(' ');

            foreach (string str in splittedDestination)
            {
                if (Array.IndexOf(Common.AdditionalDestinationMarkers, str.ToUpperInvariant()) >= 0)
                {
                    break;
                }
                outputStringBuilder.Append(str);
                outputStringBuilder.Append(" ");
            }
            if (outputStringBuilder.Length > 2)
            {
                return (outputStringBuilder.ToString(0, outputStringBuilder.Length - 1)).TrimEnd(new char[] { ' ', '-' });
            }
            else
            {
                return null;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get additional destination string from full destination or null
        /// </summary>
        /// <param name="destination">Full destination string</param>
        /// <param name="remove_U_S">Remove letters U and S if true</param>
        /// <returns>New string with additional destination</returns>
        static public string GetAdditionalDestination(string destination, bool remove_U_S = false)
        {
            if (string.IsNullOrEmpty(destination)) return null;

            if (remove_U_S) destination = RemoveUS(destination);

            string[] splittedDestination = destination.Split(' ');

            StringBuilder outputStringBuilder = new StringBuilder();
            bool startBuilding = false;
            foreach (string str in splittedDestination)
            {
                if (Array.IndexOf(Common.AdditionalDestinationMarkers, str.ToUpperInvariant()) >= 0 || startBuilding)
                {
                    startBuilding = true;
                    outputStringBuilder.Append(str);
                    outputStringBuilder.Append(" ");
                }
            }

            if (outputStringBuilder.Length > 2)
            {
                return (outputStringBuilder.ToString(0, outputStringBuilder.Length - 1)).TrimEnd(' ').TrimStart('-');
            }
            else
            {
                return null;
            }
        }

        /// ************************************************************************************************
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

            StringBuilder outputMainStringBuilder = new StringBuilder();
            StringBuilder outputAdditionalStringBuilder = new StringBuilder();

            bool startBuildingAdditionalstring = false;
            bool endBuildingMainstring = false;

            foreach (string str in splittedDestination)
            {
                if (Array.IndexOf(Common.AdditionalDestinationMarkers, str.ToUpperInvariant()) >= 0 || startBuildingAdditionalstring)
                {
                    startBuildingAdditionalstring = true;
                    outputAdditionalStringBuilder.Append(str);
                    outputAdditionalStringBuilder.Append(" ");
                }

                if (!endBuildingMainstring)
                {
                    if (Array.IndexOf(Common.AdditionalDestinationMarkers, str.ToUpperInvariant()) >= 0)
                    {
                        endBuildingMainstring = true;
                    }

                    outputMainStringBuilder.Append(str);
                    outputMainStringBuilder.Append(" ");
                }
            }

            if (outputAdditionalStringBuilder.Length > 2)
            {
                additionalDestination = (outputAdditionalStringBuilder.ToString(0, outputAdditionalStringBuilder.Length - 1)).TrimEnd(' ').TrimStart('-');
                if (remove_U_S_additional)
                {
                    additionalDestination = RemoveUS(additionalDestination);
                }
            }

            if (outputMainStringBuilder.Length > 2)
            {
                mainDestination = (outputMainStringBuilder.ToString(0, outputMainStringBuilder.Length - 1)).TrimEnd(new char[] { ' ', '-' });
                if (remove_U_S_main)
                {
                    mainDestination = RemoveUS(mainDestination);
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// True if string contains markers of U- and S-bahn
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns></returns>
        static public bool IsStringContainsUS(string destination)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(destination))
            {
                destination += " ";
                result = IsMarkerPresent(destination, Common.USSpacedMarkers, exactly: true);
            }

            return result;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get image with "U", "S", "U S", First Cars, Last Cars, Ball, Zoo, Messe and Olymplic signs for destination
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns></returns>
        public static BitmapImage GetDestinationImage(string destination, string label = "", string currentStation = "", bool mainLabel = true)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }

            string iconKey = null;

            if (!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(currentStation))
            {
                if (IsFussballDestination(currentStation, destination, label))
                {
                    iconKey = Common.FussballIconKey;
                }
                if (IsZooDestination(currentStation, destination, label))
                {
                    iconKey = Common.ZooIconKey;
                }
                if (IsMesseDestination(currentStation, destination, label))
                {
                    iconKey = Common.MesseIconKey;
                }
                if (IsOlympiaDestination(currentStation, destination, label))
                {
                    iconKey = Common.OlympiaIconKey;
                }
            }

            if (string.IsNullOrEmpty(iconKey))
            {
                bool trainFirstPresent = false;
                bool trainSecondPresent = false;
                bool UbahnPresent = false;
                bool SbahnPresent = false;
                string[] splittedDestination = destination.Split(' ');

                foreach (string str in splittedDestination)
                {
                    string strU = str.ToUpperInvariant();
                    if (strU == Common.TrainFirstHalf.ToUpperInvariant())
                    {
                        trainFirstPresent = true;
                    }

                    if (strU == Common.TrainSecondHalf.ToUpperInvariant())
                    {
                        trainSecondPresent = true;
                    }

                    if (IsMarkerPresent(strU, Common.ULineMarkers, exactly: true))
                    {
                        UbahnPresent = true;
                    }
                    if (IsMarkerPresent(strU, Common.SLineMarkers, exactly: true))
                    {
                        SbahnPresent = true;
                    }

                    if (IsMarkerPresent(strU, Common.USLineMarkers, exactly: true))
                    {
                        SbahnPresent = true;
                        UbahnPresent = true;
                    }
                }
                if (trainFirstPresent || trainSecondPresent)
                {
                    iconKey = trainFirstPresent ? Common.TrainFirstHalfIconKey : Common.TrainSecondHalfIconKey;
                }
                else
                {
                    int products = (UbahnPresent ? 1 : 0) + (SbahnPresent ? 2 : 0);
                    switch (products)
                    {
                        case 1: iconKey = mainLabel ? Common.UBahnMonoFullIconKey : Common.UBahnMonoHalfIconKey; break;
                        case 2: iconKey = mainLabel ? Common.SBahnMonoFullIconKey : Common.SBahnMonoHalfIconKey; break;
                        case 3: iconKey = mainLabel ? Common.USBahnsMonoFullIconKey : Common.USBahnsMonoHalfIconKey; break;
                        default: break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey = iconKey.ToUpperInvariant()))
            {
                return Common.icons[iconKey];
            }
            else
            {
                return null;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Remove Letter U and S from destination string
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns>New string without U and S</returns>
        static private string RemoveUS(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }

            destination += " ";
            foreach (string s in Common.USSpacedMarkers)
            {
                destination = destination.Replace(s, " ");
            }

            destination = destination.TrimEnd(' ');

            return destination;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Remove split markers from destination. This markers added during parsing and will be replaced with icons in the table
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns>Destination string without split markers</returns>
        static private string RemoveSplitMarkers(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }
            destination += " ";
            destination = destination.Replace(Common.TrainFirstHalf, " ");
            destination = destination.Replace(Common.TrainSecondHalf, " ");
            destination = destination.TrimEnd(' ');

            return destination;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Remove letters Bf or Bf. from destination string
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns>New string without Bf or Bf.</returns>
        static private string RemoveBf(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }

            destination += " ";
            destination = destination.Replace(" Bf ", " ");
            destination = destination.Replace(" Bf. ", " ");
            destination = destination.TrimEnd(' ');

            return destination;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Remove text in brackets from destination string
        /// </summary>
        /// <param name="destination">Destination string</param>
        /// <returns>Destination string without text in brackets</returns>
        static private string RemoveBrackets(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                return null;
            }
            Regex.Replace(destination, @"\([^()]*\)", string.Empty);
            return destination;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Check football destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="label">MVV Line number</param>
        /// <returns>True if line is going to the Alliance Arena</returns>
        static private bool IsFussballDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestinationId(currentStation: currentStation,
                                        destination: destination,
                                        label: label,
                                        targetDictionary: Common.FussballDestinationsId);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Check Zoo destination
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <param name="label">MVV Line number</param>
        /// <returns>True if line is going to the Tierpark Hellabrunn</returns>
        static private bool IsZooDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestinationId(currentStation: currentStation,
                                        destination: destination,
                                        label: label,
                                        targetDictionary: Common.ZooDestinationsId);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Check Messe destination
        /// </summary>
        /// <param name="currentStation"></param>
        /// <param name="destination"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        static private bool IsMesseDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestinationId(currentStation: currentStation,
                                        destination: destination,
                                        label: label,
                                        targetDictionary: Common.MesseDestinationsId);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Check Olympia destination
        /// </summary>
        /// <param name="currentStation"></param>
        /// <param name="destination"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        static private bool IsOlympiaDestination(string currentStation, string destination, string label)
        {
            return IsTargetDestinationId(currentStation: currentStation,
                                        destination: destination,
                                        label: label,
                                        targetDictionary: Common.OlympiaDestinationsId);
        }

        /// ************************************************************************************************
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
                if (string.Compare(key.ToUpperInvariant(), label.ToUpperInvariant()) != 0)
                {
                    continue;
                }

                foreach (string targetDestination in targetDictionary[key])
                {
                    if (string.Compare(currentStation.ToUpperInvariant(), targetDestination.ToUpperInvariant()) == 0)
                    {
                        targetPassed = true;
                    }

                    if (string.Compare(targetDestination.ToUpperInvariant(), destination.ToUpperInvariant()) == 0)
                    {
                        destinationDetected = true;
                    }
                }
            }
            return !targetPassed & destinationDetected;
        }

        /// ************************************************************************************************
        static private bool IsTargetDestinationId(string currentStation, string destination, string label, Dictionary<string, string[]> targetDictionary)
        {
            return string.Equals(label, "LINIE")
                ? false
                : IsTargetDestination(currentStation: MVGAPI.MVGAPI.GetIdForStation(GetMainDestination(currentStation, removeSplitMarkers: true, removeUS: true)),
                                        destination: MVGAPI.MVGAPI.GetIdForStation(GetMainDestination(destination, removeSplitMarkers: true, removeUS: true)),
                                        label: label,
                                        targetDictionary: targetDictionary);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Check, does the marker from special array exist in the input string
        /// </summary>
        /// <param name="checkedString">Input string, find markers here</param>
        /// <param name="markersArray">Special array with markers</param>
        /// <param name="exactly">If true the whole string has to match with marker</param>
        /// <returns>True if a marker has found in the input string</returns>
        static public bool IsMarkerPresent(string checkedString, string[] markersArray, bool exactly = false)
        {
            bool result = false;
            foreach (string marker in markersArray)
            {
                if (exactly)
                {
                    if (string.Compare(marker, checkedString) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                else
                {
                    if (checkedString.Contains(marker))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Adding special markers in the destination string if the train is splitted in the future
        /// This markers will be replaced with icons by destination converters
        /// Works only with array sorted with MVGAPI. If array is not sorted, use sortDepartures flag
        /// </summary>
        /// <param name="departureResponse">DeserializedDepartures array</param>
        /// <param name="sortDepartures">Sort input array by MVGAPI if true</param>
        static public void ProcessForkLines(ref DeserializedDepartures[] departureResponse, bool sortDepartures = false)
        {
            if (departureResponse == null || departureResponse.Length < 2)
            {
                return;
            }

            if (sortDepartures)
            {
                MVGAPI.MVGAPI.Sort(ref departureResponse);
            }

            for (int i = 0; i < departureResponse.Length - 1; ++i)
            {
                foreach (string label in Common.ForkedDestinationsId.Keys)
                {
                    if (label == departureResponse[i].label &&
                        label == departureResponse[i + 1].label &&
                        departureResponse[i].departureTime == departureResponse[i + 1].departureTime)
                    {
                        string splittedId0 = Common.ForkedDestinationsId[label][0];
                        string splittedId1 = Common.ForkedDestinationsId[label][1];
                        string destinationId0 = MVGAPI.MVGAPI.GetIdForStation(GetMainDestination(departureResponse[i].destination, removeSplitMarkers: true, removeUS: true, removeBf: true));
                        string destinationId1 = MVGAPI.MVGAPI.GetIdForStation(GetMainDestination(departureResponse[i + 1].destination, removeSplitMarkers: true, removeUS: true, removeBf: true));
                        if (splittedId0 == destinationId0 && splittedId1 == destinationId1)
                        {
                            departureResponse[i].destination += " " + Common.TrainFirstHalf;
                            departureResponse[i + 1].destination += " " + Common.TrainSecondHalf;
                            continue;
                        }
                        if (splittedId0 == destinationId1 && splittedId1 == destinationId0)
                        {
                            departureResponse[i].destination += " " + Common.TrainSecondHalf;
                            departureResponse[i + 1].destination += " " + Common.TrainFirstHalf;
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}