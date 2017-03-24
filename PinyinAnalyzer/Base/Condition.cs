using System;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct Condition
	{
		public int N => Chars.Length;
		[JsonProperty]
		public string Chars { get; }
		[JsonProperty]
		public string Pinyin { get; }
		public override string ToString() => Chars;
		public Condition(string str, string pinyin = "")
		{
			Chars = str;
			Pinyin = pinyin;
		}
		public Condition Reduce()
		{
			if (N == 0)
				throw new InvalidOperationException();
			return new Condition(Chars.Substring(1));
		}
	}
}
