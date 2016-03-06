using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bikes.View
{
    public partial class LocationServicesPermissionPage : Page
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
               this.Frame.GoBack();
            }
            else
            {
                Application.Current.Exit();
            }
        }
    }
}