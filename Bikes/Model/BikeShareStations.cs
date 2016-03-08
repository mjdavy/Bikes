using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeShareStations
    {
        [JsonProperty("stations")]
        public IList<BikeShareStation> Stations
        {
            get;
            set;
        }
    }
}
