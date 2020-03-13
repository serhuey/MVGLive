// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Windows.Media;

namespace MVGAPI
{
    public class PreparedDeparture
    {
        public string Product { get; set; }
        public string Label { get; set; }
        public string Destination { get; set; }
        public string DepartureTime { get; set; }
        public string MinutesToDeparture { get; set; }
        public string Delay { get; set; }
        public string LineBackgroundColor { get; set; }
        public bool Sev { get; set; }
        public string FontSize { get; set; }
        public string Platform { get; set; }
        public string Station { get; set; }
        public FontFamily FontFamily { get; set; }
    }
}