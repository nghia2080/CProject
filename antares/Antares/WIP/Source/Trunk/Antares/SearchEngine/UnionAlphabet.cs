using System.Linq;
using SearchEngine.Interfaces;

namespace SearchEngine
{
    public class UnionAlphabet : IAlphabet
    {
        private readonly IAlphabet[] _alphabets;
        private readonly char[] _chars;

        public UnionAlphabet(params IAlphabet[] alphabets)
        {
            _alphabets = alphabets;

            int charsLength = alphabets.Aggregate(0, (current, alphabet) => current + alphabet.Size());

            _chars = new char[charsLength];

            var index = 0;
            foreach (var alphabet in alphabets)
                foreach (var ch in alphabet.Chars())
                    _chars[index++] = ch;
        }

        public int MapChar(char ch)
        {
            var index = -1;
            if (_alphabets.Any(alphabet => (index = alphabet.MapChar(ch)) >= 0))
            {
                return index;
            }
            return -1;
        }

        public char[] Chars()
        {
            return _chars;
        }

        public int Size()
        {
            return _chars.Length;
        }
    }
}
