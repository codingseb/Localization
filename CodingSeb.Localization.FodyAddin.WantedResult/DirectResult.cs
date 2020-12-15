using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CodingSeb.Localization.FodyAddin.WantedResult
{
    public class DirectResult : INotifyPropertyChanged
    {
        private static readonly List<string> __localizedPropertyNames__ = new List<string>()
        {
            "TestProperty",
            "OtherProperty"
        };

        public DirectResult()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), __CurrentLanguageChanged__);
        }

        protected void __CurrentLanguageChanged__(object sender, CurrentLanguageChangedEventArgs e)
        {
            __localizedPropertyNames__.ForEach(__NotifyPropertyChanged__);
        }

        private void __NotifyPropertyChanged__(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
