using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class TextAnalyzerThree: TextAnalyzer
	{
		//Dictionary<Condition, Statistic> _statisticDict = new Dictionary<Condition, Statistic>();
		[JsonProperty("dict")]
		Dictionary<string, Statistic> _statisticDict = new Dictionary<string, Statistic>();

		protected override IEnumerable<KeyValuePair<Condition, Statistic>> CSPair =>
			_statisticDict.Select(pair => new KeyValuePair<Condition, Statistic>(
				new Condition(pair.Key.ToString()), pair.Value));

		public override Statistic GetStatistic(Condition condition)
		{
			if(condition.N != 2)
				throw new NotImplementedException();
			return GetStatistic(condition.Chars);
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
				if (!IsChineseChar(c))
				{
					pre = "^^";
					continue;
				}
				//var condition = new Condition(lastChar.ToString());
				GetStatistic(pre).Add(c);
				pre = pre[1].ToString() + c;
			}
		}

		public override IInputer BuildInputer()
		{
			throw new NotImplementedException();
		}
	}
}
