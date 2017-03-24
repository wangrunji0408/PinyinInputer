using System;
namespace PinyinAnalyzer
{
	public static class StringExtension
	{
		public static string LastSubString(this string str, int lastn, char space = ' ')
		{
			int l = str.Length;
			if (l >= lastn)
				return str.Substring(l - lastn);
			else
				return new string(space, lastn - l) + str;
		}
	}
}
