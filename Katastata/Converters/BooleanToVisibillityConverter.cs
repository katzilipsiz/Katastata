using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Katastata.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool invert = false;
                if (parameter != null)
                {
                    bool.TryParse(parameter.ToString(), out invert);
                }

                if (invert)
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}