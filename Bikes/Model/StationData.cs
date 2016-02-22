using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
