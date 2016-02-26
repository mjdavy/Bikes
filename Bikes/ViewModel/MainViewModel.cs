
using Bikes.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Net.NetworkInformation;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;

namespace Bikes.ViewModel
{
    public class MainViewModel : ObservableObject, IDisposable
    {
        private float mapZoomLevel = 15.0f;
        private string currentStatus = string.Empty;
        private Geolocator geolocator = null;

        private StationLoader stationLoader = new StationLoader();
        private CollectionViewSource stationSource = new CollectionViewSource();
        private GeoCoordinate myLocation;
        private GeoCoordinate mapCenter = GeoCoordinate.Unknown;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool disposed = false;
        private bool isUpdating;
        private Visibility isMyLocationVisible;

        public MainViewModel()
        {
            this.InitializeCommands();
            this.StationCollection = new ObservableCollection<Station>();
            this.StationSource.Source = this.StationCollection;
            this.timer.Interval = TimeSpan.FromMinutes(1.0);
            this.timer.Tick += new EventHandler(TimerTick);
            this.StationSource.Filter += new FilterEventHandler(StationSource_Filter);
            this.isMyLocationVisible = Visibility.Collapsed;
        }

        ~MainViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #region public properties
        public CollectionViewSource StationSource
        {
            get
            {
                return this.stationSource;
            }
            set
            {
                this.stationSource = value;
                this.RaisePropertyChanged("StationSource");
            }
        }

        public ObservableCollection<Station> StationCollection
        {
            get;
            set;
        }

        public MapStyle MapMode
        {
            get
            {
                return AppSettings.Instance.MapMode;
            }
        }

        public string CurrentStatus
        {
            get
            {
                return this.currentStatus;
            }
            set
            {
                this.currentStatus = value;
                this.RaisePropertyChanged("CurrentStatus");
            }
        }

        public bool IsUpdating
        {
            get
            {
                return this.isUpdating;
            }
            set
            {
                this.isUpdating = value;
                this.RaisePropertyChanged("IsUpdating");
            }
        }

        public float MapZoomLevel
        {
            get
            {
                return this.mapZoomLevel;
            }
            set
            {
                this.mapZoomLevel = value;
                this.RaisePropertyChanged("MapZoomLevel");
            }
        }

        public GeoCoordinate MyLocation
        {
            get
            {
                return this.myLocation;
            }
            set
            {
                this.myLocation = value;
                this.RaisePropertyChanged("MyLocation");
            }
        }

        public Visibility IsMyLocationVisible
        {
            get
            {
                return this.isMyLocationVisible;
            }
            set
            {
                this.isMyLocationVisible = value;
                this.RaisePropertyChanged("IsMyLocationVisible");
            }
        }

        public GeoCoordinate MapCenter
        {
            get
            {
                return this.mapCenter;
            }
            set
            {
                this.mapCenter = value;
                this.RaisePropertyChanged("MapCenter");
            }
        }

        #endregion

        // First time start up
        public async void Start()
        {
            this.InitializeGeoLocator();
            await Cities.InitializeAsync();
            this.MyLocation = await Cities.FindMyLocationAsync();
            this.CenterMapToMyLocation();
            this.LoadStationDataAsync();
        }

