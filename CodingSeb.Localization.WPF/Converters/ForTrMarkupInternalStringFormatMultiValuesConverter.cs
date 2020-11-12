using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CodingSeb.Localization.WPF.Converters
{
    internal class ForTrMarkupInternalStringFormatMultiValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format((string)values[0], values.Skip(1).ToArray());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
