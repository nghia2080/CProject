using SearchEngine.Interfaces;
using System;
using System.Collections.Generic;

namespace SearchEngine
{
    /// <summary>
    /// A searcher class which does the search and returns the matches.
    /// </summary>
    public class NGramSearcherM1 : WordSearcher
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// A function to calculate the distance (differences) between words.
        /// </summary>
        private readonly Metric _metric;

        /// <summary>
        /// The maximum allowed distance.
        /// </summary>
        private readonly int _maxDistance;

        /// <summary>
        /// True if a word used in comparison is a prefix pattern.
        /// </summary>
        private readonly bool _prefix;

        /// <summary>
        /// The list of words.
        /// </summary>
        private readonly string[] _dictionary;

        /// <summary>
        /// The base alphabet.
        /// </summary>
        private readonly IAlphabet _alphabet;

        /// <summary>
        /// The index table.
        /// </summary>
        private readonly int[][][] _ngramMap;

        /// <summary>
        /// The length of the n-gram.
        /// </summary>
        private readonly int _n;

        /// <summary>
        /// The maximum length of a word.
        /// </summary>
        private readonly int _maxLength;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the NGramSearcherM1 class.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="metric"></param>
        /// <param name="maxDistance"></param>
        /// <param name="prefix"></param>
        public NGramSearcherM1(NGramIndexM1 index, Metric metric, int maxDistance, bool prefix)
            : base(index)
        {
            _metric = metric;
            _maxDistance = maxDistance;
            _prefix = prefix;
            _dictionary = index.Dictionary;
            _alphabet = index.Alphabet;
            _ngramMap = index.NGramMap;
            _n = index.N;
            _maxLength = index.MaxLength;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Searches matches of a specific query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A set contains the indices of each matches of the query.</returns>
        public override IEnumerable<int> Search(string query)
        {
            var words = query.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var returned = new HashSet<int>();
            foreach (var word in words)
            {
                returned.UnionWith(SearchSingleWord(word));
            }
            return returned;
        }

        private IEnumerable<int> SearchSingleWord(string word)
        {
            var set = new HashSet<int>();

            var stringLength = Math.Min(word.Length, _maxLength);

            for (int i = 0; i < stringLength - _n + 1; ++i)
            {
                var ngram = NGramIndexerM1.GetNGram(_alphabet, word, i, _n);

                for (var j = Math.Max(0, i - _maxDistance); j <= Math.Min(stringLength - _n, i + _maxDistance); ++j)
                {
                    var dictIndexes = _ngramMap[ngram][j];

                    if (dictIndexes == null) continue;
                    foreach (var k in dictIndexes)
                    {
                        var distance = _metric.GetDistance(_dictionary[k], word, _maxDistance, _prefix);
                        if (distance <= _maxDistance) set.Add(k);
                    }
                }
            }

            return set;
        } 

        #endregion
    }

}
