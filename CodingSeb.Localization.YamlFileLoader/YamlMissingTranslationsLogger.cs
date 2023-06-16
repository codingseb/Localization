using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace CodingSeb.Localization
{
    /// <summary>
    /// -- Describe here to what is this class used for. (What is it's purpose) --
    /// </summary>
    public static class YamlMissingTranslationsLogger
    {
        public static void EnableLog()
        {
            Loc.LogOutMissingTranslations = true;
            Loc.MissingTranslationFound += Loc_MissingTranslationFound;
        }

        public static void DisableLogFor(Loc loc)
        {
            Loc.MissingTranslationFound -= Loc_MissingTranslationFound;
        }

        public static string MissingTranslationsFileName { get; set; } = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "LocalizationMissingTranslations.yaml");

        private static void Loc_MissingTranslationFound(object sender, LocalizationMissingTranslationEventArgs e)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(e.MissingTranslations);

            File.WriteAllText(MissingTranslationsFileName, yaml);
        }
    }
}
