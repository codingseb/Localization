using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CodingSeb.Localization.AvaloniaExample.ViewModels;
using CodingSeb.Localization.AvaloniaExample.Views;
using CodingSeb.Localization.AvaloniaExamples;
using PropertyChanged;
using System;

namespace CodingSeb.Localization.AvaloniaExample
{
    [DoNotNotify]
    public partial class App : Application
    {
        public override void Initialize()
        {
            GC.KeepAlive(typeof(CodingSeb.Localization.Avalonia.Tr).Assembly);
            Languages.Init();
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
