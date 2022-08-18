using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodingSeb.Localization.AvaloniaExample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Loc LanguagesManager => Loc.Instance;

        public List<string> Labels => Loc.Instance
                    .TranslationsDictionary
                    .Keys.ToList()
                    .FindAll(k => k.StartsWith("Text:"));

        //public Visibility VisibilityForText { get; set; } = Visibility.Hidden;

        //[Localize("ANiceText")]
        //public string AutoTranslation { get; set; }

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
