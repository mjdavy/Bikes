using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Bikes.Model
{
    public class GeoUtil
    {
        public enum MapUnits
        {
            Killometers,
            Miles,
            NauticalMiles
        };

        static public double DistanceTo(BasicGeoposition pos1, BasicGeoposition pos2, MapUnits unit = MapUnits.Killometers)
        {
            double rlat1 = Math.PI * pos1.Latitude / 180;
            double rlat2 = Math.PI * pos2.Latitude / 180;
            double theta = pos1.Longitude - pos2.Longitude;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case MapUnits.Killometers: 
                    return dist * 1.609344;
                case MapUnits.NauticalMiles: 
                    return dist * 0.8684;
                case MapUnits.Miles:
                    return dist;
            }

            return dist;
        }

        static public bool AreEqual(BasicGeoposition pos1, BasicGeoposition pos2)
        {
            return (pos1.Latitude == pos2.Latitude && pos1.Longitude == pos2.Longitude);
        }
    }
}
