using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace PinyinAnalyzer
{
	public static class TextStatistic
	{
		//static string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		//static string statPath = $"{path}stat.csv";
		//static Logger logger = LogManager.GetCurrentClassLogger();

		static void DoAndPrintTime(Action action, string title = "")
		{
			if (title == "")
				title = action.ToString();
			Console.WriteLine($"Begin: {title}");
			DateTime t0 = DateTime.Now;
			action();
			TimeSpan ts = DateTime.Now - t0;
			Console.WriteLine($"End. Time = {ts}");
		}

		static void AnalyzeFiles(TextStatistician statistician, IEnumerable<string> filePaths)
		{
			foreach (var filePath in filePaths)
				DoAndPrintTime(() => statistician.AnalyzeFile(filePath),
							   $"Analyze: {filePath}");
		}

		public static void AnalyzeFiles(IEnumerable<string> filePaths, string statPath, bool append = false)
		{
			var statistician = new TextStatistician();
			if(append)
				statistician.Load(statPath);
			AnalyzeFiles(statistician, filePaths);
			statistician.RemoveLowFrequency((int)(statistician.Total * 1e-6));
			DoAndPrintTime(() => statistician.Save(statPath), $"Writing to file: {statPath}");
		}

		public static void AnalyzeFilesSeparately(IEnumerable<string> filePaths)
		{
			foreach (var filePath in filePaths)
			{
				var fileInfo = new FileInfo(filePath);
				var outFilePath = $"{fileInfo.Directory}/{fileInfo.Name}_stat.csv";
				var statistician = new TextStatistician();
				DoAndPrintTime(() => 
				{ 
					statistician.AnalyzeFile(filePath);
					statistician.RemoveLowFrequency((int)(statistician.Total * 1e-6));
					statistician.Save(outFilePath);
				}, $"Analyze: {filePath}");
			}
		}

		public static void MergeFiles(IEnumerable<string> filePaths, string outFilePath)
		{
			var stat = new TextStatistician();
			foreach (var filePath in filePaths)
				DoAndPrintTime(() => stat.MergeFrom(new TextStatistician(filePath)),
				               $"Merge: {filePath}");
			DoAndPrintTime(() => stat.Save(outFilePath), $"Save to {outFilePath}");
		}

		public static void RemoveLowFrequency(string statPath)
		{
			float rate = 1e-6f;
			DoAndPrintTime(() =>
			{
				var statistician = new TextStatistician();
				statistician.Load(statPath);
				statistician.RemoveLowFrequency((int)(statistician.Total * rate));
				statistician.Save(statPath);
			}, $"Remove low frequency ({rate}): {statPath}");
		}
	}
}
