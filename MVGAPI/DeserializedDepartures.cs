using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVGAPI
{
    public class DeserializedDepartures
    {
        public long departureTime { get; set; }
        public string product { get; set; }
        public string label { get; set; }
        public string destination { get; set; }
        public bool live { get; set; }
        public string lineBackgroundColor { get; set; }
        public long departureId { get; set; }
        public bool sev { get; set; }
    }
}
