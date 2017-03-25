using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PinyinAnalyzer
{
	public class NGramBindModel: NGramModel
	{
		NGramModel[] Models { get; }
		public Func<NGramModel[], Condition, Distribute<char>> MixDistributeStrategy { get; set; }
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

		public NGramBindModel(params NGramModel[] models)
		{
			Models = models;
		}

		public override IEnumerable<KeyValuePair<Condition, Statistic>> Statistics
			=> Models.SelectMany(m => m.Statistics);

		public override void Analyze(TextReader reader)
		{
			throw new NotImplementedException();
		}

		public override Distribute<char> GetDistribute(Condition condition)
		{
			return MixDistributeStrategy(Models, condition);
		}

		public static Distribute<char> MixDistributeStrategy_MaxNotEmptyN(NGramModel[] models, Condition condition)
		{
			return models.Take(Math.Min(condition.N, 2) + 1)
				      .Reverse()
				      .Select(ng => ng.GetDistribute(condition))
				      .First(dtb => !dtb.IsEmpty);
		}

		public static Distribute<char> MixDistributeStrategy_Lambda(NGramModel[] models, Condition condition)
		{
			var dict = new Dictionary<NGramModel, float>();
			float rest = 1, lambda = 0.75f;
			foreach (var model in models.Reverse())
			{
				dict.Add(model, rest * lambda);
				rest *= 1 - lambda;
			}
			return new Distribute<NGramModel>(dict).ExpandAndMerge(ng => ng.GetDistribute(condition));
		}

		protected override Statistic GetStatisticOnlyByChars(Condition condition)
		{
			throw new NotImplementedException();
		}
	}
}
