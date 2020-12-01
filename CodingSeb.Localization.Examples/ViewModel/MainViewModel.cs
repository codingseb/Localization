using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodingSeb.Localization.Examples
{
    public sealed class MainViewModel : NotifyPropertyChangedBaseClass
    {
        private static MainViewModel instance;

        public static MainViewModel Instance
        {
            get
            {
                return instance ?? (instance = new MainViewModel());
            }
        }

        private MainViewModel()
        {
            Languages.Init();
        }

        public Loc LanguagesManager
        {
            get { return Loc.Instance; }
        }

        public List<string> Labels
        {
            get
            {
                return Loc.Instance
                    .TranslationsDictionary
                    .Keys.ToList()
                    .FindAll(k => k.StartsWith("Text:"));
            }
        }

        public string Label { get; set; }

        public ObservableCollection<ItemViewModel> Items { get; set; } = new ObservableCollection<ItemViewModel>()
        {
            new ItemViewModel()
            {
                ContentName="Text:Hello",
                OtherValue = 1
            },
            new ItemViewModel()
            {
                ContentName="Text:HowAreYou",
                OtherValue=2
            }
        };
    }
}
