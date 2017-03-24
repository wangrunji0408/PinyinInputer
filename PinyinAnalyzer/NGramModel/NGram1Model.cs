using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	public class NGram1Model: NGramModel
	{
		[JsonProperty("stat")]
		readonly Statistic _stat = new Statistic();

		protected override IEnumerable<KeyValuePair<Condition, Statistic>> CSPair =>
			new[] { new KeyValuePair<Condition, Statistic>(new Condition(""), _stat) };

		public override void Analyze(TextReader reader)
		{
			while (reader.Peek() != -1)
			{
				char c = (char)reader.Read();
				if (!IsChineseChar(c))
					continue;
				_stat.Add(c);
			}
		}

		protected override Statistic GetStatisticOnlyByChars(Condition condition)
		{
			return _stat;
		}
	}
}
