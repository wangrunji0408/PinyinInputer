using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PinyinAnalyzer
{
	public class NGramBindModel : NGramModelBase
	{
		NGramModelBase[] Models { get; }
		public Func<IEnumerable<Distribute<char>>, Distribute<char>> MixDistributeStrategy { get; set; }
			= MixDistributeStrategy_MaxNotEmptyN;

		public override PinyinDict PinyinDict
		{
			set
			{
				base.PinyinDict = value;
				foreach(var ng in Models)
					ng.PinyinDict = value;
			}
		}

		public NGramBindModel(params NGramModelBase[] models)
		{
			Models = models;
		}

		public override Distribute<char> GetDistribute(Condition condition)
		{
		    var dtbs = Models.Take(Math.Min(condition.N, 2) + 1)
		                    .Select(m => m.GetDistribute(condition));
			return MixDistributeStrategy(dtbs);
		}

		public static Distribute<char> MixDistributeStrategy_MaxNotEmptyN(IEnumerable<Distribute<char>> dtbs)
		{
			return dtbs.Last(dtb => !dtb.IsEmpty);
		}

		public static Distribute<char> MixDistributeStrategy_Lambda(IEnumerable<Distribute<char>> dtbs)
		{
			var dict = new Dictionary<Distribute<char>, float>();
			float rest = 1, lambda = 0.75f;
			foreach (var dtb in dtbs.Reverse())
			{
				dict.Add(dtb, rest * lambda);
				rest *= 1 - lambda;
			}
			return new Distribute<Distribute<char>>(dict).ExpandAndMerge(dtb => dtb);
		}

		public override void FromStatistician(TextStatistician stat)
		{
			throw new InvalidOperationException();
		}

		public override Distribute<char> GetDistribute(string pre)
		{
			throw new NotImplementedException();
		}
	}
}
