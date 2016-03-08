using Newtonsoft.Json;

namespace Bikes.Model
{
    public class BikeShareStation
    {
        [JsonProperty("empty_slots")]
        public int EmptySlots
        {
            get;
            set;
        }
    }
}