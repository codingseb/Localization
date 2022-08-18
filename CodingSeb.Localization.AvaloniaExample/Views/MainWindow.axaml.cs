using Avalonia.Controls;
using Avalonia.Interactivity;
using CodingSeb.Localization.AvaloniaExamples;

namespace CodingSeb.Localization.AvaloniaExample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LanguageChangedRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Languages.ReloadFiles();
            Loc.Instance.RaiseLanguageChangeEvents();
        }
    }
}
