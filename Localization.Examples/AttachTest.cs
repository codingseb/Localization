using System.Windows;
using System.Windows.Controls;

namespace CodingSeb.Localization.Examples
{
    /// <summary>
    /// To Test Tr in AttachedProperty
    /// </summary>
    public static class AttachTest
    {
        public static string GetAttachedText(DependencyObject obj)
        {
            return (string)obj.GetValue(AttachedTextProperty);
        }

        public static void SetAttachedText(DependencyObject obj, string value)
        {
            obj.SetValue(AttachedTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedTextProperty =
            DependencyProperty.RegisterAttached("AttachedText", typeof(string), typeof(AttachTest), new FrameworkPropertyMetadata("", AttachedTextchanged));

        private static void AttachedTextchanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is TextBlock textBlock)
            {
                textBlock.Text = args.NewValue.ToString();
            }
        }
    }
}
