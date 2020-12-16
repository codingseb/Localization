using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public static class FodyExtensions
    {
        private static readonly List<string> propertyChangedTriggerMethodCommonNames = new List<string>()
        {
            "NotifyPropertyChanged",
            "NotifyPropertyChange",
            "OnPropertyChanged",
            "OnPropertyChange",
            "RaisePropertyChanged",
            "RaisePropertyChange",
            "NotifyOfPropertyChange",
            "NotifyOfPropertyChanged",
            "TriggerPropertyChange",
            "TriggerPropertyChanged",
            "PropertyChangeTrigger",
            "PropertyChangedTrigger",
            "FirePropertyChange",
            "FirePropertyChanged",
        };

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

        public static MethodDefinition FindPropertyChangedTriggerMethod(this TypeDefinition typeDefinition)
        {
            if (typeDefinition?.FullName.Equals("System.Object") != false)
                return null;
            else if (typeDefinition.Methods.SingleOrDefault(m => propertyChangedTriggerMethodCommonNames.Any(name => name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))) is MethodDefinition method)
                return method;
            else if (typeDefinition.BaseType is TypeDefinition parentTypeDefinition)
                return parentTypeDefinition.FindPropertyChangedTriggerMethod();
            else
                return null;
        }

        public static bool HasLocalizeAttribute(this PropertyDefinition propertyDefinition)
        {
            return propertyDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.Name.Equals("LocalizeAttribute"));
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] args)
        {
            var reference = new MethodReference(
                self.Name,
                self.ReturnType,
                self.DeclaringType.MakeGenericInstanceType(args))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (var genericParam in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
            }

            return reference;
        }
    }
}
