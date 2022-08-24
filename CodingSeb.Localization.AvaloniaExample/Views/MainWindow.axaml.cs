using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;
using CodingSeb.Localization.AvaloniaExamples;
using PropertyChanged;

namespace CodingSeb.Localization.AvaloniaExample.Views
{
    [DoNotNotify]
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
