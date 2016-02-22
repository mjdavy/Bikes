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
using System.Windows.Data;
using System.Collections.ObjectModel;
using Bikes.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Maps.Controls;

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
                AppSettings.Instance.SaveSettings();
                this.RaisePropertyChanged("DefaultToNearestCity");
                this.RaisePropertyChanged("EnableCityChooser");
                Messenger.Default.Send<SettingsChangedMessage>(new SettingsChangedMessage(AppSettings.DefaultToNearestCitySetting));
            }
        }

        public ObservableCollection<MapCartographicMode> MapModes { get; set; }

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

        public MapCartographicMode MapMode
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
            MapCartographicMode[] modes = Enum.GetValues(typeof(MapCartographicMode)) as MapCartographicMode[];
            var collection = Array.AsReadOnly<MapCartographicMode>(modes);
            this.MapModes = new ObservableCollection<MapCartographicMode>(collection);
        }

    }
}
