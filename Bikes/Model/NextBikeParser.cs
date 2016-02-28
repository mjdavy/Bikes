using System;
using System.IO;
using System.Xml.Linq;

namespace Bikes.Model
{
    public class NextBikeParser : IStationDataParser
    {
        private readonly char[] bikeCountUnwantedChars = { '+' };

        public StationData Parse(string input)
        {
            var data = new StationData();

            using (StringReader sr = new StringReader(input))
            {
                XElement stationElements = XElement.Load(sr);
                foreach (var stationElement in stationElements.Descendants("city").Descendants("place"))
                {
                    var uid = stationElement.Attribute("uid");
                    var placeName = stationElement.Attribute("name");
                    var lat = stationElement.Attribute("lat");
                    var lng = stationElement.Attribute("lng");
                    var bikes = stationElement.Attribute("bikes");
                    var bikeRacks = stationElement.Attribute("bike_racks");

                    GeoCoordinate location = new GeoCoordinate(double.Parse(lat.Value), double.Parse(lng.Value));
                    int free = bikeRacks == null ? 0 : int.Parse(bikeRacks.Value);
                    int bikeCount = bikes == null ? 0 : this.CountBikes(bikes.Value);
                    int id = int.Parse(uid.Value);

                    var station = Station.Create(id, placeName.Value, bikeCount, free, true, false, location);
                    data.Stations.Add(station);
                }

                data.LastUpdated = "Last Updated: " + DateTime.Now.ToString();
            }

            return data;
        }

        private int CountBikes(string inputString)
        {
            var bikeCountString = inputString.Trim(bikeCountUnwantedChars);
            return int.Parse(bikeCountString);
        }
    }
}
