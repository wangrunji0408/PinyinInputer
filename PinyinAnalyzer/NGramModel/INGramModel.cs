using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	public interface ITextAnalyzer
	{
		void Analyze(string str);
		void Analyze(TextReader reader);
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class NGramModel : ITextAnalyzer
	{
		// Shared Property
		public virtual PinyinDict PinyinDict { get; set; }

		// Abstract Method
		protected abstract IEnumerable<KeyValuePair<Condition, Statistic>> CSPair { get; }
		protected abstract Statistic GetStatisticOnlyByChars(Condition condition);
		public abstract void Analyze(TextReader reader); 

		// Implement
		public void Analyze(string str)
		{
			Analyze(new StringReader(str));
		}

		public virtual Distribute<char> GetDistribute(Condition condition)
		{
			var dtb = GetStatisticOnlyByChars(condition).ToDistribute();
			if (condition.Pinyin == "")
				return dtb;
			if (PinyinDict == null)
				throw new NullReferenceException("PinyinDict is null!");
			return dtb.Where(c => PinyinDict.GetPinyins(c).Contains(condition.Pinyin));
		}

		protected const char CHINESE_CHAR_MIN = (char)0x4e00;
		protected const char CHINESE_CHAR_MAX = (char)0x9fbb;
		protected const int CHINESE_CHAR_COUNT = CHINESE_CHAR_MAX - CHINESE_CHAR_MIN + 1;
		protected bool IsChineseChar(char c)
		{
			return c >= CHINESE_CHAR_MIN && c <= CHINESE_CHAR_MAX;
		}
	}
}
