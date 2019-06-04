using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVGAPI
{
    public class Location
    {
        public string type;
        public float latitude;
        public float longitude;
        public int id;
        public string place;
        public string name;
        public bool hasLiveData;
        public bool hasZoomData;
        public string[] products;
        public string aliases;
        public string link;
        public Lines lines;
    }
}
