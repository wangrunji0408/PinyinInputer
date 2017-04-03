//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.IO;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace PinyinAnalyzer
//{
//	[Obsolete]
//	public class NGram2Model_DictDict: NGramModelBase
//	{
//		[JsonProperty]
//		Dictionary<char, Dictionary<string, Distribute<char>>> dict
//			= new Dictionary<char, Dictionary<string, Distribute<char>>>();
//		// char - pinyin - distribute

//		public override Distribute<char> GetDistribute(Condition condition)
//		{
//			return dict.GetOrDefault(condition.Chars.LastSubString(1, '^')[0])
//				       .GetOrDefault(condition.Pinyin);
//		}
//		public override void FromStatistician(TextStatistician stat)
//		{
//			var statByPinyinByChar = new Dictionary<char, Dictionary<string, Statistic<char>>>();
//			foreach (var pair in stat.StringFrequency)
//			{
//				if (pair.Key.Length != 2)
//					continue;
//				char c0 = pair.Key[0];
//				char c1 = pair.Key[1];
//				int freq = pair.Value;
//				foreach (var pinyin in PinyinDict.GetPinyins(c1))
//					statByPinyinByChar.GetOrAddDefault(c0)
//					                  .GetOrAddDefault(pinyin)
//					                  .Add(c1, freq);
//			}
//			dict = statByPinyinByChar.ToDictionary(pair => pair.Key, pair => pair.Value.ToDictionary(
//				ppair => ppair.Key, ppair => ppair.Value.ToDistribute()));
//		}
//		public override void Compress()
//		{
//			foreach (var dic in dict.Values)
//			{
//				var pinyins = dic.Keys.ToList();
//				foreach (var pinyin in pinyins)
//					dic[pinyin] = dic[pinyin].Take(5);
//			}
//		}
//	}
//}
