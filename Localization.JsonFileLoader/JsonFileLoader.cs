using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodingSeb.Localization.Loaders
{
    /// <summary>
    /// This class allow to load localizations from "*.loc.json" files
    /// </summary>
    public class JsonFileLoader : ILocalizationFileLoader
    {
        /// <summary>
        /// The separator used to concat the path of the label when structured translations are used
        /// By default "."
        /// </summary>
        public string LabelPathSeparator { get; set; } = ".";

        /// <summary>
        /// The root (prefix) used for all labels
        /// By default string.Empty
        /// </summary>
        public string LabelPathRootPrefix { get; set; } = string.Empty;

        /// <summary>
        /// The leaves (suffix) used for all labels
        /// By default string.Empty
        /// </summary>
        public string LabelPathSuffix { get; set; } = string.Empty;

        /// <summary>
        /// To define how is decoded the LangId of a translation.<para/>
        /// Default value : <see cref="JsonFileLoaderLangIdDecoding.LeafNodeKey"/>
        /// </summary>
        public JsonFileLoaderLangIdDecoding LangIdDecoding { get; set; }

        /// <summary>
        /// Test if the specified file is loadable by this Loader
        /// </summary>
        /// <param name="fileName">The filename to test</param>
        /// <returns><c>true</c> if it can load the file, <c>false</c> otherwise</returns>
        public bool CanLoadFile(string fileName)
        {
            return fileName.TrimEnd().EndsWith(".loc.json", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Load all translation defined in Json format in the specified file
        /// </summary>
        /// <param name="fileName">The file we want to load</param>
        /// <param name="loader">The loader to load each translation</param>
        public void LoadFile(string fileName, LocalizationLoader loader)
        {
            LoadFromString(File.ReadAllText(fileName), loader, fileName);
        }

        /// <summary>
        /// Load all translations defined in Json format from the specified <paramref name="jsonString"/>.
        /// </summary>
        /// <param name="jsonString">String to load serialized Json format translations from.</param>
        /// <param name="loader">The loader to use for loading translations from the string.</param>
        /// <param name="sourceFileName">Optional source file name.</param>
        public void LoadFromString(string jsonString, LocalizationLoader loader, string sourceFileName = "")
        {
            JObject root = (JObject)JsonConvert.DeserializeObject(jsonString);

            root.Properties().ToList()
                .ForEach(property => ParseSubElement(property, new Stack<string>(), loader, sourceFileName));
        }

        private void ParseSubElement(JProperty property, Stack<string> textId, LocalizationLoader loader, string source)
        {
            switch (property.Value.Type)
            {
            case JTokenType.Object:
                textId.Push(property.Name);
                ((JObject)property.Value).Properties().ToList()
                    .ForEach(subProperty => ParseSubElement(subProperty, textId, loader, source));
                textId.Pop();
                break;
            case JTokenType.String:

                if (LangIdDecoding == JsonFileLoaderLangIdDecoding.InFileNameBeforeExtension)
                {
                    textId.Push(property.Name);
                    loader.AddTranslation(
                        LabelPathRootPrefix + string.Join(LabelPathSeparator, textId.Reverse()) + LabelPathSuffix,
                        Path.GetExtension(Regex.Replace(source, @"\.loc\.json", "")).Replace(".", ""),
                        property.Value.ToString(),
                        source);
                    textId.Pop();
                }
                else if (LangIdDecoding == JsonFileLoaderLangIdDecoding.DirectoryName)
                {
                    textId.Push(property.Name);
                    loader.AddTranslation(LabelPathRootPrefix + string.Join(LabelPathSeparator, textId.Reverse()) + LabelPathSuffix,
                        Path.GetDirectoryName(source),
                        property.Value.ToString(),
                        source);
                    textId.Pop();
                }
                else
                {
                    loader.AddTranslation(LabelPathRootPrefix + string.Join(LabelPathSeparator, textId.Reverse()) + LabelPathSuffix, property.Name, property.Value.ToString(), source);
                }
                break;
            default:
                throw new FormatException($"Invalid format in Json language file for property [{property.Name}]");
            }
        }
    }
}
