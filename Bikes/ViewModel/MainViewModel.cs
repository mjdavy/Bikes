
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
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Linq;

namespace Bikes.ViewModel
{
    public class MainViewModel : ObservableObject, IDisposable
    {
        private float mapZoomLevel = 15.0f;
        private string currentStatus = string.Empty;
        private Geolocator geolocator = null;
        private StationLoader stationLoader = new StationLoader();
        private BasicGeoposition myLocation;
        private BasicGeoposition mapCenter;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool disposed = false;
        private bool isUpdating;
        private Visibility isMyLocationVisible;
        private ObservableCollection<Station> _stationSource;
        private Station _currentStation;

        public MainViewModel()
        {
            this.InitializeCommands();
            this.StationSource = new ObservableCollection<Station>();
            this.timer.Interval = TimeSpan.FromMinutes(1.0);
            this.timer.Tick += Timer_Tick; 
            this.isMyLocationVisible = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, object e)
        {
            this.LoadStationDataAsync();
        }

        ~MainViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #region public properties
       

        public ObservableCollection<Station> StationSource
        {
            get
            {
                return _stationSource;
            }
            set
            {
                Set(() => StationSource, ref _stationSource, value);
            }
        }

        public Station CurrentStation
        {
            get
            {
                return _currentStation;
            }
            set
            {
                Set(() => CurrentStation, ref _currentStation, value);
            }
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

        public BasicGeoposition MyLocation
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

        public BasicGeoposition MapCenter
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
            var position = await Cities.FindMyLocationAsync();
            this.MyLocation = new BasicGeoposition { Latitude = position.Coordinate.Latitude, Longitude = position.Coordinate.Longitude };
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
            Station oldStation = this.CurrentStation;
            
            if (oldStation != null)
            {
                oldStation.DetailsVisibility = Visibility.Collapsed;
            }

            if (station != null)
            {
                // stupid hack to fix zorder
                this.StationSource.Remove(station);
                this.StationSource.Add(station);

                station.DetailsVisibility = Visibility.Visible;
            }

            this.CurrentStation = station;
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

            foreach (var station in this.StationSource)
            {
                double dist = GeoUtil.DistanceTo(myLocation,station.Location);
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
                        var index = this.StationSource.IndexOf(station);
                        if (index == -1)
                        {
                            this.StationSource.Add(station);
                        }
                        else
                        {
                            var item = this.StationSource[index];
                            item.Update(station);
                        }
                    }

                    this.SelectStation(selected);
                    this.SetLoadingStatus(false, data.LastUpdated);
                });
        }

        private void TimerTick(object sender, EventArgs e)
        {
           
        }

        private void SetLoadingStatus(bool status, string message)
        {
            this.CurrentStatus = message;
            this.IsUpdating = status;
        }

        public void UpdateDistances()
        {
            foreach (var station in this.StationSource)
            {
                station.Distance = (int)(GeoUtil.DistanceTo(station.Location, this.myLocation) * 1000.0);
            }

            this.StationSource.OrderBy(x => x.Distance);
        }

        private bool StationSourceFilter(Station station)
        {
            if (station == null)
            {
                return false;
            }

            var myCity = Cities.CurrentCity;
            if (myCity == null)
            {
                return false;
            }

            if (GeoUtil.DistanceTo(station.Location, myCity.Center) < 50000)
            {
                return true;
            }

            return false;

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
                this.MyLocation = new BasicGeoposition { Latitude = args.Position.Coordinate.Latitude, Longitude = args.Position.Coordinate.Longitude };
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
                this.StationSource.Clear();
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
