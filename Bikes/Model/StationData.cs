using System.Collections.Generic;

namespace Bikes.Model
{
    public class StationData 
    {
        public StationData()
        {
            this.Stations = new List<Station>();
            this.Initialize();
        }

        public string LastUpdated
        {
            get;
            set;
        }

        public IList<Station> Stations
        {
            get;
            set;
        }

        public void Initialize()
        {
            this.Stations.Clear();
            this.LastUpdated = string.Empty;
        }

        public void Error(string message)
        {
            this.Stations.Clear();
            this.LastUpdated = message;
        }
    }
}
