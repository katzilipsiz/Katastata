using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Katastata.Converters
{
    public class TrackingStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTracking)
            {
                return isTracking
                    ? new SolidColorBrush(Color.FromRgb(40, 167, 69)) // Зеленый
                    : new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Красный
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}