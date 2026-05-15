using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaEurope.WPF.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
            => value is bool b && b
                ? new SolidColorBrush(Color.FromRgb(233, 69, 96))
                : new SolidColorBrush(Color.FromRgb(46, 204, 113));

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}