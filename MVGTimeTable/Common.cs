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
    }
    static class Common
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
                                                    { MessageType.Warning, "WARNING"  }};

        public static readonly Dictionary<MessageType, string> Messages =
            new Dictionary<MessageType, string> {   { MessageType.NoConnection, "www.mvg.de - Verbindung fehlgeschlagen"},
                                                    { MessageType.Warning, "Die Daten sind möglicherweise nicht relevant"}};

        public static readonly string[] UBahnIconKey = { "U", "U1", "U2", "U3", "U4", "U5", "U6", "U7", "U8" };
        public static readonly string[] SBahnIconKey = { "S", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S",
                                                         "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",  "S",
                                                         "S20" };
        public static readonly string[] TramIconKey = { "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM", "TRAM",
                                                        "TRAM", "TRAM", "TRAM12", "TRAM", "TRAM", "TRAM15", "TRAM16", "TRAM17", "TRAM18", "TRAM19",
                                                         "TRAM20", "TRAM21", "TRAM", "TRAM23", "TRAM", "TRAM25", "TRAM", "TRAM27", "TRAM28", "TRAM29"};
        public static readonly string[] GleisIconKey = { "Haltestelle", "Gleis1", "Gleis2", "Gleis3", "Gleis4", "Gleis5", "Gleis6", "Gleis7", "Gleis8", "Gleis9",
                                                         "Gleis10", "Gleis11", "Gleis12", "Gleis13", "Gleis14", "Gleis15", "Gleis16" };
        public static readonly string[] SGleisIconKey = { "Haltestelle", "SGleis1", "SGleis2", "SGleis3", "SGleis4", "SGleis5", "SGleis6", "SGleis7", "SGleis8", "SGleis9",
                                                         "SGleis10", "SGleis11", "SGleis12", "SGleis13", "SGleis14", "SGleis15", "SGleis16" };

        public const string DefaultUBahnIconKey = "UBahn";
        public const string DefaultSBahnIconKey = "SBahn";
        public const string NSevBusIconKey = "NSevBus";
        public const string SevBusIconKey = "SevBus";
        public const string NBusIconKey = "NBus";
        public const string ExpressBusIconKey = "ExpressBus";
        public const string BusIconKey = "Bus";
        public const string DefaultTramIconKey = "Tram";
        public const string NTramIconKey = "NTram";
        public const string WarningIconKey = "Warning";
        public const string NoConnectionIconKey = "NoConnection";
        public const string NowIconKey = "jetzt";
        public const string FussballIconKey = "Fussball";
        public const string ZooIconKey = "Zoo";
        public const string MesseIconKey = "Messe";
        public const string OlympiaIconKey = "Olympia";
        public const string UBahnMonoFullIconKey = "UBahn_100";
        public const string UBahnMonoHalfIconKey = "UBahn_50";
        public const string SBahnMonoFullIconKey = "SBahn_100";
        public const string SBahnMonoHalfIconKey = "SBahn_50";
        public const string USBahnsMonoFullIconKey = "USBahns_100";
        public const string USBahnsMonoHalfIconKey = "USBahns_50";
        public const string TrainFirstHalfIconKey = "FirstS1";
        public const string TrainSecondHalfIconKey = "LastS1";

        public static readonly Dictionary<string, string> AirportIconKeys = new Dictionary<string, string> { { "S1", "S1FH" }, { "S8", "S8FH" } };

        public static readonly string[] AirportMarkers = { "FLUGHAFEN" };
        public static readonly string[] BusMarkers = { "BUS" };
        public static readonly string[] TramMarkers = { "TRAM" };
        public static readonly string[] UBahnMarkers = { "UBAHN" };
        public static readonly string[] SBahnMarkers = { "SBAHN" };
        public static readonly string[] AdditionalDestinationMarkers = { "VIA", "Ü.", "WEITER" };
        public static readonly string[] NightLineMarkers = { "N" };
        public static readonly string[] ExpressLineMarkers = { "X" };

        public static readonly string[] ULineMarkers = { "U", "(U)" };
        public static readonly string[] SLineMarkers = { "S", "(S)" };
        public static readonly string[] USLineMarkers = { "SU", "US", "(U)(S)", "(S)(U)", "(U) (S)", "(S) (U)" };

        public static readonly string[] USSpacedMarkers = BuildSpacedUSMarkersArray();

        // These colors are not using now due to memory overusing during the binding, now they are hardcoded in xaml
        public const string PrimaryForegroundColor = "#FFE8E8E8";
        public const string SecondaryForegroundColor = "#55F1F1F1";
        public const string PrimaryBackgroundColor = "#FF262A74";
        public const string SecondaryBackgroundColor = "#FF00056C";

        public const string NoConnectionForegroundColor = "#FFFF4E48";
        public const string WarningForegroundColor = "#FFFFEB85";

        public const double DelayFontSizeCoeff = 0.55;
        public const int UndefinedSignThreshold = -3;

        public const string TimeSignSeparator = "  ";
        public const string MinutesSign = "Min.";
        public const string HoursSign = "Std.";
        public const string UndefinedTimeSign = "  ????";

        public const string TrainFirstHalf = "{-}{V}";
        public const string TrainSecondHalf = "{V}{-}";

        public static Dictionary<string, BitmapImage> icons;

        /// <summary>
        /// Create Dictionary with Bitmaps of the same height from SVG sources
        /// </summary>
        /// <param name="icons">Dictionary with icons</param>
        /// <param name="fontSize">Height of icons</param>
        /// <returns></returns>
        public static bool CreateIconsDictionaryFromSVG(out Dictionary<string, BitmapImage> icons, double fontSize)
        {
            bool result = true;

            icons = new Dictionary<string, BitmapImage>();

            string[] svgNames = GetResourcesNamesFromFolder("SVGIcons");

            foreach (string svgName in svgNames)
            {
                try
                {
                    BitmapImage bi = GetBitmapFromSVG(new Uri(SvgPath + svgName), fontSize);
                    string key = Path.GetFileNameWithoutExtension(svgName).ToUpperInvariant();
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

        /// <summary>
        /// Get all names of embedded resources in project folder
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <returns>Araay of the strings with file names</returns>
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

        /// <summary>
        /// Get Bitmap with defined height from SVG-file with defined uri
        /// </summary>
        /// <param name="uri">Uri of the SVG-file</param>
        /// <param name="height">Desired height of the BitmapImage</param>
        /// <returns></returns>
        private static BitmapImage GetBitmapFromSVG(Uri uri, double height)
        {
            SvgDocument svgDocument;
            XmlDocument xmlDocument = new XmlDocument();

            using (Stream inputStream = Application.GetResourceStream(uri).Stream)
            {
                xmlDocument.Load(inputStream);
            }

            svgDocument = SvgDocument.Open(xmlDocument);
            Bitmap bitmap = svgDocument.Draw(0, (int)height);
            BitmapImage bitmapImage = ConvertImageToBitmapImage(bitmap);

            return bitmapImage;
        }


        /// <summary>
        /// Convert legacy Image to BitmapImage
        /// </summary>
        /// <param name="image">Image object</param>
        /// <returns></returns>
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


        private static string[] BuildSpacedUSMarkersArray()
        {
            List<string> outputList = new List<string>();

            foreach(string s in USLineMarkers)
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
