using Bikes.Model;
using Bikes.ViewModel;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;

namespace Bikes
{
    public partial class MainPage : Page
    {
        private MainViewModel viewModel;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.viewModel = this.DataContext as MainViewModel; // Hack
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            //  FIXME this.NavigationService.Navigate(new Uri("/View/SettingsPage.xaml", UriKind.Relative));
        }

        private void StationListClick(object sender, EventArgs e)
        {
            // FIXME this.NavigationService.Navigate(new Uri("/View/StationsPage.xaml", UriKind.Relative));
            this.viewModel.UpdateDistances();
        }


        private void myMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            this.viewModel.SelectStation(null);
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.viewModel.Start();
        }

        private void Station_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ContentControl stationPin = sender as ContentControl;
            Station newStation = stationPin.DataContext as Station;
            this.viewModel.SelectStation(newStation);
        }
    }
}