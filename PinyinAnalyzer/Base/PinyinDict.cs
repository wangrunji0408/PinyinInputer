using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace PinyinAnalyzer
{
    public class PinyinDict
    {
		Dictionary<char, List<string>> _charToPinyins = new Dictionary<char, List<string>>();
		Dictionary<string, SortedSet<char>> _pinyinToChars = new Dictionary<string, SortedSet<char>>();

		public List<string> GetPinyins(char c)
        {
			return _charToPinyins.GetOrDefault(c);
        }

		public ISet<char> GetChars(string pinyin)
		{
			return _pinyinToChars.GetOrDefault(pinyin);
		}

		public void Add(char c, string pinyin)
		{
			_pinyinToChars.GetOrAddDefault(pinyin).Add(c);
			_charToPinyins.GetOrAddDefault(c).Add(pinyin);
		}

        public void Load(string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
			{
                var tokens = line.Split();
                var pinyin = tokens[0];
				foreach (var s in tokens.Skip(1))
					Add(s[0], pinyin);
            }
        }

		public PinyinDict() { }
		public PinyinDict(string filePath)
		{
			Load(filePath);
		}
    }
}