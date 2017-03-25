using System;
using System.IO;
using NUnit.Framework;

namespace PinyinAnalyzer.Test
{
	[TestFixture]
	public class TestModel
	{
		static string PATH_BASE = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		static string pinyinPath = $"{PATH_BASE}拼音汉字表/拼音汉字表.txt";
		static string inputPath = $"{PATH_BASE}input.txt";
		static string outputPath = $"{PATH_BASE}output.txt";
		static string testInputPath = $"{PATH_BASE}test_in.txt";
		static string testOutputPath = $"{PATH_BASE}test_out.txt";

		public void TestOnData(NGramModel model)
		{
			PinyinDict pydict = model.PinyinDict;
			var inputer = new NGramInputer(model);
			var tester = new InputerTester(inputer, pydict);

			using (var inputFile = File.OpenText(testInputPath))
			//using (var outputFile = File.CreateText(testOutputPath))
				tester.TestData(inputFile, Console.Out);
			Assert.Warn("Check the output.");
		}

		[Test]
		public void TestNGram1()
		{
			TestOnData(NGramModelFileLoader.Load<NGram1Model>());
		}

		[Test]
		public void TestNGram2()
		{
			TestOnData(NGramModelFileLoader.Load<NGram2Model>());
		}

		[Test]
		public void TestNGram12()
		{
			TestOnData(NGramModelFileLoader.Load12());
		}

		[Test]
		public void TestNGram12_Lambda()
		{
			TestOnData(NGramModelFileLoader.Load12_Lambda());
		}
	}
}
