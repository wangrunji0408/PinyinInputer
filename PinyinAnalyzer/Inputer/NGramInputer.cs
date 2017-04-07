using System;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	/// <summary>
	/// 使用N-Gram统计模型的完整拼音输入法。
	/// </summary>
	public class NGramInputer : FullPinyinInputer
	{
	    public override string Name => $"NGramInputer using [{Model.GetType().Name}] [{Model.SourceName}]";
	    NGramModelBase Model { get; }
		Distribute<string> distribute = Distribute<string>.Single("");
		List<string> goodResults = new List<string>();
	    public int TakeSize { get; set; } = 10;

	    public bool TraceDistribute { get; set; } = false;
	    public bool MakeGoodResults { get; set; } = false;
	    public int PrintDistributeSize { get; set; } = 0;
		public List<Distribute<string>> Distributes { get; } = new List<Distribute<string>>();

		public NGramInputer(NGramModelBase model)
		{
			Model = model;
		}

		public override IEnumerable<string> Results
			=> distribute.KeyProbDescending.Select(pair => pair.Key);

		public override IEnumerable<string> SubResult
			=> goodResults.Reverse<string>();

		public override void Clear()
		{
			base.Clear();
			distribute = Distribute<string>.Single("");
			goodResults.Clear();
			Distributes.Clear();
		}

		public override void Input(string pinyin)
		{
		    distribute = distribute.ExpandAndMerge(str =>
		            Model.GetDistribute(new Condition(str, pinyin))
						.Take(TakeSize)
		                .Select(c => str + c))
		            .Take(TakeSize)
		            .Norm();
		    if(MakeGoodResults)
			    goodResults.AddRange(distribute.KeyProbDescending
											 .TakeWhile(pair => pair.Value > 0.2)
											 .Reverse()
											 .Select(pair => pair.Key));
			if (TraceDistribute)
				Distributes.Add(distribute);
			if (PrintDistributeSize > 0)
				distribute.Take(PrintDistributeSize).Print();
		}

		public override void ConfirmSubResult(int index)
		{
			throw new NotImplementedException();
		}
	}

	public static class NGramInputerExtension
	{
		public static NGramInputer ToInputer(this NGramModelBase model)
		{
			return new NGramInputer(model);
		}
	}
}
