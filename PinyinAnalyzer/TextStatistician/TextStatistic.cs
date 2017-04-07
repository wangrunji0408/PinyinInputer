using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	public static class TextStatistic
	{
	    public static float MinRate { get; set; } = 1e-6f;
	    public static string Strategy { get; set; } = "n";

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
			statistician.RemoveLowFrequency((int)(statistician.Total * MinRate));
			DoAndPrintTime(() => statistician.Save(statPath), $"Writing to file: {statPath}");
		}

		public static void AnalyzeFilesSeparately(IEnumerable<string> filePaths, string outputDir = null)
		{
			foreach (var filePath in filePaths)
			{
				var fileInfo = new FileInfo(filePath);
				var outFilePath = $"{outputDir ?? fileInfo.DirectoryName}/{fileInfo.Name}_stat.csv";
				var statistician = new TextStatistician();
				DoAndPrintTime(() => 
				{ 
					statistician.AnalyzeFile(filePath);
					statistician.RemoveLowFrequency((int)(statistician.Total * MinRate));
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
		    DoAndPrintTime(() => stat.RemoveLowFrequency((int) (stat.Total * MinRate)), $"Remove low frequency ({MinRate})");
			DoAndPrintTime(() => stat.Save(outFilePath), $"Save to {outFilePath}");
		}

		public static void RemoveLowFrequency(string statPath)
		{
			DoAndPrintTime(() =>
			{
				var statistician = new TextStatistician();
				statistician.Load(statPath);
				statistician.RemoveLowFrequency((int)(statistician.Total * MinRate));
				statistician.Save(statPath);
			}, $"Remove low frequency ({MinRate}): {statPath}");
		}
	}
}
