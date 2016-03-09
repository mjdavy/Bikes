using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeStationExtra
    {
        [JsonProperty("locked")]
        public bool Locked
        {
            get;
            set;
        }

        [JsonProperty("installed")]
        public bool Installed
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

        [JsonProperty("uid")]
        public int Id
        {
            get;
            set;
        }
    }
}
