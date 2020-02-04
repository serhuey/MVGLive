// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace MVGAPI
{
    public class Location
    {
        public string type;
        public float latitude;
        public float longitude;
        public string id;
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
