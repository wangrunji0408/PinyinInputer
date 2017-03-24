using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Distribute<T>
	{
		[JsonProperty]
		Dictionary<T, float> dict = new Dictionary<T, float>();
		public IEnumerable<KeyValuePair<T, float>> KeyProbDescending =>
			from pair in dict
			orderby pair.Value descending
			select pair;

		public float GetProbability(T key)
		{
			return dict.GetOrDefault(key);
		}

		private Distribute()
		{
			
		}

		public Distribute(IEnumerable<KeyValuePair<T, float>> dict)
		{
			foreach (var pair in dict)
				this.dict.Add(pair.Key, pair.Value);
			Normalize();
		}

		static public Distribute<T> Single(T key)
		{
			var obj = new Distribute<T>();
			obj.dict.Add(key, 1);
			return obj;
		}

		void Normalize()
		{
			float sum = dict.Values.Sum();
			if (Math.Abs(sum - 1) < 1e-5)
				return;
			dict = dict.ToDictionary(pair => pair.Key, pair => pair.Value / sum);
		}

		public Distribute<T> Where(Func<T, bool> prediction)
		{
			return new Distribute<T>(dict.Where(pair => prediction(pair.Key)));
		}

		public Distribute<T1> Select<T1>(Func<T, T1> func)
		{
			return new Distribute<T1>(dict.Select(
				pair => new KeyValuePair<T1, float>(func(pair.Key), pair.Value)));
		}

		public Distribute<T> Take(int count)
		{
			return new Distribute<T>(KeyProbDescending.Take(count));
		}

		public Distribute<T1> ExpandAndMerge<T1>(Func<T, Distribute<T1>> func)
		{
			var newPairs = from pair in dict
							from ppair in func(pair.Key).dict
			                group ppair.Value * pair.Value by ppair.Key into g
							select new KeyValuePair<T1, float>(g.Key, g.Sum());
			return new Distribute<T1>(newPairs);
		}

		public Distribute<T> ExpandAndMerge(Func<T, Distribute<T>> func)
			=> ExpandAndMerge<T>(func);

		public void Print()
		{
			int count = 10;
			foreach (var pair in KeyProbDescending)
			{
				Console.Write($"{pair.Key}({pair.Value}),");
				if (count-- == 0) break;
			}
			Console.WriteLine();
		}
	}
}
