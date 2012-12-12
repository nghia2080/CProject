using SearchEngine.Interfaces;

namespace SearchEngine
{
    /// <summary>
    /// An index class which stores the n-gram index (first modification).
    /// </summary>
    public class NGramIndexM1 : WordIndexBase
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// The alphabet used as base for indexing. Words which contain a character
        /// that does not exist in this alphabet are not indexed.
        /// </summary>
        private readonly IAlphabet _alphabet;

        /// <summary>
        /// The index table.
        /// </summary>
        private readonly int[][][] _ngramMap;

        /// <summary>
        /// N as in N-gram.
        /// </summary>
        private readonly int _n;

        /// <summary>
        /// The length of the longest word in the dictionary.
        /// </summary>
        private readonly int _maxLength;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the class NGramIndexM1.
        /// </summary>
        /// <param name="dictionary">A list of to-be-indexed words.</param>
        /// <param name="alphabet">The base alphabet. Words which contains characters that not exist in the alphabet are ignored.</param>
        /// <param name="ngramMap">The index table.</param>
        /// <param name="n">The length of the n-gram.</param>
        /// <param name="maxLength">The length of the longest word in the dictionary.</param>
        public NGramIndexM1(string[] dictionary, IAlphabet alphabet, int[][][] ngramMap, int n, int maxLength)
            : base(dictionary)
        {
            _alphabet = alphabet;
            _ngramMap = ngramMap;
            _n = n;
            _maxLength = maxLength;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the base alphabet.
        /// </summary>
        public IAlphabet Alphabet
        {
            get { return _alphabet; }
        }

        /// <summary>
        /// Gets the index table.
        /// </summary>
        public int[][][] NGramMap
        {
            get { return _ngramMap; }
        }

        /// <summary>
        /// Gets the length of the n-gram.
        /// </summary>
        public int N
        {
            get { return _n; }
        }

        /// <summary>
        /// Gets the length of the longest word in the dictionary.
        /// </summary>
        public int MaxLength
        {
            get { return _maxLength; }
        }

        #endregion
    }

}
