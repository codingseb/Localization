using System;
using System.Collections.Generic;
using System.Text;

namespace CodingSeb.Localization.AvaloniaExample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Loc LanguagesManager => Loc.Instance;
    }
}
