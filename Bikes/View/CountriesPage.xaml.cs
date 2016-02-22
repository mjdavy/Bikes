using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Bikes.Model;
using Bikes.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace Bikes.View
{
    public partial class CountriesPage : PhoneApplicationPage
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

        private void ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var vm = this.DataContext as CountriesViewModel;

            var msg = new CountryChangedMessage
            {
                Country = vm.SelectedCountry
            };

            Messenger.Default.Send<CountryChangedMessage>(msg);
            this.NavigationService.GoBack();
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