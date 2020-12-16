using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        #region Attibutes

        private const MethodAttributes staticConstructorAttributes =
            MethodAttributes.Private |
            MethodAttributes.HideBySig |
            MethodAttributes.Static |
            MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName;

        #endregion

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

            var propertyListFieldDefinition = AddLocalizedPropertyNamesStaticList(typeDefinition, propertyToLocalize.Select(p => p.Name));

            var triggerPropertyChangedMethod = typeDefinition.FindPropertyChangedTriggerMethod();

            var currentLanguageChangedMethod = AddCurrentLanguageChangedMethod(typeDefinition, propertyListFieldDefinition, triggerPropertyChangedMethod);

            List<MethodDefinition> exclusiveAlwaysCalledConstructors = typeDefinition.GetConstructors().Where(constructor =>
            {
                return constructor.Body.Instructions.Count > 2 &&
                    constructor.Body.Instructions[1].OpCode == OpCodes.Call &&
                    constructor.Body.Instructions[1].Operand is MethodReference methodReference &&
                    methodReference.DeclaringType != typeDefinition;
            }).ToList();

            var weakEventManagerType = ModuleDefinition.ImportReference(FindTypeDefinition("System.Windows.WeakEventManager`2"));
            var locType = ModuleDefinition.ImportReference(FindTypeDefinition("CodingSeb.Localization.Loc"));
            var currentLanguageChangedEventArgsType = ModuleDefinition.ImportReference(FindTypeDefinition("CodingSeb.Localization.CurrentLanguageChangedEventArgs"));
            var genericEventManagerType = ModuleDefinition.ImportReference(weakEventManagerType.MakeGenericInstanceType(locType, currentLanguageChangedEventArgsType));
            //var resolvedEventManagerType = ModuleDefinition.ImportReference(genericEventManagerType.Resolve());
            var addHandlerMethod = ModuleDefinition.ImportReference(genericEventManagerType.Resolve().Methods.FirstOrDefault(m => m.Name.Equals("AddHandler")));
            var genericAddHandlerMethod = ModuleDefinition.ImportReference(addHandlerMethod.MakeHostInstanceGeneric(locType, currentLanguageChangedEventArgsType));

            var tempMethod = typeDefinition.Methods.FirstOrDefault(m => m.Name.Equals("MyMethod"));

            //exclusiveAlwaysCalledConstructors.ForEach(constructor =>
            //{
            //    var instructions = constructor.Body.Instructions;

            //    if (instructions.LastOrDefault(i => i.OpCode == OpCodes.Ret) is Instruction instruction)
            //    {
            //        instructions.Remove(instruction);
            //    }

            //    instructions.Add(Instruction.Create(OpCodes.Nop));
            //    instructions.Add(Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(typeof(Loc).GetProperty("Instance").GetGetMethod())));
            //    instructions.Add(Instruction.Create(OpCodes.Ldstr, "CurrentLanguageChanged"));
            //    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            //    instructions.Add(Instruction.Create(OpCodes.Ldftn, currentLanguageChangedMethod));
            //    instructions.Add(Instruction.Create(OpCodes.Newobj, ModuleDefinition.ImportReference(typeof(CurrentLanguageChangedEventArgs).GetConstructors()[0])));
            //    instructions.Add(Instruction.Create(OpCodes.Call, genericAddHandlerMethod));
            //    instructions.Add(Instruction.Create(OpCodes.Nop));
            //    instructions.Add(Instruction.Create(OpCodes.Ret));
            //});
        }

        private MethodDefinition AddCurrentLanguageChangedMethod(TypeDefinition typeDefinition, FieldDefinition propertyListFieldDefinition,  MethodDefinition triggerPropertyChangedMethod)
        {
            var method = new MethodDefinition("__CurrentLanguageChanged__", MethodAttributes.Private, TypeSystem.VoidReference);
            method.Parameters.Add(new ParameterDefinition("sender", ParameterAttributes.None, TypeSystem.ObjectReference));
            method.Parameters.Add(new ParameterDefinition("e", ParameterAttributes.None, ModuleDefinition.ImportReference(typeof(CurrentLanguageChangedEventArgs))));
            ILProcessor processor = method.Body.GetILProcessor();
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldsfld, propertyListFieldDefinition);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldftn, triggerPropertyChangedMethod);
            processor.Emit(OpCodes.Newobj, ModuleDefinition.ImportReference(typeof(Action<string>).GetConstructors()[0]));
            processor.Emit(OpCodes.Callvirt, ModuleDefinition.ImportReference(typeof(List<string>).GetMethod("ForEach", new Type[] { typeof(Action<string>) })));
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ret);
            typeDefinition.Methods.Add(method);
            return method;
        }

        private FieldDefinition AddLocalizedPropertyNamesStaticList(TypeDefinition typeDefinition, IEnumerable<string> propertiesNames)
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

            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(OpCodes.Stsfld, field));

            instructions.Add(Instruction.Create(OpCodes.Ret));

            return field;
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
            yield return "System.Runtime";
            yield return "System.Core";
            yield return "netstandard";
            yield return "System.Collections";
            yield return "System.ObjectModel";
            yield return "System.Threading";
            yield return "WindowsBase";
            yield return "CodingSeb.Localization";
        }

        public override bool ShouldCleanReference => true;
    }
}
