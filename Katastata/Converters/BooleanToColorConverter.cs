using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Katastata.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return new SolidColorBrush(Color.FromRgb(0, 122, 204)); // Синий
            }
            return new SolidColorBrush(Color.FromRgb(108, 117, 125)); // Серый
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}