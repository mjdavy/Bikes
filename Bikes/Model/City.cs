using System;
using Windows.Devices.Geolocation;

namespace Bikes.Model
{
    public class City
    {
        public enum VendorType
        {
            CityBikes,
            NextBike
        };

        public VendorType Vendor
        {
            get;
            set;
        }

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

        public bool UseCacheHack
        {
            get;
            set;
        }

        public Uri DataSource
        {
            get
            {
                if (this.UseCacheHack)
                {
                    return new Uri(string.Format("{0}?nocache={1}", Url, DateTime.Now.Ticks));
                }

                return new Uri(Url);
            }
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
