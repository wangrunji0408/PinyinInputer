using System;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	public class BinaryInputer : PinyinInputer
	{
		readonly TextAnalyzer _analyzer;
		readonly PinyinDict _pydict;

		public BinaryInputer(TextAnalyzer analyzer, PinyinDict pydict)
		{
			_analyzer = analyzer;
			_pydict = pydict;
		}

		public override IEnumerable<string> Results //=> new[] { result };
			=> distribute.KeyProbDescending.Select(pair => pair.Key);

		string result = "";
		Distribute<string> distribute = Distribute<string>.Single("");

		public override void Clear()
		{
			base.Clear();
			result = "";
			distribute = Distribute<string>.Single("");
		}

		char GetLast(string str) =>
			str.Length == 0 ? '\0' : str[str.Length - 1];

		public override void Input(string pinyin)
		{
			distribute = distribute.ExpandAndMerge(str => 
			                        _analyzer.GetStatistic(new Condition(GetLast(str).ToString()))
			                                       .ToDistribute()
												   .Where(c => _pydict.GetPinyins(c).Contains(pinyin))
			                                       .Take(10)
			                                       .Select(c => str + c))
			                       .Take(10);
			distribute.Print();
			//var stat = _analyzer.GetStatistic(new Condition(last.ToString()));
			//char c = stat.CharCountDescending
			//			 .FirstOrDefault(pair => _pydict.GetPinyins(pair.Key).Contains(pinyin))
			//             .Key;
			//result += c;
			//Console.WriteLine($"Read: {pinyin}, Char: {c}");
		}
	}
}
