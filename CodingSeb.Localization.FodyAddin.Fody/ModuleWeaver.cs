using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodingSeb.Localization.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            List<string> typeDefinitions = ModuleDefinition.
                Types
                .Where(td => td.IsClass)
                .Select(td => td.Name + ";"+ string.Join(";", td.Properties.Select(p => string.Join(";", p.CustomAttributes.Select(a => a.AttributeType.Name)))))
                .ToList();

            ModuleDefinition
                .Types
                .Where(typeDefinition =>
                    typeDefinition.IsClass &&
                    typeDefinition.Properties.Any(property => property.CustomAttributes.Any(attribute => attribute.AttributeType.Name.Equals("LocalizeAttribute"))))
                .ToList()
                .ForEach(typeDefinition =>
                {
                    int i = 0;
                    Debug.WriteLine(typeDefinition.Name);
                });
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
        }

        public override bool ShouldCleanReference => true;
    }
}
