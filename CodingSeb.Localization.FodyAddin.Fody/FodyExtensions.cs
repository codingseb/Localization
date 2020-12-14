using Mono.Cecil;
using System.Linq;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public static class FodyExtensions
    {
        public static bool IsINotifyPropertyChanged(this TypeDefinition typeDefinition)
        {
            if (typeDefinition?.FullName.Equals("System.Object") != false)
                return false;
            else if (typeDefinition.Interfaces.Any(@interface => @interface.InterfaceType.FullName.Equals("System.ComponentModel.INotifyPropertyChanged")))
                return true;
            else if (typeDefinition.BaseType is TypeDefinition parentTypeDefinition)
                return parentTypeDefinition.IsINotifyPropertyChanged();
            else
                return false;
        }
    }
}
