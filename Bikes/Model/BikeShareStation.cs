using Newtonsoft.Json;
using System;

namespace Bikes.Model
{
    public class BikeShareStation : IEquatable<BikeShareStation>
    {
        [JsonProperty("empty_slots")]
        public int EmptySlots
        {
            get;
            set;
        }

        [JsonProperty("extra")]
        public BikeStationExtra Extra
        {
            get;
            set;
        }

        [JsonProperty("free_bikes")]
        public int FreeBikes
        {
            get;
            set;
        }

        [JsonProperty("id")]
        public Guid Id
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

        public bool Equals(BikeShareStation other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}