using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.Devices.Geolocation;
using Bikes.Model;

namespace Bikes.ViewModel
{
    public class StationViewModel : ObservableObject
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
        private int distance;
 

        private StationViewModel()
        {
        }

        public static StationViewModel Create(BikeShareStation bikeShareStation)
        {
            var vm = new StationViewModel();

            vm.Id = bikeShareStation.Id;
            vm.name = bikeShareStation.Extra.Name;
            vm.bikeCount = bikeShareStation.FreeBikes;
            vm.emptyDockCount = bikeShareStation.EmptySlots;
            vm.Locked = bikeShareStation.Extra.Locked;
            vm.Installed = bikeShareStation.Extra.Installed;
            vm.location = new Geopoint(
                new BasicGeoposition { Latitude = bikeShareStation.Latitude, Longitude = bikeShareStation.Longitude });

            vm.CalcPieArc(vm.BikeCount, vm.EmptyDockCount);
            vm.DetermineAvailability();

            return vm;
        }

        public Guid Id
        {
            get;
            set;
        }

        public bool Locked
        {
            get;
            set;
        }

        public bool Installed
        {
            get;
            set;
        }

        public void Update(StationViewModel other)
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

            if (this.Locked != other.Locked)
            {
                this.Locked = other.Locked;
                recalcPie = true;
            }

            if (this.Installed != other.Installed)
            {
                this.Installed = other.Installed;
                recalcPie = true;
            }

            if (recalcPie)
            {
                this.CalcPieArc(bikeCount, emptyDockCount);
                this.DetermineAvailability();
            }
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

            if (this.Locked)
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
            this.NoBikesOrDocks = (this.Locked == true || this.Installed == false || (this.BikeCount == 0 && this.EmptyDockCount == 0));
        }
    }
}
