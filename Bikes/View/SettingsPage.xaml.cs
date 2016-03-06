using System;
using Windows.UI.Xaml.Controls;

namespace Bikes.View
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarDoneClick(object sender, EventArgs e)
        {
            this.Frame.GoBack();
        }

        private void CityButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CitiesPage));
        }
    }
}