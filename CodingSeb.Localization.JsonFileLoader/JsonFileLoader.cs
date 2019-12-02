using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodingSeb.Localization.Loaders
{
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
        /// Test sif the specified file is loadable by this Loader
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
            using(StreamReader reader = File.OpenText(fileName))
            {
                JObject root = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                root.Properties().ToList()
                    .ForEach(property => ParseSubElement(property, new Stack<string>(), loader, fileName));
            }
        }

        private void ParseSubElement(JProperty property, Stack<string> textId, LocalizationLoader loader, string fileName)
        {
            switch(property.Value.Type)
            {
                case JTokenType.Object:
                    textId.Push(property.Name);
                    ((JObject)property.Value).Properties().ToList()
                        .ForEach(subProperty => ParseSubElement(subProperty, textId, loader, fileName));
                    textId.Pop();
                    break;
                case JTokenType.String:
                    loader.AddTranslation(LabelPathRootPrefix + string.Join(LabelPathSeparator, textId.Reverse()) + LabelPathSuffix, property.Name, property.Value.ToString(), fileName);
                    break;
                default:
                    throw new FormatException($"Invalid format in Json language file for property [{property.Name}]");
            }
        }
    }
}
