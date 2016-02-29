using Bikes.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace Bikes.ViewModel
{
    public class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            InitializeMapModes();
        }

        public bool DefaultToNearestCity
        {
            get
            {
                return AppSettings.Instance.DefaultToNearestCity;
            }
            set
            {
                AppSettings.Instance.DefaultToNearestCity = value;
                this.RaisePropertyChanged("DefaultToNearestCity");
                this.RaisePropertyChanged("EnableCityChooser");
                Messenger.Default.Send<SettingsChangedMessage>(new SettingsChangedMessage(AppSettings.DefaultToNearestCitySetting));
            }
        }

        public ObservableCollection<MapStyle> MapModes { get; set; }

        public Visibility EnableCityChooser
        {
            get 
            {
                return AppSettings.Instance.DefaultToNearestCity ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public City SelectedCity
        {
            get
            {
                return Cities.CurrentCity;
            }
            set
            {
                Cities.CurrentCity = value;
                this.SelectedCountry = value.Country;
                this.RaisePropertyChanged("SelectedCity");
            }
        }
        public Country SelectedCountry
        {
            get
            {
                return Countries.CurrentCountry;
            }
            set
            {
                Countries.CurrentCountry = value;
                this.RaisePropertyChanged("SelectedCountry");
            }
        }

        public MapStyle MapMode
        {
            get
            {
                return AppSettings.Instance.MapMode;
            }
            set
            {
                AppSettings.Instance.MapMode = value;
                this.RaisePropertyChanged("MapMode");
                Messenger.Default.Send<SettingsChangedMessage>(new SettingsChangedMessage("MapMode"));
            }
        }

        internal async void OnSettingsChanged(SettingsChangedMessage msg)
        {
            if (msg.ChangedSetting == AppSettings.CurrentCitySetting)
            {
                this.DefaultToNearestCity = false;
            }

            await Cities.DetermineCurrentCityAsync();
            this.SelectedCity = Cities.CurrentCity;
            
            var countryChangedMsg = new CountryChangedMessage()
            {
                Country = this.SelectedCountry
            };

            Messenger.Default.Send<CountryChangedMessage>(countryChangedMsg);
        }

        private void InitializeMapModes()
        {
            MapStyle[] modes = Enum.GetValues(typeof(MapStyle)) as MapStyle[];
            this.MapModes = new ObservableCollection<MapStyle>(modes);
        }

    }
}
