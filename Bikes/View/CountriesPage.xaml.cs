using Bikes.Model;
using Bikes.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Bikes.View
{
    public sealed partial class CountriesPage : Page
    {
        // Constructor
        public CountriesPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedCountry = e.AddedItems[0] as Country;
                var vm = this.DataContext as CountriesViewModel;
                vm.SelectedCountry = selectedCountry;
            }
        }


        private void ListBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var vm = this.DataContext as CountriesViewModel;

            var msg = new CountryChangedMessage
            {
                Country = vm.SelectedCountry
            };

            Messenger.Default.Send<CountryChangedMessage>(msg);
            this.Frame.GoBack();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}