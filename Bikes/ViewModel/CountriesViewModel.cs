using Bikes.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

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
        private ObservableCollection<Country> countryCollection;
        private CollectionViewSource countrySource;

        /// <summary>
        /// Initializes a new instance of the CountriesViewModel class.
        /// </summary>
        public CountriesViewModel()
        {
        }

        public CollectionViewSource CountrySource
        {
            get
            {
                if (this.countrySource == null)
                {
                    this.Initialize();
                }
                return this.countrySource;
            }
            set
            {
                this.countrySource = value;
                this.RaisePropertyChanged("CountrySource");
            }
        }

        public Country SelectedCountry
        {
            get
            {
                return this.CountrySource.View.CurrentItem as Country;
            }
            set
            {
                this.CountrySource.View.MoveCurrentTo(value);
                this.RaisePropertyChanged("SelectedCountry");
            }
        }

        private void Initialize()
        {
            this.countrySource = new CollectionViewSource();
            this.countryCollection = new ObservableCollection<Country>(Countries.AllCountries.Values);
            this.CountrySource.Source = this.countryCollection;
           
            this.SortByCountryName();
            this.SelectedCountry = Countries.CurrentCountry;
        }

        private void SortByCountryName()
        {
            this.CountrySource.SortDescriptions.Clear();
            this.CountrySource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        internal void OnCountryChanged(CountryChangedMessage msg)
        {
            this.SelectedCountry = msg.Country;
        }
    }
}