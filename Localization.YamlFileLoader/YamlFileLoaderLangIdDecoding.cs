namespace CodingSeb.Localization.Loaders
{
    /// <summary>
    /// Specify the way a <see cref="JsonFileLoader"/> decode the LangId
    /// </summary>
    public enum YamlFileLoaderLangIdDecoding
    {
        /// <summary>
        /// Default value. The YAML leaf node key is interpreted as the LangId
        /// </summary>
        LeafNodeKey,

        /// <summary>
        /// Take the part of the filename just before .loc.yaml as the LangId
        /// <para>
        /// Example : MyTranslationFile.en.loc.yaml
        /// </para>
        /// </summary>
        InFileNameBeforeExtension,

        /// <summary>
        /// The directory name define the LangId
        /// </summary>
        DirectoryName
    }
}
