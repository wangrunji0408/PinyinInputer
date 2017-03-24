using System;
using System.Collections.Generic;

namespace PinyinAnalyzer
{
	public static class DictionaryExtension
	{
		public static TValue GetOrAddDefault<TKey, TValue> (this IDictionary<TKey, TValue> dict, TKey key)
			where TValue: new()
		{
			TValue val;
			if (!dict.TryGetValue(key, out val))
			{
				val = new TValue();
				dict.Add(key, val);
			}
			return val;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
			where TValue : new()
		{
			TValue val;
			if (!dict.TryGetValue(key, out val))
				val = new TValue();
			return val;
		}
	}
}
