using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class CityBikesNetwork
    {
        public string city { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int radius { get; set; }
        public int lat { get; set; }
        public int lng { get; set; }
        public int id { get; set; }
    }
}
