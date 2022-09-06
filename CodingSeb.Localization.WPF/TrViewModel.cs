using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    public class TrViewModel : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TrViewModelData();
        }
    }

    public class TrViewModelData : DynamicObject, INotifyPropertyChanged
    {
        private readonly List<string> textIdsList = new List<string>();

        public TrViewModelData()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        ~TrViewModelData()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.RemoveHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        private void CurrentLanguageChanged(object sender, CurrentLanguageChangedEventArgs args)
        {
            textIdsList.ForEach(NotifyPropertyChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(!textIdsList.Contains(binder.Name))
                textIdsList.Add(binder.Name);

            result = Loc.Tr(binder.Name);

            return true;
        }
    }
}
