using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Xunit.Sdk;

namespace PinyinAnalyzer.Test
{
	public class DataAnalyze
	{
		//public static int RareCharCount(this Statistic<char> stat, float rareRate)
		//{
		//	if (rareRate < 0 || rareRate > 1)
		//		throw new ArgumentOutOfRangeException(nameof(rareRate));
		//	int af = 0; // AccumulationFrequency
		//	int maxf = (int)(rareRate * stat.Total);
		//	int count = 0;
		//	foreach (var pair in stat.KeyFrequencyAscending)
		//	{
		//		af += pair.Value;
		//		count++;
		//		if (af >= maxf)
		//			return count;
		//	}
		//	throw new Exception("Should not reach there.");
		//}

		//[Test]
		//public void AnalyzeRareCharRate()
		//{
		//	float rareRate = 0.01f;
		//	var model = NGramModelFileLoader.Load<NGram3Model>();
		//	int count = 0, total = 0;
		//	foreach (var pair in model.Statistics)
		//	{
		//		Statistic<char> stat = pair.Value;
		//		count += stat.RareCharCount(rareRate);
		//		total += stat.Count;
		//	}
		//	Console.WriteLine($"RareRate = {rareRate}\nCount = {count} Total = {total} Rate = {(float)count / total}");
		//}

		//[Test]
		//public void AnalyzeStatisticTotal()
		//{
		//	var totalCount = new Dictionary<int, int>();
		//	var model = NGramModelFileLoader.Load<NGram3Model>();
		//	foreach (var pair in model.Statistics)
		//	{
		//		Statistic<char> stat = pair.Value;
		//		if (totalCount.ContainsKey(stat.Total))
		//			totalCount[stat.Total] += 1;
		//		else
		//			totalCount.Add(stat.Total, 1);
		//	}
		//	using (var file = File.CreateText("/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/AnalyzeStatisticTotal.txt"))
		//	{
		//		foreach (var pair in totalCount)
		//			file.WriteLine($"{pair.Key}, {pair.Value}");
		//	}
		//}
	}
}
