using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PinyinAnalyzer.ConsoleApp
{
	static class Function
	{
		public static void Analyze(AnalyzeOption opt)
		{
		    TextStatistic.MinRate = opt.MinRate;
		    TextStatistic.Strategy = opt.Strategy;
			if (opt.Merge)
				TextStatistic.AnalyzeFiles(opt.FilePaths, opt.OutputFile);
		    else
			    TextStatistic.AnalyzeFilesSeparately(opt.FilePaths, opt.OutputDir);
		}

	    public static void ConvStat(ConvStatOption opt)
	    {
	        if (opt.Merge)
	        {
	            var conv = new WordDataConverter();
	            foreach (var filePath in opt.FilePaths)
                    conv.Load(filePath, opt.Format);
	            conv.Save(opt.OutputFile);
	        }
	        else
	        {
	            foreach (var filePath in opt.FilePaths)
	            {
	                var fileInfo = new FileInfo(filePath);
	                var outFilePath = $"{opt.OutputDir ?? fileInfo.DirectoryName}/{fileInfo.Name}_stat.csv";
	                var conv = new WordDataConverter();
	                conv.Load(filePath, opt.Format);
	                conv.Save(outFilePath);
	            }
	        }
	    }

		public static void Merge(IEnumerable<string> filePaths, string outputFile, float rate = 0)
		{
		    TextStatistic.MinRate = rate;
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
			inputer.PrintDistributeSize = 5;
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

		public static void TestOnData(TestOption opt)
		{
		    IEnumerable<NGramModelBase> models;
		    if (opt.StatFiles == null)
		        models = opt.ModelNames.Select(NGramModelFileLoader.LoadByName);
		    else
		    {
		        models = from stat in opt.StatFiles.Select(path => new TextStatistician(path))
		            from model in opt.ModelNames.Select(name =>
		            {
		                var model = NGramModelFileLoader.NewByName(name);
		                Console.WriteLine($"Built model: [{name}] from [{stat.SourceName}]");
		                model.FromStatistician(stat);
		                return model;
		            })
		            select model;
		    }
			var inputers = models.Select(model => new NGramInputer(model)).Cast<FullPinyinInputer>().ToArray();
			var tester = new InputerTester(inputers);

			using (var inputFile = File.OpenText(opt.InputFile))
			{
				if (opt.OutputFile == null)
					tester.TestData(inputFile, Console.Out, opt.Format);
				else
					using (var outputFile = File.CreateText(opt.OutputFile))
						tester.TestData(inputFile, outputFile, opt.Format);
			}
		}
	}
}
