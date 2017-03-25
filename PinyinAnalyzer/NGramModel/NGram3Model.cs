using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class NGram3Model: NGramModel
	{
		//Dictionary<Condition, Statistic> _statisticDict = new Dictionary<Condition, Statistic>();
		[JsonProperty("dict")]
		Dictionary<string, Statistic> _statisticDict = new Dictionary<string, Statistic>();

		public override IEnumerable<KeyValuePair<Condition, Statistic>> Statistics =>
			_statisticDict.Select(pair => new KeyValuePair<Condition, Statistic>(
				new Condition(pair.Key.ToString()), pair.Value));

		protected override Statistic GetStatisticOnlyByChars(Condition condition)
		{
			string chars = condition.Chars.LastSubString(2, '^');
			return GetStatistic(chars);
		}

		public Statistic GetStatistic(string str)
		{
			return _statisticDict.GetOrAddDefault(str);
		}

		public override void Analyze(TextReader reader)
		{
			string pre = "^^";
			while(reader.Peek() != -1)
			{
				var c = (char)reader.Read();
				if (!InCharSet(c))
				{
					pre = "^^";
					continue;
				}
				//var condition = new Condition(lastChar.ToString());
				GetStatistic(pre).Add(c);
				pre = pre[1].ToString() + c;
			}
		}
	}
}
