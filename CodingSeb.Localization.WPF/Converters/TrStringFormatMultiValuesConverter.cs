using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    public class TrStringFormatMultiValuesConverter : MarkupExtension, IMultiValueConverter
    {
        public TrStringFormatMultiValuesConverter()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        public TrStringFormatMultiValuesConverter(string textId)
        {
            TextId = textId;
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        ~TrStringFormatMultiValuesConverter()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.RemoveHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        /// <summary>
        /// The text to return if no text correspond to textId in the current language
        /// </summary>
        public string DefaultText { get; set; } = null;

        /// <summary>
        /// The language id in which to get the translation. To Specify if not CurrentLanguage
        /// </summary>
        public string LanguageId { get; set; } = null;

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        [ConstructorArgument("textId")]
        public string TextId { get; set; } = null;

        /// <summary>
        /// To provide a prefix to add at the begining of the translated text.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// To provide a suffix to add at the end of the translated text.
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Prefix + string.Format(string.IsNullOrEmpty(TextId) ? "" : Loc.Tr(TextId, DefaultText?.Replace("[apos]", "'"), LanguageId), values) + Suffix;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

        private DependencyObject xamlTargetObject;
        private DependencyProperty xamlDependencyProperty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                var xamlContext = serviceProvider.GetType()
                    .GetRuntimeFields().ToList()
                    .Find(f => f.Name.Equals("_xamlContext"))
                    .GetValue(serviceProvider);

                xamlTargetObject = xamlContext?.GetType()
                    .GetProperty("GrandParentInstance")
                    .GetValue(xamlContext) as DependencyObject;

                var xamlProperty = xamlContext?.GetType()
                    .GetProperty("GrandParentProperty")
                    .GetValue(xamlContext);

                xamlDependencyProperty = xamlProperty?.GetType()
                    .GetProperty("DependencyProperty")
                    .GetValue(xamlProperty) as DependencyProperty;

                if (string.IsNullOrEmpty(TextId))
                {
                    if (xamlTargetObject != null && xamlDependencyProperty != null)
                    {
                        string context = xamlTargetObject.GetContextByName();
                        string obj = xamlTargetObject.FormatForTextId();
                        string property = xamlDependencyProperty.ToString();

                        TextId = $"{context}.{obj}.{property}";
                    }
                    else if (!string.IsNullOrEmpty(DefaultText))
                    {
                        TextId = DefaultText;
                    }
                }
            }
            catch (InvalidCastException)
            {
                // For Xaml Design Time
                TextId = Guid.NewGuid().ToString();
            }

            return this;
        }

        private void CurrentLanguageChanged(object sender, CurrentLanguageChangedEventArgs e)
        {
            if (xamlTargetObject != null && xamlDependencyProperty != null)
            {
                BindingOperations.GetBindingExpression(xamlTargetObject, xamlDependencyProperty)?.UpdateTarget();
            }
        }
    }
}
