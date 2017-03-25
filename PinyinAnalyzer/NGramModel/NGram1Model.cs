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

		public override IEnumerable<KeyValuePair<Condition, Statistic>> Statistics =>
			new[] { new KeyValuePair<Condition, Statistic>(new Condition(""), _stat) };

		public override void Analyze(TextReader reader)
		{
			while (reader.Peek() != -1)
			{
				char c = (char)reader.Read();
				if (!InCharSet(c))
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
