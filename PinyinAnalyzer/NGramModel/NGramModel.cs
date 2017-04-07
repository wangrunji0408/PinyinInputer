using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	/// <summary>
	/// 用字典实现的通用N-Gram模型
	/// </summary>
	public class NGramModel: NGramModelBase
	{
		[JsonProperty]
		public int N { get; }
		[JsonProperty]
		Dictionary<string, Distribute<char>> dict = new Dictionary<string, Distribute<char>>();

		public NGramModel(int n)
		{
			if (n <= 0)
				throw new ArgumentException();
			N = n;
		}

		public override Distribute<char> GetDistribute(string pre)
		{
			return dict.GetOrDefault(pre.LastSubString(N - 1, '^'));
		}
		public override void FromStatistician(TextStatistician stat)
		{
		    base.FromStatistician(stat);
			var statByChar = new Dictionary<string, Statistic<char>>();
			foreach (var pair in stat.StringFrequency)
			{
				if (pair.Key.Length != N)
					continue;
				string pre = pair.Key.Substring(0, N-1);
				char c2 = pair.Key[N-1];
				int freq = pair.Value;
				statByChar.GetOrAddDefault(pre).Add(c2, freq);
			}
			dict = statByChar.ToDictionary(pair => pair.Key, pair => pair.Value.ToDistribute());
		}
	}
}
