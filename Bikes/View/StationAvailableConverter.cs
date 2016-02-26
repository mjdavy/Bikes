using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Bikes.View
{
    public class StationAvailableConverter : IValueConverter
    {
        private static Brush whiteBrush = new SolidColorBrush(Colors.White);
        private static Brush grayBrush = new SolidColorBrush(Colors.DarkGray);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool unavailable = (bool)value;

            if (unavailable)
            {
                return grayBrush;
            }
            else
            {
                return whiteBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
