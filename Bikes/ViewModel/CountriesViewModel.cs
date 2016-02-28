using Bikes.Model;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bikes.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CountriesViewModel : ViewModelBase
    {
        private ObservableCollection<Country> _countrySource;
        private Country _selectedCountry;

        /// <summary>
        /// Initializes a new instance of the CountriesViewModel class.
        /// </summary>
        public CountriesViewModel()
        {
        }

        public ObservableCollection<Country> CountrySource
        {
            get
            {
                return _countrySource;
            }
            set
            {
                Set(() => CountrySource, ref _countrySource, value);
            }
        }

        public Country SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {
                Set(() => SelectedCountry, ref _selectedCountry, value);
            }
        }

        private void Initialize()
        {
            var sortedCountries = SortByCountryName(Countries.AllCountries.Values);
            this.CountrySource = new ObservableCollection<Country>(sortedCountries);
            this.SelectedCountry = Countries.CurrentCountry;
        }

        private IOrderedEnumerable<Country> SortByCountryName(ICollection<Country> countries)
        {
            var sorted = from c in countries orderby c.Name select c;
            return sorted; 
        }

        internal void OnCountryChanged(CountryChangedMessage msg)
        {
            this.SelectedCountry = msg.Country;
        }
    }
}