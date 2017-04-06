using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinyinAnalyzer
{
	public class InputerTester
	{
		FullPinyinInputer[] Inputers { get; set; }
	    PinyinDict PinyinDict => PinyinDict.Instance;
		public Dictionary<FullPinyinInputer, ModelCount> Results { get; }
	    int PrintToConsoleAfter { get; } = 10;

	    public InputerTester(params FullPinyinInputer[] inputers)
		{
			Inputers = inputers;
//			PinyinDict = pydict;
		    Results = Inputers.ToDictionary(key => key, _ => new ModelCount());
		}

	    public SentenseCompare TestSentense(FullPinyinInputer inputer, string sentense, IEnumerable<string> pinyins)
		{
		    string result = "";
		    try
		    {
		        inputer.Clear();
		        foreach (var pinyin in pinyins)
		            inputer.Input(pinyin);
		        result = inputer.Results.First();
		    }
		    catch (Exception e)
		    {

		    }
		    return new SentenseCompare(sentense, result);
		}

		public void TestData(TextReader reader, TextWriter resultWriter = null, string format = "chinese_only")
		{
		    resultWriter?.WriteLine("----- Inputer Test Report -----");
		    for(int i=0; i<Inputers.Length; ++i)
		        resultWriter?.WriteLine($"No.{i}: {Inputers[i].Name}");
		    resultWriter?.WriteLine("-------------------------------");

		    string chinese;
		    IEnumerable<string> pinyins;
			for (int i=0; reader.Peek() != -1; ++i)
			{
			    if (format == "pinyin_chinese")
			    {
			        pinyins = reader.ReadLine().Trim().ToLower().Split(' ');
			        chinese = reader.ReadLine().Trim();
			    }
			    else if(format == "chinese_only")
			    {
                    chinese = reader.ReadLine().Trim();
			        if(chinese == "")
			            continue;
			        pinyins = chinese.Select(c => PinyinDict.GetPinyins(c).FirstOrDefault())
			                        .Where(s => s != null);
			    }
			    else
			    {
			        throw new ArgumentException(nameof(format));
			    }
			    var cmps = new SentenseCompareMultiple(chinese);
			    foreach (var inputer in Inputers)
			    {
			        var compare = TestSentense(inputer, chinese, pinyins);
			        Results[inputer].Count(compare);
			        cmps.Add(inputer, compare);
			    }
			    resultWriter?.WriteLine(cmps);

			    if(PrintToConsoleAfter != 0 && i % PrintToConsoleAfter == 0)
			        Console.WriteLine($"Test Count = {i}");
			}
		    resultWriter?.WriteLine("Results:");
		    for(int i=0; i<Inputers.Length; ++i)
			    resultWriter?.WriteLine($"{i}: {Results[Inputers[i]]}");
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

		    public string DiffStr =>
		        string.Concat(Enumerable.Range(0, Length)
		            .Select(i => MismatchPosition.Contains(i) ? (i < Result.Length? Result[i]: '？') : '＋'));
		}

	    public class SentenseCompareMultiple
	    {
	        public string Std { get; }
	        private List<SentenseCompare> Compares = new List<SentenseCompare>();

	        public SentenseCompareMultiple(string std)
	        {
	            Std = std;
	        }

	        public void Add(FullPinyinInputer inputer, string result)
	        {
	            Compares.Add(new SentenseCompare(Std, result));
	        }
	        public void Add(FullPinyinInputer inputer, SentenseCompare compare)
	        {
	            Compares.Add(compare);
	        }

	        public override string ToString()
	        {
	            var builder = new StringBuilder();
	            builder.Append("> ").AppendLine(Std);
	            int i = 0;
	            foreach (var cmp in Compares)
	                builder.Append($"{i++} ").AppendLine(cmp.DiffStr);
	            return builder.ToString();
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
				=> $"Char: {MatchRateChar}\tSentence: {MatchRateSentence}";
		}
	}
}
