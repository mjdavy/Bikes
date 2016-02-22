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
using System.Device.Location;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using Bikes.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Bikes.ViewModel
{
    public class CitiesViewModel : ObservableObject
    {
        private ObservableCollection<City> cityCollection;
        private CollectionViewSource citySource;

        public CitiesViewModel()
        {
        }

        public CollectionViewSource CitySource
        {
            get
            {
                if (this.citySource == null)
                {
                    this.Initialize();
                }
                return this.citySource;
            }
            set
            {
                this.citySource = value;
                this.RaisePropertyChanged("CitySource");
            }
        }

        public City SelectedCity
        {
            get
            {
                return this.CitySource.View.CurrentItem as City;
            }
            set
            {
                this.CitySource.View.MoveCurrentTo(value);
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

        public void SetCurrentCity()
        {
            Cities.CurrentCity = SelectedCity;
            var msg = new SettingsChangedMessage(AppSettings.CurrentCitySetting);
            Messenger.Default.Send<SettingsChangedMessage>(msg);
        }

        public void SortByCityName()
        {
            this.CitySource.SortDescriptions.Clear();
            this.CitySource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public void Initialize()
        {
            this.citySource = new CollectionViewSource();
            this.cityCollection = new ObservableCollection<City>(Cities.AllCities.Values);
            this.CitySource.Source = this.cityCollection;
            this.CitySource.Filter += CitySource_Filter;
            this.SortByCityName();
            this.SelectedCity = Cities.CurrentCity;
            this.SelectedCountry = SelectedCity.Country;
        }

        void CitySource_Filter(object sender, FilterEventArgs e)
        {
            City filterCity = e.Item as City;

            e.Accepted = filterCity.Country.Equals(Countries.CurrentCountry) ? true : false;
        }

        internal void OnCountryChanged(CountryChangedMessage msg)
        {
            this.SelectedCountry = msg.Country;
            this.CitySource.View.Refresh();
        }
    }
}
