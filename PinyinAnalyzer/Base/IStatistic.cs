using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	public interface IStatistic<TKey>
	{
		int Total { get; }
		int Count { get; }

		IEnumerable<KeyValuePair<TKey, int>> KeyFrequencyDescending { get; }
		IEnumerable<KeyValuePair<TKey, int>> KeyFrequencyAscending { get; }
		IEnumerable<KeyValuePair<TKey, int>> KeyFrequency { get; }

		int this[TKey key] { get; set; }
	    void Add(TKey key, int count = 1);
		void Clear();
	}

	public static class StatisticExtension
	{
		public static void Add<TKey>(this IStatistic<TKey> stat, IEnumerable<KeyValuePair<TKey, int>> pairs)
		{
			foreach (var pair in pairs)
				stat.Add(pair.Key, pair.Value);
		}

		public static void WriteToCsv<TKey>(this IStatistic<TKey> stat, TextWriter writer, int take = -1)
		{
			foreach (var pair in take == -1 ? stat.KeyFrequency : stat.KeyFrequency.Take(take))
				writer.WriteLine($"{pair.Key}, {pair.Value}");
		}

		public static void ReadFromCsv<TKey>(this IStatistic<TKey> stat, TextReader reader, Func<string, TKey> keyParser)
		{
			stat.Clear();
			while (reader.Peek() != -1)
			{
				var ss = reader.ReadLine().Split(',');
				var key = keyParser(ss[0]);
				var freq = int.Parse(ss[1]);
			    stat.Add(key, freq);
			}
		}
	}
}
