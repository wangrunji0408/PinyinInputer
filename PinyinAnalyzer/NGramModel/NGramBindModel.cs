using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PinyinAnalyzer
{
	public class NGramBindModel : NGramModelBase
	{
		NGramModelBase[] Models { get; }
		public Func<NGramModelBase[], Condition, Distribute<char>> MixDistributeStrategy { get; set; }
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
			return MixDistributeStrategy(Models, condition);
		}

		public static Distribute<char> MixDistributeStrategy_MaxNotEmptyN(NGramModelBase[] models, Condition condition)
		{
			return models.Take(Math.Min(condition.N, 2) + 1)
				      .Reverse()
				      .Select(ng => ng.GetDistribute(condition))
				      .First(dtb => !dtb.IsEmpty);
		}

		public static Distribute<char> MixDistributeStrategy_Lambda(NGramModelBase[] models, Condition condition)
		{
			var dict = new Dictionary<NGramModelBase, float>();
			float rest = 1, lambda = 0.75f;
			foreach (var model in models.Reverse())
			{
				dict.Add(model, rest * lambda);
				rest *= 1 - lambda;
			}
			return new Distribute<NGramModelBase>(dict).ExpandAndMerge(ng => ng.GetDistribute(condition));
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
