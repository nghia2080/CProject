using System;
using AntaresShell.Logger;
using Windows.Data.Xml.Dom;

namespace AntaresShell.Utilities
{
    /// <summary>
    /// Support xml document to load xml file.
    /// </summary>
    public static class XmlDocumentHelper
    {
        /// <summary>
        /// Load Xml document from string.
        /// </summary>
        /// <param name="document">Target object.</param>
        /// <param name="xmlString">String to load.</param>
        /// <returns>Loaded Xml document.</returns>
        public static bool LoadFromString(this XmlDocument document, string xmlString)
        {
            if (xmlString.Length <= 0)
            {
                return false;
            }

            xmlString = SpecialCharEncoder.EncodingSpecialCharacters(xmlString);

            if (xmlString.Contains("<?"))
            {
                xmlString = xmlString.Remove(0, xmlString.IndexOf("?>", StringComparison.Ordinal) + 2);
            }

            try
            {
                document.LoadXml(
                    xmlString, 
                    new XmlLoadSettings
                    {
                        ProhibitDtd = false,
                        ResolveExternals = true,
                        ElementContentWhiteSpace = false
                    });

                return true;
            }
            catch (Exception exception)
            {
                LogManager.Instance.LogException(exception.ToString());
                return false;
            }
        }
    }
}