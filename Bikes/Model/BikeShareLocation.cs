using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeShareLocation
    {
        [JsonProperty("city")]
        public string City
        {
            get;
            set;
        }

        [JsonProperty("country")]
        public string Country
        {
            get;
            set;
        }

        [JsonProperty("latitude")]
        public double Latitude
        {
            get;
            set;
        }

        [JsonProperty("longitude")]
        public double Longitude
        {
            get;
            set;
        }
    }
}
