using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class TextAnalyzerBinary_Array: TextAnalyzer
	{
		Statistic[] _statisticArray = new Statistic[CHINESE_CHAR_COUNT];
		Statistic _beginStat = new Statistic();

		protected override IEnumerable<KeyValuePair<Condition, Statistic>> CSPair
		{
			get
			{
				yield return new KeyValuePair<Condition, Statistic>(new Condition("\0"), _beginStat);
				char c = CHINESE_CHAR_MIN;
				for (int i = 0; i < CHINESE_CHAR_COUNT; ++i, ++c)
				{
					var stat = _statisticArray[i];
					if (stat == null)
						continue;
					yield return new KeyValuePair<Condition, Statistic>(
						new Condition(c.ToString()), stat);
				}
			}		
		}

		public override Statistic GetStatistic(Condition condition)
		{
			if(condition.N != 1)
				throw new NotImplementedException();
			char c = condition.Chars[0];
			if (c == 0)
				return _beginStat;
			int index = c - CHINESE_CHAR_MIN;
			if (_statisticArray[index] == null)
				_statisticArray[index] = new Statistic();
			return _statisticArray[index];
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
				var condition = new Condition(lastChar.ToString());
				GetStatistic(condition).Add(c);
				lastChar = c;
			}
		}

		public override IInputer BuildInputer()
		{
			throw new NotImplementedException();
		}
	}
}
