﻿
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
using System.Threading.Tasks;

namespace Bikes.ViewModel
{
    public class MainViewModel : ObservableObject, IDisposable
    {
        private float mapZoomLevel = 15.0f;
        private string currentStatus = string.Empty;
        private Geolocator geolocator = null;
        private StationLoader stationLoader = new StationLoader();
        private Geopoint myLocation;
        private Geopoint mapCenter;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool disposed = false;
        private bool isUpdating;
        private Visibility isMyLocationVisible;
        private ObservableCollection<StationViewModel> _stationSource;
        private StationViewModel _currentStation;

        public MainViewModel()
        {
            this.InitializeCommands();
            this.StationSource = new ObservableCollection<StationViewModel>();
            this.timer.Interval = TimeSpan.FromMinutes(1.0);
            this.timer.Tick += Timer_Tick; 
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
       

        public ObservableCollection<StationViewModel> StationSource
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

        public StationViewModel CurrentStation
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

        public Geopoint MyLocation
        {
            get
            {
                return myLocation;
            }
            set
            {
                Set(() => MyLocation, ref myLocation, value);
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

        public Geopoint MapCenter
        {
            get
            {
                return mapCenter;
            }
            set
            {
                Set(() => MapCenter, ref mapCenter, value);
            }
        }

        #endregion

        // First time start up
        public async void Start()
        {
            await this.InitializeGeoLocator();
            await this.LoadStationDataAsync();
            this.CenterMapToMyLocation();
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

        public void SelectStation(StationViewModel station)
        {
            StationViewModel oldStation = this.CurrentStation;
            
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

        public async Task LoadStationDataAsync()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var bikeShares = await this.stationLoader.LoadBikeSharesAsync();

                // MJDTODO- Find city and bikeshare
                
                 await this.UpdateStationDataAsync();
                
            }
            else
            {
                this.SetLoadingStatus(false, "No Network Available.");
            }

        }

        public async Task UpdateStationDataAsync()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                this.timer.Stop();
                this.SetLoadingStatus(true, "Updating Bike Stations.");
                var stationData = await this.stationLoader.LoadBikeShareAsync("/v2/networks/hubway"); // MJDTODO - remove hard code
                var viewModels = await this.CreateViewModelsAsync(stationData);
                this.UpdateViewModels(viewModels);
                this.timer.Start();
            }
            else
            {
                this.SetLoadingStatus(false, "No Network Available.");
            }
        }

        public async Task<IDictionary<Guid, StationViewModel>> CreateViewModelsAsync(BikeShareNetwork bikeShare)
        {
            var vmDict = new Dictionary<Guid, StationViewModel>();

            await Task.Factory.StartNew(() =>
            {
                foreach(var station in bikeShare.Stations)
                {
                    var vm = StationViewModel.Create(station);
                    vmDict.Add(station.Id, vm);
                }
            });
            return vmDict;
        }

        private void UpdateViewModels(IDictionary<Guid, StationViewModel> updated)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var selected = this.CurrentStation;

                foreach (var station in this.StationSource)
                {
                    var id = station.Id;
                    if (updated.Keys.Contains(id))
                    {
                        station.Update(updated[id]);
                        updated.Remove(id);
                    }
                }

                foreach (var key in updated.Keys)
                {
                    this.StationSource.Add(updated[key]);
                }

                this.SelectStation(selected);
               // this.SetLoadingStatus(false, data.LastUpdated); // FIXME
            });
        }

        private void CenterMapToMyLocation()
        {
             this.MapCenter = this.MyLocation;
             this.IsMyLocationVisible = Visibility.Visible;
        }

        private void SelectNearestStationWithAvailableBike()
        {
            this.SelectNearestStationWithAvailablilty((StationViewModel s) => s.BikeCount > 0);
        }

        private void SelectNearestStationWithAvailableDock()
        {
            this.SelectNearestStationWithAvailablilty((StationViewModel s) => s.EmptyDockCount > 0);
        }

        private void SelectNearestStationWithAvailablilty(Predicate<StationViewModel> condition)
        {
            double minDist = Double.MaxValue;
            StationViewModel nearestStation = null;

            foreach (var station in this.StationSource)
            {
                double dist = GeoUtil.DistanceTo(myLocation.Position,station.Location.Position);
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

        private async void Timer_Tick(object sender, object e)
        {
           await this.UpdateStationDataAsync();
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
                station.Distance = (int)(GeoUtil.DistanceTo(station.Location.Position, this.myLocation.Position) * 1000.0);
            }

            var ordered = this.StationSource.OrderBy(x => x.Distance);
            this.StationSource = new ObservableCollection<StationViewModel>(ordered);
        }

        private bool StationSourceFilter(StationViewModel station)
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

            if (GeoUtil.DistanceTo(station.Location.Position, myCity.Center) < 50000)
            {
                return true;
            }

            return false;

        }

        private async Task InitializeGeoLocator()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            string status = null;
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    geolocator = new Geolocator { DesiredAccuracyInMeters = 20, MovementThreshold = 10 };

                    // Subscribe to the PositionChanged event to get location updates.
                    geolocator.PositionChanged += Geolocator_PositionChanged; ;


                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    geolocator.StatusChanged += Geolocator_StatusChanged;

                    
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    UpdateLocationData(pos);
                    break;

                case GeolocationAccessStatus.Denied:
                    status = "Access to location is denied.";
                    UpdateLocationData(null);
                    break;

                case GeolocationAccessStatus.Unspecified:
                    status = "Unspecified error.";
                    UpdateLocationData(null);
                    break;
            }

            if (status != null)
            {
                Messenger.Default.Send<StatusMessage>(new StatusMessage(status));
            }
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                UpdateLocationData(args.Position);
            });
        }

        private void UpdateLocationData(Geoposition pos)
        {
            this.MyLocation = new Geopoint(new BasicGeoposition { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude });
        }

        private void Geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
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
                case PositionStatus.NotAvailable:
                    status = "Location not available on this version of the OS.";
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
