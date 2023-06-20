using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        #region Attributes

        private const MethodAttributes staticConstructorAttributes =
            MethodAttributes.Private |
            MethodAttributes.HideBySig |
            MethodAttributes.Static |
            MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName;

        private const MethodAttributes destructorAttributes =
            MethodAttributes.Family |
            MethodAttributes.Virtual |
            MethodAttributes.HideBySig;

        #endregion

        #region References

        private TypeReference weakEventManagerType;
        private TypeReference locType;
        private MethodReference locGetInstance;
        private TypeReference currentLanguageChangedEventArgsType;
        private MethodReference currentLanguageChangedEventHandler;
        private TypeReference genericEventManagerType;
        private MethodReference addHandlerMethod;
        private MethodReference genericAddHandlerMethod;
        private MethodReference removeHandlerMethod;
        private MethodReference genericRemoveHandlerMethod;

        private void InitReferences()
        {
            weakEventManagerType = ModuleDefinition.ImportReference(FindTypeDefinition("System.Windows.WeakEventManager`2"));
            locType = ModuleDefinition.ImportReference(FindTypeDefinition("CodingSeb.Localization.Loc"));
            locGetInstance = ModuleDefinition.ImportReference(typeof(Loc).GetProperty("Instance").GetGetMethod());
            currentLanguageChangedEventArgsType = ModuleDefinition.ImportReference(FindTypeDefinition("CodingSeb.Localization.CurrentLanguageChangedEventArgs"));
            currentLanguageChangedEventHandler = ModuleDefinition.ImportReference(typeof(EventHandler<CurrentLanguageChangedEventArgs>).GetConstructors()[0]);
            genericEventManagerType = ModuleDefinition.ImportReference(weakEventManagerType.MakeGenericInstanceType(locType, currentLanguageChangedEventArgsType));
            addHandlerMethod = ModuleDefinition.ImportReference(genericEventManagerType.Resolve().Methods.FirstOrDefault(m => m.Name.Equals("AddHandler")));
            genericAddHandlerMethod = ModuleDefinition.ImportReference(addHandlerMethod.MakeHostInstanceGeneric(locType, currentLanguageChangedEventArgsType));
            removeHandlerMethod = ModuleDefinition.ImportReference(genericEventManagerType.Resolve().Methods.FirstOrDefault(m => m.Name.Equals("RemoveHandler")));
            genericRemoveHandlerMethod = ModuleDefinition.ImportReference(removeHandlerMethod.MakeHostInstanceGeneric(locType, currentLanguageChangedEventArgsType));
        }

        #endregion

        public override void Execute()
        {
            InitReferences();

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

            propertyToLocalize.ToList().ForEach(EventuallyInjectPropertyCode);

            var propertyListFieldDefinition = AddLocalizedPropertyNamesStaticList(typeDefinition, propertyToLocalize.Select(p => p.Name));
            var triggerPropertyChangedMethod = typeDefinition.FindPropertyChangedTriggerMethod();
            var currentLanguageChangedMethod = AddCurrentLanguageChangedMethod(typeDefinition, propertyListFieldDefinition, triggerPropertyChangedMethod);

            SubscribeToLanguageChangedInConstructors(typeDefinition, currentLanguageChangedMethod);
            UnSubscribeFromLanguageChangedInDestructors(GetOrCreateFinalizer(typeDefinition), currentLanguageChangedMethod);
        }

        private void EventuallyInjectPropertyCode(PropertyDefinition property)
        {
            var attribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name.Equals("LocalizeAttribute"));

            if (attribute.ConstructorArguments.Count > 0)
            {
                var textId = attribute.ConstructorArguments[0].Value.ToString();

                var instructions = property.GetMethod.Body.Instructions;

                instructions.Clear();

                instructions.Add(Instruction.Create(OpCodes.Ldstr, textId));

                if (attribute.Properties.FirstOrDefault(p => p.Name.Equals("DefaultValue")) is CustomAttributeNamedArgument defaultValueProperty
                    && defaultValueProperty.Argument.Value != null)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldstr, defaultValueProperty.Argument.Value.ToString()));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldnull));
                }

                instructions.Add(Instruction.Create(OpCodes.Ldnull));
                instructions.Add(Instruction.Create(OpCodes.Call, ModuleDefinition.ImportReference(typeof(Loc).GetMethod("Tr", new Type[] { typeof(string), typeof(string), typeof(string) }))));
                instructions.Add(Instruction.Create(OpCodes.Ret));
            }
        }

        private void SubscribeToLanguageChangedInConstructors(TypeDefinition typeDefinition, MethodDefinition currentLanguageChangedMethod)
        {
            List<MethodDefinition> exclusiveAlwaysCalledConstructors = typeDefinition.GetConstructors().Where(constructor =>
            {
                return !constructor.IsStatic &&
                    constructor.Body.Instructions.Count > 2 &&
                    !constructor.Body.Instructions.Any(i => i.OpCode == OpCodes.Call
                        && i.Operand is MethodReference methodReference
                        && methodReference.Resolve().IsConstructor
                        && methodReference.DeclaringType == typeDefinition);
            }).ToList();

            string locPropertyName = typeDefinition.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType.Name.Equals("LocPropertyAttribute"))?.ConstructorArguments[0].Value?.ToString();
            var locProperty = typeDefinition.Properties.FirstOrDefault(property => property.Name.Equals(locPropertyName) && property.PropertyType.FullName.Equals(locType.FullName));

            string locFieldName = typeDefinition.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType.Name.Equals("LocFieldAttribute"))?.ConstructorArguments[0].Value?.ToString();
            var locField = typeDefinition.Fields.FirstOrDefault(field => field.Name.Equals(locFieldName) && field.FieldType.FullName.Equals(locType.FullName));

            exclusiveAlwaysCalledConstructors.ForEach(constructor =>
            {
                var instructions = constructor.Body.Instructions;

                if (instructions.LastOrDefault(i => i.OpCode == OpCodes.Ret) is Instruction instruction)
                {
                    instructions.Remove(instruction);
                }

                instructions.Add(Instruction.Create(OpCodes.Nop));

                if (locProperty  != null)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    instructions.Add(Instruction.Create(OpCodes.Call, locProperty.GetMethod));
                }
                else if (locField != null)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    instructions.Add(Instruction.Create(OpCodes.Ldfld, locField));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Call, locGetInstance));
                }

                instructions.Add(Instruction.Create(OpCodes.Ldstr, "CurrentLanguageChanged"));
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Ldftn, currentLanguageChangedMethod));
                instructions.Add(Instruction.Create(OpCodes.Newobj, currentLanguageChangedEventHandler));
                instructions.Add(Instruction.Create(OpCodes.Call, genericAddHandlerMethod));
                instructions.Add(Instruction.Create(OpCodes.Nop));
                instructions.Add(Instruction.Create(OpCodes.Ret));
            });
        }

        private void UnSubscribeFromLanguageChangedInDestructors(MethodDefinition finalizer, MethodDefinition currentLanguageChangedMethod)
        {
            var instructions = finalizer.Body.Instructions;
            int index = 2;

            instructions.Insert(index++, Instruction.Create(OpCodes.Call, locGetInstance));
            instructions.Insert(index++, Instruction.Create(OpCodes.Ldstr, "CurrentLanguageChanged"));
            instructions.Insert(index++, Instruction.Create(OpCodes.Ldarg_0));
            instructions.Insert(index++, Instruction.Create(OpCodes.Ldftn, currentLanguageChangedMethod));
            instructions.Insert(index++, Instruction.Create(OpCodes.Newobj, currentLanguageChangedEventHandler));
            instructions.Insert(index++, Instruction.Create(OpCodes.Call, genericRemoveHandlerMethod));
            instructions.Insert(index++, Instruction.Create(OpCodes.Nop));
        }

        private MethodDefinition AddCurrentLanguageChangedMethod(TypeDefinition typeDefinition, FieldDefinition propertyListFieldDefinition, MethodDefinition triggerPropertyChangedMethod)
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

        private MethodDefinition GetOrCreateFinalizer(TypeDefinition typeDefinition)
        {
            MethodDefinition finalizer = typeDefinition.Methods.FirstOrDefault(m => m.Name.Equals("Finalize"));

            if (finalizer == null)
            {
                MethodReference parentFinalizer = ModuleDefinition.ImportReference(typeDefinition.BaseType.Resolve().FindNearestFinalizeParentMethod());
                finalizer = new MethodDefinition("Finalize", destructorAttributes, TypeSystem.VoidDefinition);

                var il = finalizer.Body.GetILProcessor();

                Instruction retInstruction = Instruction.Create(OpCodes.Ret);

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Leave_S, retInstruction);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, parentFinalizer);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Endfinally);
                il.Append(retInstruction);

                typeDefinition.Methods.Add(finalizer);
            }

            return finalizer;
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
            yield return "System.Runtime";
            yield return "System.Core";
            yield return "System.Collections";
            yield return "System.ObjectModel";
            yield return "System.Threading";
            yield return "WindowsBase";
            yield return "CodingSeb.Localization";
        }

        public override bool ShouldCleanReference => true;
    }
}
