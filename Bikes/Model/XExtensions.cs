using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace Bikes.Model
{
    public static class XExtensions
    {
        public static int ToInt(this XElement xe, int emptyValue)
        {
            return xe == null ? emptyValue : int.Parse(xe.Value);
        }

        public static bool ToBool(this XElement xe, bool emptyValue)
        {
            return xe == null ? emptyValue : bool.Parse(xe.Value);
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBixiGeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("lat").Value);
            double longitude = double.Parse(xe.Element("long").Value);
            return xe == null ? emptyValue :
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBCycleGeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("Latitude").Value);
            double longitude = double.Parse(xe.Element("Longitude").Value);
            return xe == null ? emptyValue : 
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }

        // MJDTODO - Refactor  DRY
        public static BasicGeoposition ToBixi2GeoCoordinate(this XElement xe, BasicGeoposition emptyValue)
        {
            double latitude = double.Parse(xe.Element("latitude").Value);
            double longitude = double.Parse(xe.Element("longitude").Value);
            return xe == null ? emptyValue : 
                new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
        }
    }
}
