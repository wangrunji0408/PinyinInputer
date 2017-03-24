using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class Statistic
	{
		[JsonProperty("total")]
		public int Total { get; private set; }
		[JsonProperty("dict")]
		readonly IDictionary<char, int> _dict = new Dictionary<char, int>();
		public IEnumerable<KeyValuePair<char, int>> CharCountDescending =>
			from pair in _dict
			orderby pair.Value descending
			select pair;

		public void Add(char c, int count = 1)
		{
			if (_dict.ContainsKey(c))
				_dict[c] += count;
			else
				_dict.Add(c, count);
			Total += count;
		}

		public int GetCount(char c)
		{
			int ans = 0;
			_dict.TryGetValue(c, out ans);
			return ans;
		}

		public char First()
		{
			return CharCountDescending.First().Key;
		}

		public Distribute<char> ToDistribute()
		{
			return new Distribute<char>(_dict.Select(pair => 
			       		new KeyValuePair<char, float>(pair.Key, (float)pair.Value / Total)));
		}
	}
}
