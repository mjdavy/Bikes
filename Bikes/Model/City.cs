using System;
using Windows.Devices.Geolocation;

namespace Bikes.Model
{
    public class City
    {
      
        public string Name
        {
            get;
            set;
        }

        public Country Country
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public BasicGeoposition Center
        {
            get;
            set;
        }
    }
}
