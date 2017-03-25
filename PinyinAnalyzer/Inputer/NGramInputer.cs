using System;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	public class NGramInputer : PinyinInputer
	{
		readonly NGramModel _model;

		public bool TraceDistribute { get; set; } = true;
		public List<Distribute<string>> Distributes { get; } = new List<Distribute<string>>();

		public NGramInputer(NGramModel model)
		{
			_model = model;
		}

		public override IEnumerable<string> Results
			=> distribute.KeyProbDescending.Select(pair => pair.Key);

		Distribute<string> distribute = Distribute<string>.Single("");

		public override void Clear()
		{
			base.Clear();
			distribute = Distribute<string>.Single("");
			Distributes.Clear();
		}

		public override void Input(string pinyin)
		{
			distribute = distribute.ExpandAndMerge(str => 
			                                       _model.GetDistribute(new Condition(str, pinyin))
			                                       .Take(10)
			                                       .Select(c => str + c))
			                       .Take(10);
			if (TraceDistribute)
				Distributes.Add(distribute);
		}
	}
}
