using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.Examples
{
    public class BoolToStringConverter : MarkupExtension, IValueConverter
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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
