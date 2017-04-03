using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	public class InputerTester
	{
		FullPinyinInputer Inputer { get; set; }
		PinyinDict PinyinDict { get; set; }
		public ModelCount Result { get; }

		public InputerTester(FullPinyinInputer inputer, PinyinDict pydict)
		{
			Inputer = inputer;
			PinyinDict = pydict;
			Result = new ModelCount();
		}

		public SentenseCompare TestSentense(string sentense)
		{
			var pinyins = sentense.Select(c => PinyinDict.GetPinyins(c).FirstOrDefault())
								  .Where(s => s != "");
			Inputer.Clear();
			foreach (var pinyin in pinyins)
				Inputer.Input(pinyin);
			string result = "";
			if(Inputer.Results.Count() != 0)
				result = Inputer.Results.First();
			return new SentenseCompare(sentense, result);
		}

		public void TestData(TextReader sentenseReader, TextWriter resultWriter = null)
		{
			while (sentenseReader.Peek() != -1)
			{
				var input = sentenseReader.ReadLine();
				if (input == "")
					continue;
				var compare = TestSentense(input);
				Result.Count(compare);
				resultWriter?.WriteLine(compare);
			}
			resultWriter?.WriteLine(Result);
		}

		//string ToPinyin(string sentense)
		//{
		//	var pinyins = sentense.Select(c => PinyinDict.GetPinyins(c).FirstOrDefault())
		//	                      .Where(s => s != "");
		//	return string.Join(" ", pinyins);
		//}

		public class SentenseCompare
		{
			public string Input { get; }
			public string Result { get; }
			public int MatchCount { get; }
			public int Length { get; }
			public float MatchRate => (float)MatchCount / Length;
			public bool FullMatch => MatchCount == Length;
			public ISet<int> MismatchPosition { get; }

			public SentenseCompare(string input, string result)
			{
				Input = input;
				Result = result;
				Length = Math.Max(input.Length, result.Length);
				MismatchPosition = new HashSet<int>();
				for (int i = 0; i < Length; ++i)
					if (input.ElementAtOrDefault(i) == result.ElementAtOrDefault(i))
						MatchCount++;
					else
						MismatchPosition.Add(i);
			}

			public override string ToString()
			{
				var pointers = string.Concat(Enumerable.Range(0, Length)
				                          				.Select(i => MismatchPosition.Contains(i) ? '＋' : '　'));
				return string.Join("\n", Input, Result, pointers);
			}
		}

		public class ModelCount
		{
			//public string ModelName { get; }
			public int CountChar { get; private set; }
			public int CountMatchChar { get; private set; }
			public int CountSentense { get; private set; }
			public int CountMatchSentense { get; private set; }
			public float MatchRateChar => (float)CountMatchChar / CountChar;
			public float MatchRateSentence => (float)CountMatchSentense / CountSentense;

			//public ModelCount(string name)
			//{
			//	ModelName = name;
			//}

			internal void Count(SentenseCompare compare)
			{
				CountChar += compare.Length;
				CountMatchChar += compare.MatchCount;
				CountSentense += 1;
				CountMatchSentense += compare.FullMatch ? 1 : 0;
			}

			public override string ToString()
				=> $"\nChar: {MatchRateChar}\nSentense: {MatchRateSentence}";
		}
	}
}
