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
				ng1.PinyinDict = value;
				ng2.PinyinDict = value;
				ng3.PinyinDict = value;
			}
		}

		public NGram123Model()
		{
			string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
			string path1 = $"{path}count1.txt";
			string path2 = $"{path}count2.txt";
			//string path3 = $"{path}count3.txt";

			using (var fileReader = File.OpenText(path1))
				ng1 = new JsonSerializer().Deserialize<NGram1Model>(new JsonTextReader(fileReader));
			using (var fileReader = File.OpenText(path2))
				ng2 = new JsonSerializer().Deserialize<NGram2Model>(new JsonTextReader(fileReader));
			ng3 = new NGram3Model();
			//using (var fileReader = File.OpenText(path3))
				//ng3 = new JsonSerializer().Deserialize<NGram3Model>(new JsonTextReader(fileReader));
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
