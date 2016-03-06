using Bikes.Model;
using Bikes.ViewModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Bikes.View
{
    public partial class CitiesPage : Page
    {
        public CitiesPage()
        {
            InitializeComponent();
        }

        // You have to do this because there is no IsSynchronizedWithCurrentItem
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedCity = e.AddedItems[0] as City;
                var vm = this.DataContext as CitiesViewModel;
                vm.SelectedCity = selectedCity;
            }
        }


        private void ListBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var vm = this.DataContext as CitiesViewModel;
            vm.SetCurrentCity();
            this.Frame.GoBack();
        }
    }
}