using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PinyinAnalyzer.ConsoleApp
{
	class Program
	{
		static string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		static string statPath = $"{path}count1.txt";
		static string pinyinPath = $"{path}拼音汉字表/拼音汉字表.txt";
		static string inputPath = $"{path}input.txt";
		static string outputPath = $"{path}output.txt";

		static public void AnalyzeData(TextAnalyzer analyzer)
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
			using (var fileWriter = File.CreateText(statPath))
			{
				DateTime t0 = DateTime.Now;
				new JsonSerializer().Serialize(fileWriter, analyzer);
				TimeSpan ts = DateTime.Now - t0;
				Console.WriteLine($"Write End: count.txt Time = {ts}");
			}
		}

		static void Trans()
		{
			TextAnalyzer analyzer;
			using (var fileReader = File.OpenText(statPath))
			{
				Console.WriteLine("Loading statistic file...");
				analyzer = new JsonSerializer().Deserialize<TextAnalyzerBinary>(new JsonTextReader(fileReader));
				Console.WriteLine("Success to statistic file.");
			}
			var pydic = new PinyinDict(pinyinPath);
			var inputer = new BinaryInputer(analyzer, pydic);
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

		static void Interact()
		{
			TextAnalyzer analyzer;
			using (var fileReader = File.OpenText(statPath))
			{
				Console.WriteLine("Loading statistic file...");
				analyzer = new JsonSerializer().Deserialize<TextAnalyzerBinary>(new JsonTextReader(fileReader));
				Console.WriteLine("Success to statistic file.");
			}
			var pydic = new PinyinDict(pinyinPath);
			var inputer = new BinaryInputer(analyzer, pydic);
			while(true)
			{
				Console.Write("> ");
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

		static void Pinyin()
		{
			var pydict = new PinyinDict(pinyinPath);
			while (true)
			{
				Console.Write("> ");
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

		static void Main(string[] args)
		{
			if (args.Length == 1)
			{
				switch (args[0])
				{
					case "pinyin": Pinyin(); break;
					case "input": Interact(); break;
					case "analyze2": AnalyzeData(new TextAnalyzerBinary()); break;
				}
			}
		}
	}
}
