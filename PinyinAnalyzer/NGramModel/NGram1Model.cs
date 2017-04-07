using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[Serializable]
	public class NGram1Model: NGramModelBase
	{
		[JsonProperty]
		Distribute<char> dtb = new Distribute<char>();

		public override Distribute<char> GetDistribute(string pre)
		{
			return dtb;
		}
		public override void FromStatistician(TextStatistician stat)
		{
		    base.FromStatistician(stat);
			var stat0 = new Statistic<char>();
			foreach (var pair in stat.StringFrequency)
			{
				if (pair.Key.Length != 1)
					continue;
				char c = pair.Key[0];
				int freq = pair.Value;
				stat0.Add(c, freq);
			}
			dtb = stat0.ToDistribute();
		}
	}
}
