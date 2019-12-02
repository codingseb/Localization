using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodingSeb.Localization.Examples
{
    public class NotifyPropertyChangedBaseClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
