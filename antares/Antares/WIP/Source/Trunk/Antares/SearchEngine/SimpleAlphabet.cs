using System.Globalization;
using SearchEngine.Interfaces;

namespace SearchEngine
{
    /// <summary>
    /// A simple alphabet.
    /// </summary>
    public class SimpleAlphabet : IAlphabet
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// The first character of the alphabet.
        /// </summary>
        private readonly char _min;

        /// <summary>
        /// The last character of the alphabet.
        /// </summary>
        private readonly char _max;

        /// <summary>
        /// All characters.
        /// </summary>
        private readonly char[] _chars;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the SimpleAlphabet class.
        /// </summary>
        /// <param name="min">The first character of the alphabet.</param>
        /// <param name="max">The last character of the alphabet.</param>
        public SimpleAlphabet(char min, char max)
        {
            _min = min;
            _max = max;

            // Generates all characters in the alphabet.
            _chars = new char[max - min + 1];
            int index = 0;
            for (char ch = min; ch <= max; ++ch)
                _chars[index++] = ch;
        }

        public SimpleAlphabet(string[] unicode)
        {
            _chars = new char[unicode.Length];
            for (int i = 0; i < unicode.Length; i++)
            {
                _chars[i] = char.ConvertFromUtf32(int.Parse(unicode[i], NumberStyles.HexNumber))[0];
            }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Gets the index of the specified character in the alphabet.
        /// </summary>
        /// <param name="ch">The character to get index.</param>
        /// <returns>Returns an integer denotes the index of the specified character 
        /// -or- negative 1 if the character does not exist in the alphabet.</returns>
        public int MapChar(char ch)
        {
            //if (ch < _min || ch > _max) return -1;
            //return ch - _min;
            for (var i = 0; i < _chars.Length; ++i)
            {
                if (_chars[i] == ch) return i;
            }
            return -1;
        }

        /// <summary>
        /// Gets all characters exist in the alphabet.
        /// </summary>
        /// <returns>Returns an array contains all characters in the alphabet.</returns>
        public char[] Chars()
        {
            return _chars;
        }

        /// <summary>
        /// Gets the size of the alphabet.
        /// </summary>
        /// <returns>Returns an integer denotes the size of the alphabet.</returns>
        public int Size()
        {
            return _chars.Length;
        }

        #endregion
    }

}
