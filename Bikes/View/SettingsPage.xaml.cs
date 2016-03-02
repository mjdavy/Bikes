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
            // FIXME this.NavigationService.GoBack();
        }
    }
}