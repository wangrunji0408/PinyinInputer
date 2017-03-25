using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class NGram2Model: NGramModel
	{
		[JsonProperty("dict")]
		Dictionary<char, Statistic> _statisticDict = new Dictionary<char, Statistic>();

		public override IEnumerable<KeyValuePair<Condition, Statistic>> Statistics =>
			_statisticDict.Select(pair => new KeyValuePair<Condition, Statistic>(
				new Condition(pair.Key.ToString()), pair.Value));

		protected override Statistic GetStatisticOnlyByChars(Condition condition)
		{
			string chars = condition.Chars.LastSubString(1, '^');
			return GetStatistic(chars[0]);
		}

		public Statistic GetStatistic(char c)
		{
			return _statisticDict.GetOrAddDefault(c);
		}

		public override void Analyze(TextReader reader)
		{
			char lastChar = '^';
			while(reader.Peek() != -1)
			{
				var c = (char)reader.Read();
				if (!InCharSet(c))
				{
					lastChar = '^';
					continue;
				}
				GetStatistic(lastChar).Add(c);
				lastChar = c;
			}
		}
	}
}
