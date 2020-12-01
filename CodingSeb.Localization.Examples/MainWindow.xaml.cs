using System.Windows;

namespace CodingSeb.Localization.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cbLanguageSelection.Focus();
        }

        private void LanguageChangedRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Languages.ReloadFiles();
            Loc.Instance.RaiseLanguageChangeEvents();
        }
    }
}
