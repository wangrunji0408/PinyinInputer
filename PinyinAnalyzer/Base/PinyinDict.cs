using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace PinyinAnalyzer
{
	public class PinyinDict
	{
		IDictionary<char, List<string>> _charToPinyins = new Dictionary<char, List<string>>();
		IDictionary<string, SortedSet<char>> _pinyinToChars = new Dictionary<string, SortedSet<char>>();
		ICollection<string> Pinyins => _pinyinToChars.Keys;

		public ICollection<string> GetPinyins(char c)
		{
			return _charToPinyins.GetOrDefault(c);
		}

		public IEnumerable<string> GetPinyinsStartsWith(string str)
		{
			return Pinyins.Where(py => py.StartsWith(str));
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

		static Lazy<PinyinDict> lazyInstance = new Lazy<PinyinDict>(() =>
		    new PinyinDict(GlobalConfig.DefaultPinyinFile));
		public static PinyinDict Instance => lazyInstance.Value;
	}
}