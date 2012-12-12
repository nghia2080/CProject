using System.Linq;
using SearchEngine.Interfaces;
using System;

namespace SearchEngine
{
    /// <summary>
    /// An indexer class which index words using the first modification of n-gram technique.
    /// </summary>
    public class NGramIndexerM1 : IIndexer
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// WTF!!??
        /// </summary>
        private const int LENGTH_FACTOR = 4;

        /// <summary>
        /// The default length on a n-gram.
        /// </summary>
        private const int DEFAULT_N = 2;

        /// <summary>
        /// The base alphabet.
        /// </summary>
        private readonly IAlphabet _alphabet;

        /// <summary>
        /// The length of a n-gram.
        /// </summary>
        private readonly int _n;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the NGramIndexerM1 class.
        /// </summary>
        /// <param name="alphabet">The base alphabet.</param>
        public NGramIndexerM1(IAlphabet alphabet)
        {
            _alphabet = alphabet;
            _n = DEFAULT_N;
        }

        /// <summary>
        /// Initializes a new instance of the NGramIndexerM1 class.
        /// </summary>
        /// <param name="alphabet">The base alphabet.</param>
        /// <param name="n">The length of a n-gram.</param>
        public NGramIndexerM1(IAlphabet alphabet, int n)
        {
            _alphabet = alphabet;
            _n = n;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Creates an index table of the dictionary.
        /// </summary>
        /// <param name="dictionary">The list of words to search in.</param>
        /// <returns>Returns a new instance of a class devired from IIndex interface with the index table generated.</returns>
        public WordIndexBase CreateIndex(string[] dictionary)
        {
            // Gets the length of the longest words in the dictionary.
            int maxLength = dictionary.Select(word => word.Length).Concat(new[] {0}).Max();

            // WTF!!??
            maxLength = Math.Min(maxLength, _n * LENGTH_FACTOR);

            // Calculates the total number of possibles n-gram. Table length = alphabet size ^ n-gram length.
            // To be able to support Japanese, which contains thousands of characters, we can only use n = 2.
            int mapLength = 1;
            for (int i = 0; i < _n; ++i)
                mapLength *= _alphabet.Size();
            // Creates a map to hold the frequency of each n-gram.
            // ngramCountMap[ng][k] = the number of times the n-gram ng appears at position k on any string.
            var ngramCountMap = new int[mapLength][];

            // Processes all words.
            foreach (var word in dictionary)
            {
                int wordLength = Math.Min(word.Length, maxLength);
                for (int k = 0; k < wordLength - _n + 1; ++k)
                {
                    // Gets the index of the n-gram.
                    int ngram = GetNGram(_alphabet, word, k, _n);
                    if (ngramCountMap[ngram] == null) ngramCountMap[ngram] = new int[maxLength - _n + 1];
                    ++ngramCountMap[ngram][k];
                }
            }

            // ngramMap[ng][k][j] = the index of a word which n-gram ng appears at position k.
            var ngramMap = new int[mapLength][][];

            // Processes all words.
            for (int i = 0; i < dictionary.Length; ++i)
            {
                string word = dictionary[i];
                int wordLength = Math.Min(word.Length, maxLength);
                for (int k = 0; k < wordLength - _n + 1; ++k)
                {
                    // Gets the index of the n-gram.
                    int ngram = GetNGram(_alphabet, word, k, _n);
                    if (ngramMap[ngram] == null) ngramMap[ngram] = new int[maxLength - _n + 1][];
                    if (ngramMap[ngram][k] == null) ngramMap[ngram][k] = new int[ngramCountMap[ngram][k]];
                    ngramMap[ngram][k][--ngramCountMap[ngram][k]] = i;
                }
            }
            return new NGramIndexM1(dictionary, _alphabet, ngramMap, _n, maxLength);
        }

        /// <summary>
        /// Gets the index of a n-gram given the base alphabet.
        /// </summary>
        /// <param name="alphabet">The base alphabet</param>
        /// <param name="str">The string which contains the n-gram.</param>
        /// <param name="start">The start position of the n-gram.</param>
        /// <param name="n">The length of the n-gram.</param>
        /// <returns>An integer denotes the index of the n-gram with the given alphabet.</returns>
        public static int GetNGram(IAlphabet alphabet, string str, int start, int n)
        {
            int ngram = 0;
            for (int i = start; i < start + n; ++i)
                ngram = ngram * alphabet.Size() + alphabet.MapChar(str[i]);
            return ngram;
        }

        #endregion
    }
}
