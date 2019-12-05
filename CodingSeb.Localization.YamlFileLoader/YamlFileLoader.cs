using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace CodingSeb.Localization.Loaders
{
    /// <summary>
    /// This class allow to load localizations from "*.loc.yaml" files
    /// </summary>
    public class YamlFileLoader : ILocalizationFileLoader
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
        /// Test if the specified file is loadable by this Loader
        /// </summary>
        /// <param name="fileName">The filename to test</param>
        /// <returns><c>true</c> if it can load the file, <c>false</c> otherwise</returns>
        public bool CanLoadFile(string fileName)
        {
            return fileName.TrimEnd().EndsWith(".loc.yaml", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Load all translation defined in Json format in the specified file
        /// </summary>
        /// <param name="fileName">The file we want to load</param>
        /// <param name="loader">The loader to load each translation</param>
        public void LoadFile(string fileName, LocalizationLoader loader)
        {
            var input = new StringReader(File.ReadAllText(fileName));
            var yaml = new YamlStream();

            yaml.Load(input);

            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            mapping.ToList()
                .ForEach(pair => ParseSubElement(pair, new Stack<string>(), loader, fileName));
        }

        private void ParseSubElement(KeyValuePair<YamlNode, YamlNode> nodePair, Stack<string> textId, LocalizationLoader loader, string fileName)
        {
            if(nodePair.Value is YamlMappingNode mappingNode)
            {
                textId.Push(nodePair.Key.ToString());
                mappingNode.ToList()
                    .ForEach(pair => ParseSubElement(pair, textId, loader, fileName));
                textId.Pop();
            }
            else
            {
                loader.AddTranslation(LabelPathRootPrefix + string.Join(LabelPathSeparator, textId.Reverse()) + LabelPathSuffix, nodePair.Key.ToString(), nodePair.Value.ToString(), fileName);
            }
        }
    }
}
