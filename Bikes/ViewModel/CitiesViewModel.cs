using Bikes.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

namespace Bikes.ViewModel
{
    public class CitiesViewModel : ObservableObject
    {
        private City _selectedCity;
        private ObservableCollection<City> _citySource;

        public CitiesViewModel()
        {
            Initialize();
        }

        public ObservableCollection<City> CitySource
        {
            get
            {
                return _citySource;
            }
            set
            {
                Set(() => CitySource, ref _citySource, value);
            }
        }

        public City SelectedCity
        {
            get
            {
                return _selectedCity;
            }
            set
            {
                Set(() => SelectedCity, ref _selectedCity, value);
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
            
        }

        public void Initialize()
        { 
            this.CitySource = new ObservableCollection<City>(Cities.AllCities.Values);
            this.SortByCityName();
            this.SelectedCity = Cities.CurrentCity;
            this.SelectedCountry = SelectedCity.Country;
        }

        bool FilterCountries(City filterCity)
        {
            return filterCity.Country.Equals(Countries.CurrentCountry) ? true : false;
        }

        internal void OnCountryChanged(CountryChangedMessage msg)
        {
            this.SelectedCountry = msg.Country;
        }
    }
}
