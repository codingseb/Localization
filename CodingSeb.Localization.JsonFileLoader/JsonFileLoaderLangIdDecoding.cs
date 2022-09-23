namespace CodingSeb.Localization.JsonFileLoader
{
    /// <summary>
    /// Specify the way a <see cref="JsonFileLoader"/> decode the LangId
    /// </summary>
    public enum JsonFileLoaderLangIdDecoding
    {
        /// <summary>
        /// Default value. The leaf Json node is interpreted as the LangId
        /// </summary>
        JsonNodeLeafKey,

        /// <summary>
        /// Take the part of the filename just before .loc.json as the LangId
        /// <para>
        /// Example : MyTranslationFile.en.loc.json
        /// </para>
        /// </summary>
        InFileNameBeforeExtension,

        /// <summary>
        /// The directory name define the LangId
        /// </summary>
        DirectoryName
    }
}
