using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PinyinAnalyzer
{
	// Only 1 2
	public class NGram123Model: NGramModel
	{
		readonly NGram1Model ng1;
		readonly NGram2Model ng2;
		readonly NGram3Model ng3;
		readonly NGramModel[] ngs;

		public override PinyinDict PinyinDict
		{
			set
			{
				base.PinyinDict = value;
				foreach(var ng in ngs)
					ng.PinyinDict = value;
			}
		}

		public NGram123Model(NGram1Model ng1, NGram2Model ng2, NGram3Model ng3)
		{
			this.ng1 = ng1;
			this.ng2 = ng2;
			this.ng3 = ng3;
			ngs = new NGramModel[] { ng1, ng2, ng3 };
		}

		protected override IEnumerable<KeyValuePair<Condition, Statistic>> CSPair
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void Analyze(TextReader reader)
		{
			throw new NotImplementedException();
		}

		public override Distribute<char> GetDistribute(Condition condition)
		{
			for (int n = Math.Min(condition.N, 2); n >= 0; --n)
			{
				var distribute = ngs[n].GetDistribute(condition);
				if (!distribute.IsEmpty)
					return distribute;
			}
			throw new Exception("Should not reach there.");
		}

		protected override Statistic GetStatisticOnlyByChars(Condition condition)
		{
			throw new NotImplementedException();
		}
	}
}
