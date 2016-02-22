using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;

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

        public GeoCoordinate Center
        {
            get;
            set;
        }
    }
}
