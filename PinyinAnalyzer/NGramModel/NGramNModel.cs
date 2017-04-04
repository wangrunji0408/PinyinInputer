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
	public class NGramNModel: NGramModelBase
	{
		[JsonProperty]
		Dictionary<string, Distribute<char>> dict = new Dictionary<string, Distribute<char>>();

	    public Func<IEnumerable<Distribute<char>>, Distribute<char>> MixDistributeStrategy { get; set; }
	        = NGramBindModel.MixDistributeStrategy_Lambda;

		public override Distribute<char> GetDistribute(Condition condition)
		{
			var list = new List<Distribute<char>>();
			for (int n = 0; ; ++n)
			{
				var dtb = dict.GetOrDefault(condition.Chars.LastSubString(n, ' '));
				if (condition.Pinyin != null)
					dtb = dtb.Where(c => PinyinDict.GetPinyins(c).Contains(condition.Pinyin));
				if (dtb.IsEmpty)
					break;
				list.Add(dtb);
			}
			//Console.WriteLine($"{condition} {list.Count-1}");
		    return MixDistributeStrategy(list);
		}
		public override Distribute<char> GetDistribute(string pre)
		{
			throw new NotImplementedException();
		}
		public override void FromStatistician(TextStatistician stat)
		{
			var statByPre = new Dictionary<string, Statistic<char>>();
			foreach (var pair in stat.StringFrequency)
			{
				string str = pair.Key;
				string pre = str.Substring(0, str.Length - 1);
				char c = str[str.Length - 1];
				int freq = pair.Value;
				statByPre.GetOrAddDefault(pre).Add(c, freq);
			}
			dict = statByPre.ToDictionary(pair => pair.Key, pair => pair.Value.ToDistribute());
		}
	}
}
