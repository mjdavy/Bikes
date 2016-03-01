using Newtonsoft.Json.Linq;
using System;

namespace Bikes.Model
{
    public class CityBikesParser : IStationDataParser
    {
        public StationData Parse(string input)
        {
            StationData data = new StationData();
            JArray jStations = JArray.Parse(input);

            foreach (var jStation in jStations)
            {
                int id = (int)jStation["id"];
                int bikes = this.GetIntValueFromToken(jStation["bikes"]);
                int free = this.GetIntValueFromToken(jStation["free"]);
                int lat = (int)jStation["lat"];
                int lng = (int)jStation["lng"];
                string name = (string)jStation["name"];

                double decLat = (double)lat / 1000000.0;
                double declong = (double)lng / 1000000.0;

                var station = Station.Create(id, name, bikes, free, true, false, new Windows.Devices.Geolocation.BasicGeoposition { Latitude = decLat, Longitude = declong });
                data.Stations.Add(station);
            }

            data.LastUpdated = "Last Updated: " +  DateTime.Now.ToString();
            return data;
        }

        private int GetIntValueFromToken(JToken token)
        {
            int value = 0;
            if (token.Type == JTokenType.Integer)
            {
                value = (int)token;
            }
            else
            {
                value = int.Parse((string)token);
            }
            return value;
        }

        
    }
}
