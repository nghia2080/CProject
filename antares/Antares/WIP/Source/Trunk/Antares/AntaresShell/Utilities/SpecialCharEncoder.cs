namespace AntaresShell.Utilities
{
    /// <summary>
    /// Provide encoder/decoder for special char.
    /// </summary>
    public class SpecialCharEncoder
    {
        /// <summary>
        /// Encoding new line character.
        /// </summary>
        /// <param name="originalString">Original string.</param>
        /// <returns>Encoded string.</returns>
        public static string EncodingSpecialCharacters(string originalString)
        {
            originalString = originalString.Replace("\'", "___");
            originalString = originalString.Replace("\"", "\'");
            return originalString.Replace("\r\n", "  ");
        }

        /// <summary>
        /// Decoding new line character.
        /// </summary>
        /// <param name="encodedString">Encoded string.</param>
        /// <returns>Original string.</returns>
        public static string DecodingSpecialCharacters(string encodedString)
        {
            encodedString = encodedString.Replace("  ", "\r\n");
            encodedString = encodedString.Replace("\'", "\"");
            return encodedString.Replace("___", "\'");
        }
    }
}
