using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class Statistic<T>: IStatistic<T>
	{
		[JsonProperty("total")]
		public int Total { get; private set; }

		[JsonProperty("dict")]
		readonly IDictionary<T, int> dict = new Dictionary<T, int>();

		public int Count => dict.Count;

		public IEnumerable<KeyValuePair<T, int>> KeyFrequencyDescending =>
			from pair in dict orderby pair.Value descending select pair;

		public IEnumerable<KeyValuePair<T, int>> KeyFrequencyAscending =>
			from pair in dict orderby pair.Value select pair;

		public IEnumerable<KeyValuePair<T, int>> KeyFrequency => dict;

		public int this[T key]
		{
			get
			{
				return dict.GetOrDefault(key);
			}

			set
			{
				if (dict.ContainsKey(key))
					dict[key] += value;
				else
					dict.Add(key, value);
				Total += value;
			}
		}

		public void Add(T c, int count = 1)
		{
			if (dict.ContainsKey(c))
				dict[c] += count;
			else
				dict.Add(c, count);
			Total += count;
		}

		public Statistic()
		{ 
		}

		public Statistic(IEnumerable<KeyValuePair<T, int>> dict)
		{
			foreach (var pair in dict)
				this.dict.Add(pair.Key, pair.Value);
		}

		public Statistic<T> Where(Func<T, bool> prediction)
		{
			return new Statistic<T>(dict.Where(pair => prediction(pair.Key)));
		}

		public Statistic<T> Where(Func<KeyValuePair<T, int>, bool> prediction)
		{
			return new Statistic<T>(dict.Where(pair => prediction(pair)));
		}

		public Statistic<T1> Select<T1>(Func<T, T1> func)
		{
			return new Statistic<T1>(dict.Select(
				pair => new KeyValuePair<T1, int>(func(pair.Key), pair.Value)));
		}

		public Statistic<T> Take(int count)
		{
			return new Statistic<T>(KeyFrequencyDescending.Take(count));
		}

		public Distribute<T> ToDistribute()
		{
			return new Distribute<T>(dict.Select(pair => 
			       		new KeyValuePair<T, float>(pair.Key, (float)pair.Value / Total)));
		}

		public void Clear()
		{
			dict.Clear();
			Total = 0;
		}
	}
}
