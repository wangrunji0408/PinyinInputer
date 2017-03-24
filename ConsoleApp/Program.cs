using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer.ConsoleApp
{
	class Program
	{
		static string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		static string ngram1Path = $"{path}count1.txt";
		static string ngram2Path = $"{path}count2.txt";
		static string pinyinPath = $"{path}拼音汉字表/拼音汉字表.txt";
		static string inputPath = $"{path}input.txt";
		static string outputPath = $"{path}output.txt";

		static void AnalyzeData(NGramModel analyzer, string dataPath)
		{
			for (int i = 1; i <= 11; ++i)
			{
				string filePath = $"{path}sina_news/2016-{i:00}.txt";
				Console.WriteLine($"Analyzing: 2016-{i:00}.txt");
				using (var fileReader = File.OpenText(filePath))
				{
					DateTime t0 = DateTime.Now;
					analyzer.Analyze(fileReader);
					TimeSpan ts = DateTime.Now - t0;
					Console.WriteLine($"Analyze End: 2016-{i:00}.txt Time = {ts}");
				}
			}
			using (var fileWriter = File.CreateText(dataPath))
			{
				DateTime t0 = DateTime.Now;
				new JsonSerializer().Serialize(fileWriter, analyzer);
				TimeSpan ts = DateTime.Now - t0;
				Console.WriteLine($"Write End. Time = {ts}");
			}
		}

		static NGramModel LoadModel2()
		{
			NGramModel model;
			using (var fileReader = File.OpenText(ngram2Path))
			{
				Console.WriteLine("Loading statistic file...");
				model = new JsonSerializer().Deserialize<NGram2Model>(new JsonTextReader(fileReader));
				Console.WriteLine("Success to load statistic file.");
			}
			model.PinyinDict = new PinyinDict(pinyinPath);
			return model;
		}

		static void InputFile()
		{
			NGramModel model = LoadModel2();
			var inputer = new NGramInputer(model);
			using (var outputWriter = File.CreateText(outputPath))
			{
				foreach (string input in File.ReadLines(inputPath))
				{
					inputer.Clear();
					foreach (string pinyin in input.Split())
						inputer.Input(pinyin);
					outputWriter.WriteLine(inputer.Results.First());
				}
			}
		}

		static void Input()
		{
			NGramModel model = new NGram123Model();
			model.PinyinDict = new PinyinDict(pinyinPath);
			var inputer = new NGramInputer(model);
			Console.WriteLine($"Using model: {model.GetType().Name}");

			while(true)
			{
				Console.Write("input > ");
				var input = Console.ReadLine();
				try
				{
					inputer.Clear();
					foreach (string pinyin in input.Split())
						inputer.Input(pinyin);
					Console.WriteLine(inputer.Results.First());
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		static void AskPinyin()
		{
			var pydict = new PinyinDict(pinyinPath);
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

		static void AskDistributeData()
		{
			NGramModel ng1, ng2;

			Console.WriteLine("Loading statistic data...");
			using (var fileReader = File.OpenText(ngram1Path))
				ng1 = new JsonSerializer().Deserialize<NGram1Model>(new JsonTextReader(fileReader));
			using (var fileReader = File.OpenText(ngram2Path))
				ng2 = new JsonSerializer().Deserialize<NGram2Model>(new JsonTextReader(fileReader));
			Console.WriteLine("Success to load statistic data.");

			Console.WriteLine("Loading pinyin data...");
			PinyinDict pydict = new PinyinDict(pinyinPath);
			Console.WriteLine("Success to load pinyin data.");
			ng1.PinyinDict = pydict;
			ng2.PinyinDict = pydict;

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
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

			}
		}

		static void Main(string[] args)
		{
			if (args.Length >= 1)
			{
				switch (args[0])
				{
					case "pinyin": AskPinyin(); break;
					case "input": Input(); break;
					case "data": AskDistributeData(); break;
					case "analyze1": AnalyzeData(new NGram1Model(), ngram1Path); break;
					case "analyze2": AnalyzeData(new NGram2Model(), ngram2Path); break;
				}
			}
		}
	}
}
