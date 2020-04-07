// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Svg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace MVGTimeTable
{
    public enum Column
    {
        Line,
        Destination,
        TimeToDeparture,
        DepartureTime,
        Platform
    }

    public enum MessageType
    {
        Warning,
        NoConnection,
        Waiting
    }

    internal static class Common
    {
        public const int UtmostColumn = 1;     //index of truncated column
        public const string SvgPath = "pack://application:,,,/MVGTimeTable;component/SVGIcons/";
        public static readonly List<string> MultiplatformSbahnStations = new List<string>(new string[] { "OSTBAHNHOF", "PASING", "LAIM" });
        public static readonly List<string> MultiplatformTramStations = new List<string>(new string[] { "KARLSPLATZ" });

        public static readonly Dictionary<string, string[]> FussballDestinationsId =
            new Dictionary<string, string[]> { { "U6", new string[] {   "de:09162:470",         // Fröttmaning
                                                                        "de:09184:460",         // Garching, Forschungszentrum
                                                                        "de:09184:480",         // Garching-Hochbrück
                                                                        "de:09184:490" } } };   // Garching

        public static readonly Dictionary<string, string[]> ZooDestinationsId =
            new Dictionary<string, string[]> { { "U3", new string[] {   "de:09162:1500",        // Fürstenried West
                                                                        "de:09162:1490",        // Basler Straße
                                                                        "de:09162:1480",        // Forstenrieder Allee
                                                                        "de:09162:1470",        // Machtlfinger Straße
                                                                        "de:09162:1460",        // Aidenbachstraße
                                                                        "de:09162:1450",        // Obersendling
                                                                        "de:09162:1440" } },    // Thalkirchen
                                                { "135", new string[] { "de:09162:1440" } },    // Thalkirchen
                                                {  "52", new string[] { "de:09162:1165" } },    // Tierpark (Alemannenstr.)
                                                { "X98", new string[] { "de:09162:1165" } } };  // Tierpark (Alemannenstr.)

        public static readonly Dictionary<string, string[]> MesseDestinationsId =
            new Dictionary<string, string[]> { { "U2", new string[] { "de:09162:1260" } } };  // Messestadt Ost

        public static readonly Dictionary<string, string[]> OlympiaDestinationsId =
            new Dictionary<string, string[]> {  { "U8", new string[] {  "de:09162:350" } },     // Olympiazentrum
                                                { "U3", new string[] {  "de:09162:350",         // Olympiazentrum
                                                                        "de:09162:380",         // Oberwiesenfeld
                                                                        "de:09162:360",         // Olympia-Einkaufszentrum
                                                                        "de:09162:370",         // Moosacher St.-Martins-Platz
                                                                        "de:09162:300" } } };   // Moosach

        public static readonly Dictionary<string, string[]> ForkedDestinationsId =
            new Dictionary<string, string[]> { { "S1", new string[] {   "de:09178:2680",         // 0 - first cars (Freising)
                                                                        "de:09162:3240" } } };   // 1 - last cars (Flughafen München)

        public static readonly Dictionary<Column, string> ColumnName =
            new Dictionary<Column, string> {   { Column.Line, "Linie" },
                                                { Column.Destination, "Ziel"},
                                                { Column.TimeToDeparture, "Abfahrt"},
                                                { Column.DepartureTime, "Abfahrtszeit"},
                                                { Column.Platform, "Gleis"} };

        public static readonly Dictionary<MessageType, string> WarnMessageType =
            new Dictionary<MessageType, string> {   { MessageType.NoConnection, "NO_CONNECTION" },
                                                    { MessageType.Waiting, "WAITING" },
                                                    { MessageType.Warning, "WARNING"  }};

        public static readonly Dictionary<MessageType, string> Messages =
            new Dictionary<MessageType, string> {   { MessageType.NoConnection, "www.mvg.de - Die Verbindung fehlgeschlagen"},
                                                    { MessageType.Waiting, "www.mvg.de - Warten auf die Verbindung"},
                                                    { MessageType.Warning, "Die Daten sind möglicherweise nicht relevant"}};

        public static readonly string[] UBahnIconKey = { "U", "U1", "U2", "U3", "U4", "U5", "U6", "U7", "U8" };

        public static readonly string[] SBahnIconKey = { "S", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S",
                                                         "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",
                                                         "S20" };

        public static readonly string[] TramIconKey = { "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM",
                                                        "TRAM", "TRAM", "TRAM12", "TRAM", "TRAM", "TRAM15", "TRAM16", "TRAM17", "TRAM18", "TRAM19",
                                                         "TRAM20", "TRAM21", "TRAM", "TRAM23", "TRAM", "TRAM25", "TRAM", "TRAM27", "TRAM28", "TRAM29"};

        public static readonly string[] GleisIconKey = { "HALTESTELLE", "GLEIS1", "GLEIS2", "GLEIS3", "GLEIS4", "GLEIS5", "GLEIS6", "GLEIS7", "GLEIS8", "GLEIS9",
                                                         "GLEIS10", "GLEIS11", "GLEIS12", "GLEIS13", "GLEIS14", "GLEIS15", "GLEIS16" };

        public static readonly string[] SGleisIconKey = { "HALTESTELLE", "SGLEIS1", "SGLEIS2", "SGLEIS3", "SGLEIS4", "SGLEIS5", "SGLEIS6", "SGLEIS7", "SGLEIS8", "SGLEIS9",
                                                         "SGLEIS10", "SGLEIS11", "SGLEIS12", "SGLEIS13", "SGLEIS14", "SGLEIS15", "SGLEIS16" };
        public static readonly string[] FollowForegroundIconKey1 = {    UBahnMonoFullIconKey, SBahnMonoFullIconKey, USBahnsMonoFullIconKey,
                                                                        TrainFirstHalfIconKey, TrainSecondHalfIconKey, FussballIconKey,
                                                                        ZooIconKey, MesseIconKey, OlympiaIconKey, WaitingIconKey, S1FlughafenIconKey, S8FlughafenIconKey};
        public static readonly string[] FollowForegroundIconKey2 = {    UBahnMonoHalfIconKey, SBahnMonoHalfIconKey, USBahnsMonoHalfIconKey,
                                                                        Delay1IconKey, Delay2IconKey, Delay3IconKey };
        public static readonly string[] FollowForegroundIconKey3 = { NowIconKey };

        public const string DefaultUBahnIconKey = "UBAHN";
        public const string DefaultSBahnIconKey = "SBAHN";
        public const string NSevBusIconKey = "NSEVBUS";
        public const string SevBusIconKey = "SEVBUS";
        public const string SevTramIconKey = "SEVTRAM";
        public const string NBusIconKey = "NBUS";
        public const string ExpressBusIconKey = "EXPRESSBUS";
        public const string BusIconKey = "BUS";
        public const string DefaultTramIconKey = "TRAM";
        public const string NTramIconKey = "NTRAM";
        public const string WarningIconKey = "WARNING";
        public const string NoConnectionIconKey = "NOCONNECTION";
        public const string NowIconKey = "JETZT";
        public const string FussballIconKey = "FUSSBALL";
        public const string ZooIconKey = "ZOO";
        public const string MesseIconKey = "MESSE";
        public const string OlympiaIconKey = "OLYMPIA";
        public const string UBahnMonoFullIconKey = "UBAHN_100";
        public const string UBahnMonoHalfIconKey = "UBAHN_50";
        public const string SBahnMonoFullIconKey = "SBAHN_100";
        public const string SBahnMonoHalfIconKey = "SBAHN_50";
        public const string USBahnsMonoFullIconKey = "USBAHNS_100";
        public const string USBahnsMonoHalfIconKey = "USBAHNS_50";
        public const string TrainFirstHalfIconKey = "FIRSTS1";
        public const string TrainSecondHalfIconKey = "LASTS1";
        public const string Delay0IconKey = "DELAY0";
        public const string Delay1IconKey = "DELAY1";
        public const string Delay2IconKey = "DELAY2";
        public const string Delay3IconKey = "DELAY3";
        public const string WaitingIconKey = "WAIT";
        public const string S1FlughafenIconKey = "S1FH";
        public const string S8FlughafenIconKey = "S8FH";

        public static readonly Dictionary<string, string> AirportIconKeys = new Dictionary<string, string> { { SBahnIconKey[1], S1FlughafenIconKey }, { SBahnIconKey[8], S8FlughafenIconKey } };

        public static readonly string[] AirportMarkers = { "FLUGHAFEN" };
        public static readonly string[] BusMarkers = { "BUS" };
        public static readonly string[] TramMarkers = { "TRAM" };
        public static readonly string[] UBahnMarkers = { "UBAHN" };
        public static readonly string[] SBahnMarkers = { "SBAHN" };
        public static readonly string[] AdditionalDestinationMarkers = { "VIA", "Ü.", "WEITER", "ÜBER" };
        public static readonly string[] NightLineMarkers = { "N" };
        public static readonly string[] ExpressLineMarkers = { "X" };

        public static readonly string[] ULineMarkers = { "U", "(U)" };
        public static readonly string[] SLineMarkers = { "S", "(S)" };
        public static readonly string[] USLineMarkers = { "SU", "US", "(U)(S)", "(S)(U)", "(U) (S)", "(S) (U)" };

        public const string HeaderProduct = "HEADER";

        public static readonly string[] USSpacedMarkers = BuildSpacedUSMarkersArray();

        //public const string TableForegroundColor1 = "#FFE8E8E8";
        //public const string TableForegroundColor2 = "#55F1F1F1";
        //public const string HeaderForegroundColor = "#99E8E8E8";
        //public const string TableBackgroundColor1 = "#FF00056C";
        //public const string TableBackgroundColor2 = "#FF262A74";
        //public const string HeaderBackgroundColor = "#FF000342";
        //public const string NoConnectionForegroundColor = "#FFFF4E48";
        //public const string WarningForegroundColor = "#FFFFEB85";

        public const int UndefinedSignThreshold = -3;

        public const string TimeSignSeparator = "  ";
        public const string MinutesSign = "Min.";
        public const string HoursSign = "Std.";
        public const string UndefinedTimeSign = "  ????";

        public const string TrainFirstHalf = "{-}{V}";
        public const string TrainSecondHalf = "{V}{-}";

        public const int DelayThreshold1 = 2;
        public const int DelayThreshold2 = 5;
        public const int DelayThreshold3 = 10;

        public static Dictionary<string, BitmapImage> icons;

        private static readonly SvgColourServer[] foregroundColor = new SvgColourServer[3];


        /// ************************************************************************************************
        /// <summary>
        /// Create Dictionary with Bitmaps of the same height from SVG sources
        /// </summary>
        /// <param name="icons">Dictionary with icons</param>
        /// <param name="fontSize">Height of icons</param>
        /// <param name="foregroundColor1">Foreground color for main text, it is used to change color of some icons</param>
        /// <param name="foregroundColor2">Foreground color for additional text, it is used to change color of some icons</param>
        /// <returns>true if Dictionary successfully created</returns>
        /// ************************************************************************************************
        public static bool CreateIconsDictionaryFromSVG(out Dictionary<string, BitmapImage> icons,
                                                        double fontSize,
                                                        string foregroundColor1,
                                                        string foregroundColor2,
                                                        string foregroundColor3)
        {
            bool result = true;

            icons = new Dictionary<string, BitmapImage>();
            System.Windows.Media.Color color1 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(foregroundColor1);
            System.Windows.Media.Color color2 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(foregroundColor2);
            System.Windows.Media.Color color3 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(foregroundColor3);

            foregroundColor[0] = new SvgColourServer(Color.FromArgb(color1.R, color1.G, color1.B));
            foregroundColor[1] = new SvgColourServer(Color.FromArgb(color2.R, color2.G, color2.B));
            foregroundColor[2] = new SvgColourServer(Color.FromArgb(color3.R, color3.G, color3.B));

            string[] svgNames = GetResourcesNamesFromFolder("SVGIcons");

            foreach (string svgName in svgNames)
            {
                try
                {
                    string key = Path.GetFileNameWithoutExtension(svgName).ToUpperInvariant();
                    bool changeColor1 = Array.IndexOf(FollowForegroundIconKey1, key) >= 0;
                    bool changeColor2 = Array.IndexOf(FollowForegroundIconKey2, key) >= 0;
                    bool changeColor3 = Array.IndexOf(FollowForegroundIconKey3, key) >= 0;

                    BitmapImage bi = GetBitmapFromSVG(new Uri(SvgPath + svgName), fontSize, changeColor1 || changeColor2 || changeColor3, changeColor1 ? 0 : (changeColor2 ? 1 : 2));
                    icons.Add(key, bi);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    result = false;
                    break;
                }
                catch (UriFormatException ex2)
                {
                    Console.WriteLine(ex2.Message);
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Calculate actual and scheduled times of departure in DateTime format and difference between now and actual departure time
        /// </summary>
        /// <param name="dpTime">Time from DeserializedDeparture in Unix timespan format</param>
        /// <param name="dpDelay">Delay in minutes from DeserializedDeparture</param>
        /// <param name="actualTime">Actual time of departure</param>
        /// <param name="scheduledTime">Scheduled time of departure</param>
        /// <param name="difference">Difference between Actual time and now</param>
        /// ************************************************************************************************
        public static void GetTimes(DateTime now, long dpTime, long dpDelay, out DateTime actualTime, out DateTime scheduledTime, out TimeSpan difference)
        {
            actualTime = DateTimeOffset.FromUnixTimeMilliseconds(dpTime).DateTime.ToLocalTime();
            scheduledTime = DateTimeOffset.FromUnixTimeMilliseconds(dpTime - dpDelay * 1000 * 60).DateTime.ToLocalTime();
            difference = actualTime.Subtract(now);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get all names of embedded resources in project folder
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <returns>Araay of the strings with file names</returns>
        /// ************************************************************************************************
        private static string[] GetResourcesNamesFromFolder(string folder)
        {
            folder = folder.ToLowerInvariant() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            var resourceReader = new ResourceReader(stream);

            var resources =
                from p in resourceReader.OfType<DictionaryEntry>()
                let theme = (string)p.Key
                where theme.StartsWith(folder, StringComparison.InvariantCulture)
                select theme.Substring(folder.Length);

            return resources.ToArray();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get Bitmap with defined height from SVG-file with defined uri
        /// </summary>
        /// <param name="uri">Uri of the SVG-file</param>
        /// <param name="height">Desired height of the BitmapImage</param>
        /// <param name="changeColor">If true, image replaces white color with one of the foreground colors</param>
        /// <param name="newSvgColorIndex"></param>
        /// <returns></returns>
        /// ************************************************************************************************
        private static BitmapImage GetBitmapFromSVG(Uri uri, double height, bool changeColor, int newSvgColorIndex)
        {
            SvgDocument svgDocument;
            XmlDocument xmlDocument = new XmlDocument();

            using (Stream inputStream = Application.GetResourceStream(uri).Stream)
            {
                xmlDocument.Load(inputStream);
            }

            svgDocument = SvgDocument.Open(xmlDocument);
            if (changeColor)
            {
                svgDocument = ChangeColor(svgDocument, newSvgColorIndex);// "#ff00ff00");
            }
            Bitmap bitmap = svgDocument.Draw(0, (int)height);
            BitmapImage bitmapImage = ConvertImageToBitmapImage(bitmap);

            return bitmapImage;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Change color of the SvgDocument
        /// </summary>
        /// <param name="inputDocument">Svg document to change</param>
        /// <param name="newSvgColorIndex">Index of the new foreground color [0-1]</param>
        /// <returns></returns>
        /// ************************************************************************************************
        private static SvgDocument ChangeColor(SvgDocument inputDocument, int newSvgColorIndex)
        {
            SvgDocument outputDocument = inputDocument;
            ApplyRecursive(outputDocument, newSvgColorIndex);
            return outputDocument;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Recursive function for change color of the all elements of the Svg document
        /// </summary>
        /// <param name="svgElement">Svg element</param>
        /// <param name="newSvgColorIndex">Index of the new foreground color [0-1]</param>
        /// ************************************************************************************************

        public static void ApplyRecursive(SvgElement svgElement, int newSvgColorIndex)
        {

            foreach (var e in svgElement.Traverse(e => e.Children))
            {
                if (e.FillOpacity != 0.0f && e.StrokeOpacity != 0.0f)
                {
                    if (e.TryGetAttribute("fill", out string value))
                    {
                        if (string.Compare(value.ToUpperInvariant(), "#FFFFFF") == 0 || string.Compare(value.ToUpperInvariant(), "WHITE") == 0)
                        {
                            e.Fill = foregroundColor[newSvgColorIndex];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="childrenSelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T root, Func<T, IEnumerable<T>> childrenSelector)
            => Enumerable.Repeat(root, 1).Traverse(childrenSelector);



        /// ************************************************************************************************
        /// <summary>
        /// Convert legacy Image to BitmapImage
        /// </summary>
        /// <param name="image">Image object</param>
        /// <returns></returns>
        /// ************************************************************************************************
        private static BitmapImage ConvertImageToBitmapImage(Image image)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Builds string array with all additional destination markers framed with spaces.
        /// It's needed to find them rightly.
        /// </summary>
        /// <returns>Output string array</returns>
        /// ************************************************************************************************
        private static string[] BuildSpacedUSMarkersArray()
        {
            List<string> outputList = new List<string>();

            foreach (string s in USLineMarkers)
            {
                outputList.Add(" " + s + " ");
            }

            foreach (string s in ULineMarkers)
            {
                outputList.Add(" " + s + " ");
            }

            foreach (string s in SLineMarkers)
            {
                outputList.Add(" " + s + " ");
            }

            return outputList.ToArray();
        }
    }
}