        public async void LoadStationDataAsync()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                this.timer.Stop();
                this.SetLoadingStatus(true, "Updating Bike Stations.");
                var stationData = await this.stationLoader.LoadDataAsync(Cities.CurrentCity);
                this.UpdateStations(stationData);
                this.timer.Start();
            }
            else 
            {
                this.SetLoadingStatus(false, "No Network Available.");
            }
           
        }

        #region commands

        public ICommand FindNearestBikeCommand
        {
            get;
            set;
        }

        public ICommand FindNearestDockCommand
        {
            get;
            set;
        }

        public ICommand FindMeCommand
        {
            get;
            set;
        }

        private void InitializeCommands()
        {
            this.FindMeCommand = new RelayCommand(this.CenterMapToMyLocation);
            this.FindNearestBikeCommand = new RelayCommand(this.SelectNearestStationWithAvailableBike);
            this.FindNearestDockCommand = new RelayCommand(this.SelectNearestStationWithAvailableDock);
        }

        #endregion

        public void SelectStation(Station station)
        {
            Station oldStation = this.StationSource.View.CurrentItem as Station;
            
            if (oldStation != null)
            {
                oldStation.DetailsVisibility = Visibility.Collapsed;
            }

            if (station != null)
            {
                // stupid hack to fix zorder
                this.StationCollection.Remove(station);
                this.StationCollection.Add(station);

                station.DetailsVisibility = Visibility.Visible;
            }

            this.StationSource.View.MoveCurrentTo(station);
        }

        private void CenterMapToMyLocation()
        {
             this.MapCenter = this.MyLocation;
             this.IsMyLocationVisible = Visibility.Visible;
        }

        private Station SelectedStation
        {
            get
            {
                return this.StationSource.View.CurrentItem as Station;
            }
        }

        private void SelectNearestStationWithAvailableBike()
        {
            this.SelectNearestStationWithAvailablilty((Station s) => s.BikeCount > 0);
        }

        private void SelectNearestStationWithAvailableDock()
        {
            this.SelectNearestStationWithAvailablilty((Station s) => s.EmptyDockCount > 0);
        }

        private void SelectNearestStationWithAvailablilty(Predicate<Station> condition)
        {
            double minDist = Double.MaxValue;
            Station nearestStation = null;

            foreach (var station in this.StationCollection)
            {
                double dist = myLocation.GetDistanceTo(station.Location);
                if (dist < minDist && condition(station))
                {
                    nearestStation = station;
                    minDist = dist;
                }
            }

            this.SelectStation(nearestStation);

            if (nearestStation != null)
            {
                this.MapCenter = nearestStation.Location;
            }
        }

        private void UpdateStations(StationData data)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Station selected = this.SelectedStation;

                    foreach (var station in data.Stations)
                    {
                        var index = this.StationCollection.IndexOf(station);
                        if (index == -1)
                        {
                            this.StationCollection.Add(station);
                        }
                        else
                        {
                            var item = this.StationCollection[index];
                            item.Update(station);
                        }
                    }

                    this.SelectStation(selected);
                    this.SetLoadingStatus(false, data.LastUpdated);
                });
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.LoadStationDataAsync();
        }

        private void SetLoadingStatus(bool status, string message)
        {
            this.CurrentStatus = message;
            this.IsUpdating = status;
        }

        public void UpdateDistances()
        {
            this.stationSource.SortDescriptions.Clear();
            
            foreach (var station in this.StationCollection)
            {
                station.Distance = (int)station.Location.GetDistanceTo(this.myLocation);
            }
            this.Sort();
            
        }

        private void Sort()
        {
            this.StationSource.SortDescriptions.Clear();
            this.StationSource.SortDescriptions.Add(new SortDescription("Distance", ListSortDirection.Ascending));
        }

        private void StationSource_Filter(object sender, FilterEventArgs e)
        {
            var station = e.Item as Station;
            if (station == null)
            {
                return;
            }

            var myCity = Cities.CurrentCity;
            if (myCity == null)
            {
                return;
            }

            if (station.Location.GetDistanceTo(myCity.Center) < 50000)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }

        }

        private void InitializeGeoLocator()
        {
            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.MovementThreshold = 20; // The units are meters.

            geolocator.StatusChanged += geolocator_StatusChanged;
            geolocator.PositionChanged += geolocator_PositionChanged;
        }

        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.MyLocation = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
            });
            
        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "location unkown";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location service is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "Finding your location...";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "Unable to find your location";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    break;
                case PositionStatus.NotInitialized:
                    status = "location tracking stopped";
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state
                    break;
            }

            if (args.Status != PositionStatus.Ready)
            {
                Messenger.Default.Send<StatusMessage>(new StatusMessage(status));
            }
    
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion

        internal void OnSettingsChanged(SettingsChangedMessage msg)
        {
            if (msg.ChangedSetting == AppSettings.CurrentCitySetting)
            {
                this.StationCollection.Clear();
            }
            else if (msg.ChangedSetting == AppSettings.MapModeSetting)
            {
                this.RaisePropertyChanged("MapMode");
            }
        }

        internal void OnStatusChanged(StatusMessage msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.CurrentStatus = msg.Status;
                });
        }
    }
}
