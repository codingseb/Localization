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

            var method = new MethodDefinition("__CurrentLanguageChanged__", MethodAttributes.Private, TypeSystem.VoidReference);
            ILProcessor processor = method.Body.GetILProcessor();
            processor.Append(Instruction.Create(OpCodes.Ldstr, "Hello World"));
            processor.Append(Instruction.Create(OpCodes.Ret));
            typeDefinition.Methods.Add(method);
        }

        public FieldDefinition AddLocalizedPropertyNamesStaticList(TypeDefinition typeDefinition, IEnumerable<string> propertiesNames)
        {
            TypeReference listOfStringTypeReference = ModuleDefinition.ImportReference(typeof(List<string>));
            FieldDefinition field = new FieldDefinition("__localizedPropertyNames__", FieldAttributes.Private | FieldAttributes.Static, listOfStringTypeReference);

            var constructorOnStringList = ModuleDefinition.ImportReference(typeof(List<string>).GetConstructor(new Type[] { }));
            var addMethodOnStringList = ModuleDefinition.ImportReference(typeof(List<string>).GetMethod("Add", new Type[] { typeof(string) }));

            typeDefinition.Fields.Add(field);

            MethodDefinition staticConstructor = GetOrCreateStaticConstructor(typeDefinition);

            var il = staticConstructor.Body.GetILProcessor();

            if (il.Body.Instructions.LastOrDefault(i => i.OpCode == OpCodes.Ret) is Instruction instruction)
            {
                il.Remove(instruction);
            }

            il.Emit(OpCodes.Newobj, constructorOnStringList);

            //propertiesNames.ToList().ForEach(propertyName =>
            //{
            //    instructions.Add(Instruction.Create(OpCodes.Dup));
            //    instructions.Add(Instruction.Create(OpCodes.Ldstr, propertyName));
            //    instructions.Add(Instruction.Create(OpCodes.Callvirt, addMethodOnStringList));
            //});

            il.Emit(OpCodes.Stsfld, field);

            il.Emit(OpCodes.Ret);

            return field;
        }

        public MethodDefinition GetOrCreateStaticConstructor(TypeDefinition typeDefinition)
        {
            return typeDefinition.GetStaticConstructor() ??
                new MethodDefinition(".cctor", staticConstructorAttributes, TypeSystem.VoidReference);
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
