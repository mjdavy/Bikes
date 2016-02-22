using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Bikes.ViewModel;
using Bikes.Model;

namespace Bikes.View
{
    public partial class CitiesPage : PhoneApplicationPage
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

        private void ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var vm = this.DataContext as CitiesViewModel;
            vm.SetCurrentCity();
            this.NavigationService.GoBack();
        }

    }
}