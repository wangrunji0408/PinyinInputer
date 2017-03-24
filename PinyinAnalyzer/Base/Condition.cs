using System;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct Condition
	{
		public int N => Chars.Length;
		[JsonProperty]
		public string Chars { get; private set; }
		public override string ToString() => Chars;
		public Condition(string str)
		{
			Chars = str;
		}
		public Condition Reduce()
		{
			if (N == 0)
				throw new InvalidOperationException();
			return new Condition(Chars.Substring(1));
		}
	}
}
