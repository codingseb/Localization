using ReactiveUI;

namespace CodingSeb.Localization.AvaloniaExample.ViewModels
{
    public class ItemViewModel : ReactiveObject
    {
        public string ContentName { get; set; }
        public int OtherValue { get; set; }
    }
}
