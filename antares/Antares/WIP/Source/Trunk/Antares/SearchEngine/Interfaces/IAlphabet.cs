namespace SearchEngine.Interfaces
{
    // Serializable
    /// <summary>
    /// An interface for alphabet classes.
    /// </summary>
    public interface IAlphabet
    {
        /// <summary>
        /// Gets the index of a character in the alphabet.
        /// </summary>
        /// <param name="ch">The specified character.</param>
        /// <returns>An integer denotes the 0-based index of that character in the alphabet
        /// -or- negative 1 if the character does not exist in the alphabet.</returns>
        int MapChar(char ch);

        /// <summary>
        /// Gets the list of all characters in the alphabet.
        /// </summary>
        /// <returns>An array contains all characters in the alphabet.</returns>
        char[] Chars();

        /// <summary>
        /// Gets the alphabet size.
        /// </summary>
        /// <returns>An integer denotes the total number of characters in the alphabet.</returns>
        int Size();
    }
}
