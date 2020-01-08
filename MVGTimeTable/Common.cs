using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media.Imaging;

namespace MVGTimeTable
{
    static class Common
    {
        public static string ImagePath => "pack://application:,,,/MVGTimeTable;component/Icons/";
        public static string SVGPath => "pack://application:,,,/MVGTimeTable;component/SVGIcons/";
        public static readonly List<string> multiplatformSbahnStations = new List<string>(new string[] { "OSTBAHNHOF", "PASING", "LAIM", "HAUPTBAHNHOF" });
        public static readonly List<string> multiplatformTramStations = new List<string>(new string[] { "KARLSPLATZ" });

        public static readonly string[] digits = { "0", "❶", "❷", "❸", "❹", "❺", "❻", "❼", "❽", "❾", "❿", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30" };
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
                                                                        "Thalkirchen" } } };

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
                                                                        "Moosach"
                                                } } };
        public static Dictionary<string, BitmapImage> icons;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_icons"></param>
        /// <param name="fontSize"></param>
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
        /// 
        /// </summary>
        /// <param name="_icons"></param>
        /// <param name="fontSize"></param>
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
        public static string[] GetResourcesNamesFromFolder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            var resourceReader = new ResourceReader(stream);

            var resources =
                from p in resourceReader.OfType<DictionaryEntry>()
                let theme = (string)p.Key
                where theme.StartsWith(folder)
                select theme.Substring(folder.Length);

            return resources.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmapFromSVG(Uri uri, double height)
        {
            BitmapImage bi = new BitmapImage();
            return bi;
        }

        /// <summary>
        /// 
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
