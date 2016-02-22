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
using Bikes.Model;

namespace Bikes.View
{
    public partial class LocationServicesPermissionPage : PhoneApplicationPage
    {
        public LocationServicesPermissionPage()
        {
            InitializeComponent();
        }

        private void AllowButtonClick(object sender, RoutedEventArgs e)
        {
            this.UseOfLocation(true);
        }

        private void DontAllowButtonClick(object sender, RoutedEventArgs e)
        {
            this.UseOfLocation(false);
        }

        private void UseOfLocation(bool allow)
        {
            if (allow)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                Application.Current.Terminate();
            }
        }
    }
}