using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeShareNetworkInfo
    {
        [JsonProperty("network")]
        public BikeShareNetwork Network
        {
            get;
            set;
        }
    }
}
