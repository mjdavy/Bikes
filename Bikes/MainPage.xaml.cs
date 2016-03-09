using Bikes.Model;
using Bikes.View;
using Bikes.ViewModel;
using System;
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
            var newStation = stationPin.DataContext as StationViewModel;
            this.viewModel.SelectStation(newStation);
        }

        private void StationList_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StationsPage));
            this.viewModel.UpdateDistances();
        }

        private void Settings_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
    }
}