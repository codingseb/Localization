using CodingSeb.Localization.Loaders;
using System.ComponentModel;
using System.IO;
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

#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                var JsonFileLoader = new JsonFileLoader();

                LocalizationLoader.Instance.FileLanguageLoaders.Add(JsonFileLoader);

                JsonFileLoader.LoadFromString(File.ReadAllText(@"C:\Projets\CS.Loc\CodingSeb.Localization.Examples\bin\Debug\netcoreapp3.1\lang\Example1.loc.json"), LocalizationLoader.Instance, "Example1.loc.json");
            }
#endif
        }

        private void LanguageChangedRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Languages.ReloadFiles();
            Loc.Instance.RaiseLanguageChangeEvents();
        }
    }
}
