// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media.Imaging;

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
        public const int UtmostColumn = 1;     //index of column that can be truncated
        public const string ImagePath = "pack://application:,,,/MVGTimeTable;component/Icons/";
        public const string SVGPath = "pack://application:,,,/MVGTimeTable;component/SVGIcons/";
        public static readonly List<string> MultiplatformSbahnStations = new List<string>(new string[] { "OSTBAHNHOF", "PASING", "LAIM", "HAUPTBAHNHOF" });
        public static readonly List<string> MultiplatformTramStations = new List<string>(new string[] { "KARLSPLATZ" });

        public static readonly string[] PlatformSign = { "0", "❶", "❷", "❸", "❹", "❺", "❻", "❼", "❽", "❾", "❿", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30" };
        public static readonly Dictionary<string, string[]> FussballDestinations =
            new Dictionary<string, string[]> { { "U6", new string[] {   "Fröttmaning",
                                                                        "Garching-Forschungszentrum",
                                                                        "Garching, Forschungszentrum",
                                                                        "Garching",
                                                                        "Garching, Hochbrück",
                                                                        "Garching-Hochbrück" } } };
        public static readonly Dictionary<string, string[]> ZooDestinations =
            new Dictionary<string, string[]> { { "U3", new string[] {   "Fürstenried West",
                                                                        "Basler Straße",
                                                                        "Forstenrieder Allee",
                                                                        "Machtlfinger Straße",
                                                                        "Aidenbach-straße",
                                                                        "Obersendling",
                                                                        "Thalkirchen" } },
                                                { "135", new string[] { "Thalkirchen (Tierpark)", "Thalkirchen", "Thalkirchen(Tierpark)", "Thalkirchen (Tierpark) U" } },
                                                {  "52", new string[] { "Tierpark (Alemannenstraße)", "Tierpark (Alemannenstr.)", "Tierpark" } },
                                                { "X98", new string[] { "Tierpark (Alemannenstraße)", "Tierpark (Alemannenstr.)", "Tierpark" } } };

        public static readonly Dictionary<string, string[]> MesseDestinations =
            new Dictionary<string, string[]> { { "U2", new string[] {   "Messestadt Ost",
                                                                        "Messestadt-Ost"} } };

        public static readonly Dictionary<string, string[]> OlympiaDestinations =
            new Dictionary<string, string[]> {  { "U8", new string[] {  "Olympiazentrum",
                                                                        "Olympia-zentrum" } },
                                                { "U3", new string[] {  "Olympiazentrum",
                                                                        "Olympia-zentrum",
                                                                        "Oberwiesen-feld",
                                                                        "Oberwiesenfeld",
                                                                        "Olympia-Einkaufszentrum",
                                                                        "Olympiaeinkaufszentrum",
                                                                        "Moosacher St.-Martins-Platz",
                                                                        "Moosacher St.-Martins-Pl.",
                                                                        "Moosach" } } };

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

        public const string DefaultUBahnIconKey = "UBahn";
        public const string DefaultSBahnIconKey = "SBahn";
        public const string NSevBusIconKey = "NSevBus";
        public const string SevBusIconKey = "SevBus";
        public const string NBusIconKey = "NBus";
        public const string ExpressBusIconKey = "ExpressBus";
        public const string BusIconKey = "Bus";
        public const string TramIconKey = "Tram";
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

        public static readonly Dictionary<string, string> AirportIconKeys = new Dictionary<string, string> { { "S1", "S1FH" }, { "S8", "S8FH" } };

        public const string AirportMarker = "FLUGHAFEN";
        public const string BusMarker = "BUS";
        public const string TramMarker = "TRAM";
        public const string UBahnMarker = "UBAHN";
        public const string SBahnMarker = "SBAHN";
        public const string AdditionalDestinationMarker = "VIA";
        public const string NightLineMarker = "N";
        public const string ExpressLineMarker = "X";

        public const string DefaultForegroundColor = "#FFE8E8E8";
        public const string NoConnectionForegroundColor = "#FFFF4E48";
        public const string WarningForegroundColor = "#FFFFEB85";

        public const double DelayFontSizeCoeff = 0.55;
        public const int UndefinedSignThreshold = -3;

        public const string TimeSignSeparator = "  ";
        public const string MinutesSign = "Min.";
        public const string HoursSign = "Std.";
        public const string UndefinedTimeSign = "  ???";

        public static Dictionary<string, BitmapImage> icons;

        /// <summary>
        /// Create Dictionary with Bitmaps of the same height from SVG sources
        /// </summary>
        /// <param name="_icons">Dictionary with icons</param>
        /// <param name="fontSize">Height of icons</param>
        /// <returns></returns>
        public static bool CreateIconsDictionaryFromSVG(out Dictionary<string, BitmapImage> _icons, double fontSize)
        {
            bool result = true;

            _icons = new Dictionary<string, BitmapImage>();

            string[] svgNames = Common.GetResourcesNamesFromFolder("SVGIcons");

            foreach (string svgName in svgNames)
            {
                try
                {
                    BitmapImage bi = Common.GetBitmapFromSVG(new Uri(Common.SVGPath + svgName), fontSize);
                    string key = System.IO.Path.GetFileNameWithoutExtension(svgName);
                    _icons.Add(key, bi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Create Dictionary with Bitmaps of the same height from PNG sources
        /// </summary>
        /// <param name="_icons">Dictionary with icons</param>
        /// <param name="fontSize">Height of icons</param>
        /// <returns></returns>
        public static bool CreateIconsDictionaryFromPNG(out Dictionary<string, BitmapImage> _icons, double fontSize)
        {
            bool result = true;

            _icons = new Dictionary<string, BitmapImage>();

            string[] pngNames = Common.GetResourcesNamesFromFolder("Icons");

            foreach (string pngName in pngNames)
            {
                try
                {
                    BitmapImage bi = Common.GetBitmapFromPNG(new Uri(Common.ImagePath + pngName), fontSize);
                    string key = System.IO.Path.GetFileNameWithoutExtension(pngName);
                    _icons.Add(key, bi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public static string[] GetResourcesNamesFromFolder(string folder)
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
        /// <param name="uri"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetBitmapFromSVG")]
        public static BitmapImage GetBitmapFromSVG(Uri uri, double height)
        {
            throw new NotImplementedException("GetBitmapFromSVG is not implemented now");
        }

        /// <summary>
        /// Get Bitmap with defined height from PNG-file with defined uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmapFromPNG(Uri uri, double height)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = uri;
            bi.DecodePixelHeight = (int)height;
            bi.EndInit();

            return bi;
        }
    }
}
