using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public MethodReference DebuggerNonUserCodeAttributeConstructor;

        private const MethodAttributes staticConstructorAttributes =
            MethodAttributes.Private |
            MethodAttributes.HideBySig |
            MethodAttributes.Static |
            MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName;

        public override void Execute()
        {
            ModuleDefinition
                .Types
                .Where(typeDefinition =>
                    typeDefinition.IsClass &&
                    typeDefinition.IsINotifyPropertyChanged() &&
                    typeDefinition.Properties.Any(property => property.HasLocalizeAttribute()))
                .ToList()
                .ForEach(AddCurrentLanguageChangedSensitivity);
        }

        private void AddCurrentLanguageChangedSensitivity(TypeDefinition typeDefinition)
        {
            IEnumerable<PropertyDefinition> propertyToLocalize = typeDefinition.Properties.Where(property => property.HasLocalizeAttribute());

            FieldDefinition propertyListFieldDefinition = AddLocalizedPropertyNamesStaticList(typeDefinition, propertyToLocalize.Select(p => p.Name));

            MethodDefinition triggerPropertyChangedMethod = typeDefinition.FindPropertyChangedTriggerMethod();

            if (triggerPropertyChangedMethod == null)
            {
                triggerPropertyChangedMethod = new MethodDefinition("__NotifyPropertyChanged__", MethodAttributes.Private, TypeSystem.VoidReference);
                triggerPropertyChangedMethod.Parameters.Add(new ParameterDefinition("propertyName", ParameterAttributes.None, ModuleDefinition.ImportReference(typeof(string))));
                typeDefinition.Methods.Add(triggerPropertyChangedMethod);

                // TODO
            }

            var method = new MethodDefinition("__CurrentLanguageChanged__", MethodAttributes.Private, TypeSystem.VoidReference);
            method.Parameters.Add(new ParameterDefinition("sender", ParameterAttributes.None, TypeSystem.ObjectReference));
            method.Parameters.Add(new ParameterDefinition("e", ParameterAttributes.None, ModuleDefinition.ImportReference(typeof(CurrentLanguageChangedEventArgs))));
            ILProcessor processor = method.Body.GetILProcessor();
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldsfld, propertyListFieldDefinition);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldftn, triggerPropertyChangedMethod);
            processor.Emit(OpCodes.Newobj, ModuleDefinition.ImportReference(typeof(Action<string>).GetConstructors().First()));
            processor.Emit(OpCodes.Callvirt, ModuleDefinition.ImportReference(typeof(List<string>).GetMethod("ForEach", new Type[] { typeof(Action<string>) })));
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ret);
            typeDefinition.Methods.Add(method);
        }

        public FieldDefinition AddLocalizedPropertyNamesStaticList(TypeDefinition typeDefinition, IEnumerable<string> propertiesNames)
        {
            TypeReference listOfStringTypeReference = ModuleDefinition.ImportReference(typeof(List<string>));
            FieldDefinition field = new FieldDefinition("__localizedPropertyNames__", FieldAttributes.Private | FieldAttributes.Static, listOfStringTypeReference);

            var constructorOnStringList = ModuleDefinition.ImportReference(typeof(List<string>).GetConstructor(new Type[] { }));
            var addMethodOnStringList = ModuleDefinition.ImportReference(typeof(List<string>).GetMethod("Add", new Type[] { typeof(string) }));

            typeDefinition.Fields.Add(field);

            MethodDefinition staticConstructor = typeDefinition.GetStaticConstructor();

            if (staticConstructor == null)
            {
                staticConstructor = new MethodDefinition(".cctor", staticConstructorAttributes, TypeSystem.VoidReference);
                typeDefinition.Methods.Add(staticConstructor);
            }

            var instructions = staticConstructor.Body.Instructions;

            if (instructions.LastOrDefault(i => i.OpCode == OpCodes.Ret) is Instruction instruction)
            {
                instructions.Remove(instruction);
            }

            instructions.Add(Instruction.Create(OpCodes.Newobj, constructorOnStringList));

            propertiesNames.ToList().ForEach(propertyName =>
            {
                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldstr, propertyName));
                instructions.Add(Instruction.Create(OpCodes.Callvirt, addMethodOnStringList));
            });

            instructions.Add(Instruction.Create(OpCodes.Stsfld, field));

            instructions.Add(Instruction.Create(OpCodes.Ret));

            return field;
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
            yield return "WindowsBase";
            yield return "CodingSeb.Localization";
        }

        public override bool ShouldCleanReference => true;
    }
}
