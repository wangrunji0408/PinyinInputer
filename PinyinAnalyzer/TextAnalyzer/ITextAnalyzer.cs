using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[JsonObject(MemberSerialization.OptIn)]
	public interface ITextAnalyzer
	{
		void Analyze(string str);
		void Analyze(TextReader reader);
		Statistic GetStatistic(Condition condition);
		IInputer BuildInputer();
	}

	public abstract class TextAnalyzer : ITextAnalyzer
	{
		protected abstract IEnumerable<KeyValuePair<Condition, Statistic>> CSPair { get; }
		public abstract Statistic GetStatistic(Condition condition);
		public abstract void Analyze(TextReader reader);
		public void Analyze(string str)
		{
			Analyze(new StringReader(str));
		}
		public abstract IInputer BuildInputer();


		protected const char CHINESE_CHAR_MIN = (char)0x4e00;
		protected const char CHINESE_CHAR_MAX = (char)0x9fbb;
		protected const int CHINESE_CHAR_COUNT = CHINESE_CHAR_MAX - CHINESE_CHAR_MIN + 1;
		protected bool IsChineseChar(char c)
		{
			return c >= CHINESE_CHAR_MIN && c <= CHINESE_CHAR_MAX;
		}

		//public void WriteTo(TextWriter textWriter)
		//{
		//	using (var writer = new JsonTextWriter(textWriter))
		//	{
		//		writer.WriteStartObject();
		//		foreach (var pair in CSPair)
		//		{
		//			var c = pair.Key;
		//			var state = pair.Value;
		//			writer.WritePropertyName(c.ToString());
		//			writer.WriteStartObject();
		//			foreach (var pair1 in state.CharCountDescending)
		//			{
		//				var c1 = pair1.Key;
		//				var count = pair1.Value;
		//				writer.WritePropertyName(c1.ToString());
		//				writer.WriteValue(count);
		//			}
		//			writer.WriteEndObject();
		//		}
		//		writer.WriteEndObject();
		//	}
		//}
	}
}
