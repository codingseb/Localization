using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    /// <summary>
    /// This is a localization dependant binding it recreate a standard binding and automatically update it when current language changed
    /// </summary>
    public class LocBinding : MarkupExtension
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LocBinding()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path to the binding source property.</param>
        public LocBinding(PropertyPath path)
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
            Path = path;
        }

        ~LocBinding()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.RemoveHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        /// <summary>
        /// Gets or sets the converter to use.
        /// </summary>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the parameter to pass to the Converter.
        /// </summary>
        public object ConverterParamter { get; set; }

        /// <summary>
        /// Gets or sets the name of the element to use as the binding source object.
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
        /// </summary>
        public RelativeSource RelativeSource { get; set; }

        /// <summary>
        /// Gets or sets the object to use as the binding source.
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the DataErrorValidationRule.
        /// </summary>
        public bool ValidatesOnDataErrors { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the ExceptionValidationRule.
        /// </summary>
        public bool ValidatesOnExceptions { get; set; }

        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }

        /// <summary>
        /// Gets or sets the culture in which to evaluate the converter.
        /// </summary>
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        /// <summary>
        /// The DependencyObject on which the binding is linked
        /// </summary>
        public DependencyObject TargetObject { get; private set; }

        /// <summary>
        /// The DependencyProperty on which the binding is linked
        /// </summary>
        public DependencyProperty TargetProperty { get; private set; }

        /// <summary>
        /// The internally created binding.
        /// </summary>
        public Binding Binding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            TargetProperty = service.TargetProperty as DependencyProperty;
            TargetObject = service.TargetObject as DependencyObject;
            if (TargetObject == null || TargetProperty == null)
            {
                return this;
            }

            if (Binding == null)
            {
                Binding = new Binding
                {
                    Path = Path,
                    Converter = Converter,
                    ConverterCulture = ConverterCulture,
                    ConverterParameter = ConverterParamter
                };

                if (ElementName != null) Binding.ElementName = ElementName;
                if (RelativeSource != null) Binding.RelativeSource = RelativeSource;
                if (Source != null) Binding.Source = Source;
                Binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
                Binding.ValidatesOnExceptions = ValidatesOnExceptions;

                BindingOperations.SetBinding(TargetObject, TargetProperty, Binding);
            }

            return Binding.ProvideValue(serviceProvider);
        }

        private void CurrentLanguageChanged(object sender, CurrentLanguageChangedEventArgs e)
        {
            if (TargetObject != null && TargetProperty != null)
            {
                BindingOperations.GetBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
                BindingOperations.GetMultiBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
            }
        }
    }
}
