﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVGAPI
{
    public class PreparedDeparture
    {
        public string product { get; set; }
        public string label { get; set; }
        public string destination { get; set; }
        public string departureTime { get; set; }
        public string minutesToDeparture { get; set; }
        public string lineBackgroundColor { get; set; }
        public bool sev { get; set; }
        public string fontSize { get; set; }
    }
}
