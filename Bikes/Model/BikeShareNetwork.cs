using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeShareNetwork
    {
        [JsonProperty("href")]
        public string HRef
        {
            get;
            set;
        }

        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("location")]
        public BikeShareLocation Location
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("stations")]
        public BikeShareStations Stations
        {
            get;
            set;
        }
    }
}
