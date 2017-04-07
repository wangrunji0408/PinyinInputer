using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public abstract class NGramModelBase
	{
		// Shared Property
		public virtual PinyinDict PinyinDict { get; set; } = PinyinDict.Instance;
	    public string SourceName { get; private set; }

	    // Abstract Method
	    public virtual void FromStatistician(TextStatistician stat)
	    {
	        SourceName = stat.SourceName;
	    }
		public abstract Distribute<char> GetDistribute(string pre);
		public virtual void Compress() 
		{
			throw new NotImplementedException();
		}

		// Implement
		public bool InCharSet(char c)
		{
			const char CHINESE_CHAR_MIN = (char)0x4e00;
			const char CHINESE_CHAR_MAX = (char)0x9fbb;
			// const int CHINESE_CHAR_COUNT = CHINESE_CHAR_MAX - CHINESE_CHAR_MIN + 1;
			return c >= CHINESE_CHAR_MIN && c <= CHINESE_CHAR_MAX;
		}

		public virtual Distribute<char> GetDistribute(Condition condition)
		{
			var dtb = GetDistribute(condition.Chars);
			if(condition.Pinyin != null)
				dtb = dtb.Where(c => PinyinDict.GetPinyins(c).Contains(condition.Pinyin));
			return dtb;
		}
	}
}
