using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;

namespace PinyinAnalyzer.Test
{
	[TestFixture]
	public class TestModel
	{
		static string PATH_BASE = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		static string testInputPath = $"{PATH_BASE}test_in.txt";
		static string testOutputPath = $"{PATH_BASE}test_out.txt";

		public void TestOnData(NGramModelBase model)
		{
			PinyinDict pydict = model.PinyinDict;
			var inputer = new NGramInputer(model);
			var tester = new InputerTester(inputer, pydict);

			using (var inputFile = File.OpenText(testInputPath))
			//using (var outputFile = File.CreateText(testOutputPath))
				tester.TestData(inputFile, Console.Out);
		    Assert.Inconclusive("Check the output.");
		}

		public void BuildFromStat<TModel>()
			where TModel: NGramModelBase, new()
		{
			var stat = new TextStatistician($"{PATH_BASE}stat.csv");
			var model = new TModel();
			model.FromStatistician(stat);
			model.Save();
		}

		[Test]
		public void TestNGram1_Build()
		{
			BuildFromStat<NGram1Model>();
		}

		[Test]
		public void TestNGram2_Build()
		{
			BuildFromStat<NGram2Model>();
		}

		[Test]
		public void TestNGram3_Build()
		{
			BuildFromStat<NGram3Model>();
		}

		[Test]
		public void TestNGramN_Build()
		{
			BuildFromStat<NGramNModel>();
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
		public void TestNGram12_MaxN()
		{
			TestOnData(NGramModelFileLoader.Load12_MaxN());
		}

		[Test]
		public void TestNGram12_Lambda()
		{
			TestOnData(NGramModelFileLoader.Load12_Lambda());
		}

		[Test]
		public void TestNGram123_Lambda()
		{
			TestOnData(NGramModelFileLoader.Load123_Lambda());
		}

		[Test]
		public void TestNGramN()
		{
			TestOnData(NGramModelFileLoader.Load<NGramNModel>());
		}

		//[Test]
		//public void NGramWriteToCsv()
		//{
		//	var model = NGramModelFileLoader.Load<NGram2Model_DictDict>();
		//	using (var file = File.CreateText($"{PATH_BASE}csv2.csv"))
		//		model.WriteToCsv(file);
		//	using (var file = File.OpenText($"{PATH_BASE}csv2.csv"))
		//		model.ReadFromCsv(file, str => str);
		//}
	}
}
