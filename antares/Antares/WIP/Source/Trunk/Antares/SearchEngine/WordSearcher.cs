using SearchEngine.Interfaces;
using System.Collections.Generic;

namespace SearchEngine
{
    public abstract class WordSearcher : ISearcher
    {
        protected WordSearcher(WordIndexBase index)
        {
            _index = index;
        }

        private readonly WordIndexBase _index;
        public WordIndexBase Index
        {
            get { return _index; }
        }

        public abstract IEnumerable<int> Search(string word);
    }
}

