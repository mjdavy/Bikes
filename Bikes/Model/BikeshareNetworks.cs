using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class BikeShareNetworks
    {
        [JsonProperty("networks")]
        public IList<BikeShareNetwork> Networks
        {
            get;
            set;
        }
    }
}
