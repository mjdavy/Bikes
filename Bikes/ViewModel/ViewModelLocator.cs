/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Bikes"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Bikes.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace Bikes.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator 
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<CitiesViewModel>();
            SimpleIoc.Default.Register<CountriesViewModel>();

            Messenger.Default.Register<SettingsChangedMessage>(this.MainViewModel, this.MainViewModel.OnSettingsChanged);
            Messenger.Default.Register<StatusMessage>(this.MainViewModel, this.MainViewModel.OnStatusChanged);
            Messenger.Default.Register<SettingsChangedMessage>(this.SettingsViewModel, this.SettingsViewModel.OnSettingsChanged);
            Messenger.Default.Register<CountryChangedMessage>(this.CitiesViewModel, this.CitiesViewModel.OnCountryChanged);
            Messenger.Default.Register<CountryChangedMessage>(this.CountriesViewModel, this.CountriesViewModel.OnCountryChanged);
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public CitiesViewModel CitiesViewModel
        {
            get 
            {
                return ServiceLocator.Current.GetInstance<CitiesViewModel>();
            }
        }

        public CountriesViewModel CountriesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CountriesViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            //navigationService.Configure("Details", typeof(DetailsPage));
            // navigationService.Configure("key1", typeof(OtherPage1));
            // navigationService.Configure("key2", typeof(OtherPage2));

            return navigationService;
        }
    }
}