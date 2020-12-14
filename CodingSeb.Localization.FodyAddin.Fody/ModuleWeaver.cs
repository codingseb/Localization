using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CodingSeb.Localization.FodyAddin.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public MethodReference DebuggerNonUserCodeAttributeConstructor;

        public override void Execute()
        {
            ModuleDefinition
                .Types
                .Where(typeDefinition =>
                    typeDefinition.IsClass &&
                    typeDefinition.IsINotifyPropertyChanged() &&
                    typeDefinition.Properties.Any(property => property.CustomAttributes.Any(attribute => attribute.AttributeType.Name.Equals("LocalizeAttribute"))))
                .ToList()
                .ForEach(typeDefinition =>
                {
                    AddHelloWorld(typeDefinition);
                });
        }

        void AddHelloWorld(TypeDefinition typeDefinition)
        {
            var method = new MethodDefinition("World", MethodAttributes.Public, TypeSystem.StringReference);
            ILProcessor processor = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            processor.Append(Instruction.Create(OpCodes.Ldstr, "Hello World"));
            processor.Append(Instruction.Create(OpCodes.Ret));
            typeDefinition.Methods.Add(method);
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
        }

        public override bool ShouldCleanReference => true;
    }
}
