using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class TextAnalyzerBinary: TextAnalyzer
	{
		//Dictionary<Condition, Statistic> _statisticDict = new Dictionary<Condition, Statistic>();
		[JsonProperty("dict")]
		Dictionary<char, Statistic> _statisticDict = new Dictionary<char, Statistic>();

		protected override IEnumerable<KeyValuePair<Condition, Statistic>> CSPair =>
			_statisticDict.Select(pair => new KeyValuePair<Condition, Statistic>(
				new Condition(pair.Key.ToString()), pair.Value));

		public override Statistic GetStatistic(Condition condition)
		{
			if(condition.N != 1)
				throw new NotImplementedException();
			return GetStatistic(condition.Chars[0]);
		}

		public Statistic GetStatistic(char c)
		{
			return _statisticDict.GetOrAddDefault(c);
		}

		public override void Analyze(TextReader reader)
		{
			char lastChar = '\0';
			while(reader.Peek() != -1)
			{
				var c = (char)reader.Read();
				if (!IsChineseChar(c))
				{
					lastChar = '\0';
					continue;
				}
				//var condition = new Condition(lastChar.ToString());
				GetStatistic(lastChar).Add(c);
				lastChar = c;
			}
		}

		public override IInputer BuildInputer()
		{
			throw new NotImplementedException();
		}
	}
}
