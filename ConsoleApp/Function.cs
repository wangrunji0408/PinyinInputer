using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PinyinAnalyzer.ConsoleApp
{
	public static class Function
	{
		public static void Analyze(IEnumerable<string> filePaths, bool merge, string outputFile = null, string outputDir = null)
		{
			if (merge)
				TextStatistic.AnalyzeFiles(filePaths, outputFile);
		    else
			    TextStatistic.AnalyzeFilesSeparately(filePaths, outputDir);
		}

		public static void Merge(IEnumerable<string> filePaths, string outputFile)
		{
			TextStatistic.MergeFiles(filePaths, outputFile);
		}

		public static void Solve(string inputFile, string outputFile, string modelName = "n")
		{
			var model = NGramModelFileLoader.LoadByName(modelName);
			var inputer = new NGramInputer(model);
			using (var outputWriter = File.CreateText(outputFile))
			{
				foreach (string input in File.ReadLines(inputFile))
				{
					inputer.Clear();
					foreach (string pinyin in input.Split())
						inputer.Input(pinyin);
					outputWriter.WriteLine(inputer.Results.First());
				}
			}
		}

		public static void QSolve(string modelName = "n")
		{
			NGramModelBase model = NGramModelFileLoader.LoadByName(modelName);
			var inputer = new NGramInputer(model);
			inputer.PrintDistributeSize = 3;
			Console.WriteLine($"Using model: {model.GetType().Name}");

			while (true)
			{
				Console.Write("input > ");
				var input = Console.ReadLine();
				try
				{
					inputer.Clear();
					foreach (string pinyin in input.Split())
						inputer.Input(pinyin);
					//Console.WriteLine("SubResults:");
					//foreach (var result in inputer.SubResult.Take(5))
					//	Console.WriteLine("  " + result);
					Console.WriteLine(inputer.Results.First());
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		public static void QPinyin(string filePath = null)
		{
			var pydict = filePath == null ? PinyinDict.Instance : new PinyinDict(filePath);
			while (true)
			{
				Console.Write("pinyin > ");
				var input = Console.ReadLine();
				if (input == "")
					continue;
				var chars = pydict.GetChars(input);
				var pinyins = pydict.GetPinyins(input[0]);
				if (chars.Count != 0)
				{
					foreach (char c in chars)
						Console.Write(c);
					Console.WriteLine();
				}
				else if (pinyins.Count != 0)
				{
					foreach (string py in pinyins)
						Console.Write(py + " ");
					Console.WriteLine();
				}
			}
		}

		public static void QModel(string modelName)
		{
			NGramModelBase ng1 = NGramModelFileLoader.Load<NGram1Model>();
			NGramModelBase ng2 = NGramModelFileLoader.Load<NGram2Model>();
			NGramModelBase ng3 = NGramModelFileLoader.Load<NGram3Model>();
			NGramModelBase ngn = NGramModelFileLoader.Load<NGramNModel>();

			while (true)
			{
				Console.Write("data > ");
				var input = Console.ReadLine(); // [char]+ [pinyin]
				string chars = input.Split()[0];
				string pinyin = input.Split().ElementAtOrDefault(1);
				var condition = new Condition(chars, pinyin);
				try
				{
					Console.WriteLine("1-gram");
					ng1.GetDistribute(condition).Take(5).Print();
					Console.WriteLine("2-gram");
					ng2.GetDistribute(condition).Take(5).Print();
					Console.WriteLine("3-gram");
					ng3.GetDistribute(condition).Take(5).Print();
					Console.WriteLine("n-gram");
					ngn.GetDistribute(condition).Take(5).Print();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

			}
		}

		public static void QStatistic(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine("File not exist.");
				return;
			}
			var stat = new TextStatistician(filePath);
			while (true)
			{
				Console.Write("stat > ");
				var input = Console.ReadLine();
				int count = stat.GetStringCount(input);
				Console.WriteLine(count);
			}
		}

		public static void BuildModel(string filePath, IEnumerable<string> modelNames)
		{
			var stat = new TextStatistician(filePath);
			foreach (var modelName in modelNames)
			{
				var model = NGramModelFileLoader.NewByName(modelName);
				model.FromStatistician(stat);
				model.Save();
			}
		}

	    /// <summary>
	    ///
	    /// </summary>
	    /// <param name="format">"chinese_only" | "pinyin_chinese"</param>
		public static void TestOnData(IEnumerable<string> modelNames, string inputFilePath, string outputFilePath, string format)
		{
			var models = modelNames.Select(name => NGramModelFileLoader.LoadByName(name));
			var inputers = models.Select(model => new NGramInputer(model)).Cast<FullPinyinInputer>().ToArray();
			var tester = new InputerTester(inputers);

			using (var inputFile = File.OpenText(inputFilePath))
			{
				if (outputFilePath == null)
					tester.TestData(inputFile, Console.Out, format);
				else
					using (var outputFile = File.CreateText(outputFilePath))
						tester.TestData(inputFile, outputFile, format);
			}
		}
	}
}
