using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.Devices.Geolocation;

namespace Bikes.Model
{
    public class Station : ObservableObject, IEquatable<Station>
    {
        private const int pieRadius = 10;

        private string name;
        private Geopoint location;
        private int bikeCount;
        private int emptyDockCount;
        private Point pieArc;
        private bool isPieSliceLarge;
        private Visibility detailsVisibility = Visibility.Collapsed;
        private bool noBikesOrDocks;
        private bool installed;
        private bool locked;
        private int distance;

        private Station()
        {
        }

        public static Station Create(int id, string name, int bikeCount, int emptyDockCount, bool installed, bool locked, BasicGeoposition location)
        {
            var station = new Station();

            station.Id = id;
            station.name = name;
            station.bikeCount = bikeCount;
            station.emptyDockCount = emptyDockCount;
            station.locked = locked;
            station.installed = installed;
            station.location = new Geopoint(location);

            station.CalcPieArc(bikeCount, emptyDockCount);
            station.DetermineAvailability();

            return station;
        }

        public void Update(Station other)
        {
            if (this.Id != other.Id)
            {
                Debug.Assert(false,"Station Id Mismatch.");
                return;
            }

            bool recalcPie = false;

            if (this.BikeCount != other.BikeCount)
            {
                this.BikeCount = other.BikeCount;
                recalcPie = true;
            }

            if (this.EmptyDockCount != other.EmptyDockCount)
            {
                this.EmptyDockCount = other.EmptyDockCount;
                recalcPie = true;
            }

            if (this.Name != other.Name)
            {
                this.Name = other.Name;
            }

            if (!GeoUtil.AreEqual(this.Location.Position, other.Location.Position))
            {
                this.Location = other.Location;
            }

            if (this.locked != other.locked)
            {
                this.locked = other.locked;
                recalcPie = true;
            }

            if (this.installed != other.installed)
            {
                this.installed = other.installed;
                recalcPie = true;
            }

            if (recalcPie)
            {
                this.CalcPieArc(bikeCount, emptyDockCount);
                this.DetermineAvailability();
            }
        }

        public bool Equals(Station other)
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

        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.RaisePropertyChanged("Name");
            }
        }
        
        public Geopoint Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
                this.RaisePropertyChanged("Location");
            }
        }

        public Point Origin
        {
            get
            {
                return new Point(0.5, 1.0);
            }
        }

        public Visibility DetailsVisibility
        {
            get
            {
                return this. detailsVisibility;
            }
            set
            {
                this.detailsVisibility = value;
                this.RaisePropertyChanged("DetailsVisibility");
            }

        }

        public bool NoBikesOrDocks
        {
            get
            {
                return this.noBikesOrDocks;
            }
            set
            {
                this.noBikesOrDocks = value;
                this.RaisePropertyChanged("NoBikesOrDocks");
            }
        }
        
        public int BikeCount
        {
            get
            {
                return this.bikeCount;
            }
            set
            {
                this.bikeCount = value;
                this.RaisePropertyChanged("BikeCount");
                this.RaisePropertyChanged("StationInfo");
            }
        }

        public int EmptyDockCount
        {
            get
            {
                return this.emptyDockCount;
            }
            set
            {
                this.emptyDockCount = value;
                this.RaisePropertyChanged("EmptyDockCount");
                this.RaisePropertyChanged("StationInfo");
            }
        }

        public int Distance
        {
            get
            {
                return this.distance;
            }
            set
            {
                this.distance = value;
            }
        }

        public string DistanceInfo
        {
            get
            {
                return string.Format("Distance: {0} m", this.distance);
            }
        }

        public string StationInfo
        {
            get
            {
                return string.Format("Available Bikes: {0}, Empty Docks: {1}", this.BikeCount, this.EmptyDockCount);
            }
        }

        public Point PieArc
        {
            get
            {
                return this.pieArc;
            }
            set
            {
                this.pieArc = value;
                this.RaisePropertyChanged("PieArc");
            }
        }

        public bool IsPieSliceLarge
        {
            get
            {
                return this.isPieSliceLarge;
            }
            set
            {
                this.isPieSliceLarge = value;
                this.RaisePropertyChanged("IsPieSliceLarge");
            }
        }

        private void CalcPieArc(int bikes, int docks)
        {
            double total = bikes + docks;
            double fullAngle = total > 0 ? (bikes / total) * 360 : 0;

            if (this.locked)
            {
                fullAngle = 0;
            }

            // Hack to prevent 360 looking the same as zero
            if (fullAngle == 360)
            {
                fullAngle = 359.9;
            }

            double x = pieRadius + pieRadius * Math.Cos((90 - fullAngle) * Math.PI / 180);
            double y = pieRadius - pieRadius * Math.Sin((90 - fullAngle) * Math.PI / 180);

            this.IsPieSliceLarge = fullAngle > 180;
            this.PieArc = new Point(x, y);
        }

        private void DetermineAvailability()
        {
            this.NoBikesOrDocks = (this.locked == true || this.installed == false || (this.BikeCount == 0 && this.EmptyDockCount == 0));
        }
    }
}
