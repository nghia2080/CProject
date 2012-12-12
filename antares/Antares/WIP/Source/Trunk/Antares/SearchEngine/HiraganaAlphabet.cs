namespace SearchEngine
{
    /// <summary>
    /// An English alphabet. Does not contain uppercase letters and digits.
    /// </summary>
    public class HiraganaAlphabet : SimpleAlphabet
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the EnglishAlphabet class.
        /// </summary>
        public HiraganaAlphabet()
            : base('ぁ', 'ゟ') { }

        #endregion
    }
}
