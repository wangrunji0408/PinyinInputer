using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class NGramModel
	{
		// Shared Property
		public virtual PinyinDict PinyinDict { get; set; }

		// Abstract Method
		public abstract IEnumerable<KeyValuePair<Condition, Statistic>> Statistics { get; }
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

		protected bool InCharSet(char c)
		{
			const char CHINESE_CHAR_MIN = (char)0x4e00;
			const char CHINESE_CHAR_MAX = (char)0x9fbb;
			// const int CHINESE_CHAR_COUNT = CHINESE_CHAR_MAX - CHINESE_CHAR_MIN + 1;
			return c >= CHINESE_CHAR_MIN && c <= CHINESE_CHAR_MAX;
		}
	}
}
