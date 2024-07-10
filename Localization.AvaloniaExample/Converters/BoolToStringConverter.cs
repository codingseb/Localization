using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace CodingSeb.Localization.AvaloniaExample.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string FalseValue { get; set; }

        public string TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
