using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PinyinAnalyzer.ConsoleApp
{
	public static class Function
	{
		//static string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		//static string ngram1Path = $"{path}count1.txt";
		//static string ngram2Path = $"{path}count2.txt";
		//static string ngram3Path = $"{path}count3.txt";
		//static string pinyinPath = $"{path}拼音汉字表/拼音汉字表.txt";
		//static string statPath = $"{path}stat.csv";
		//static string inputPath = $"{path}input.txt";
		//static string outputPath = $"{path}output.txt";
		//static string testInputPath = $"{path}test_in.txt";
		//static string testOutputPath = $"{path}test_out.txt";

		public static void Analyze(IEnumerable<string> filePaths, string outputFile = null)
		{
			if (outputFile == null)
				TextStatistic.AnalyzeFilesSeparately(filePaths);
			else
				TextStatistic.AnalyzeFiles(filePaths, outputFile);
		}

		public static void Merge(IEnumerable<string> filePaths, string outputFile)
		{
			//var paths = Enumerable.Range(1, 11).Select(i => $"{path}sina_news/2016-{i:00}.txt_stat.csv");
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

		public static void TestOnData(string modelName, string inputFilePath, string outputFilePath)
		{
			var model = NGramModelFileLoader.LoadByName(modelName);
			PinyinDict pydict = model.PinyinDict;
			var inputer = new NGramInputer(model);
			var tester = new InputerTester(inputer, pydict);

			using (var inputFile = File.OpenText(inputFilePath))
			{
				if (outputFilePath == null)
					tester.TestData(inputFile, Console.Out);
				else
					using (var outputFile = File.CreateText(outputFilePath))
						tester.TestData(inputFile, outputFile);
			}
		}
	}
}
