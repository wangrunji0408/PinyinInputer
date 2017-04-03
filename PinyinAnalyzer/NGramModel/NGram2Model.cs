using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class NGram2Model: NGramModelBase
	{
		[JsonProperty]
		Dictionary<char, Distribute<char>> dict = new Dictionary<char, Distribute<char>>();
		// char - distribute

		public override Distribute<char> GetDistribute(string pre)
		{
			return dict.GetOrDefault(pre.LastSubString(1, '^')[0]);
		}
		public override void FromStatistician(TextStatistician stat)
		{
			var statByChar = new Dictionary<char, Statistic<char>>();
			foreach (var pair in stat.StringFrequency)
			{
				if (pair.Key.Length != 2)
					continue;
				char c0 = pair.Key[0];
				char c1 = pair.Key[1];
				int freq = pair.Value;
			    statByChar.GetOrAddDefault(c0).Add(c1, freq);
			}
			dict = statByChar.ToDictionary(pair => pair.Key, pair => pair.Value.ToDistribute());
		}
	}
}
