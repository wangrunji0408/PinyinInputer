using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using Newtonsoft.Json;


namespace PinyinAnalyzer.Test
{
	[TestFixture()]
	public class Test
	{
		[Test]
		public void TestDirectory()
		{
			//var path = Directory.GetDirectoryRoot("/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/sina_news/");
			//Console.WriteLine(path);
		}

		[Test]
		public void TestCondition()
		{
			var cond1 = new Condition("人智导");
			var cond = cond1;
			Assert.AreEqual(cond.N, 3);
			Assert.AreEqual(cond.Chars, "人智导");
			cond = cond.Reduce();
			Assert.AreEqual(cond.Chars, "智导");
			cond = cond.Reduce();
			Assert.AreEqual(cond.Chars, "导");
			Assert.AreEqual(cond.Chars.LastSubString(2, '^'), "^导");
			Assert.AreEqual(cond.Chars.LastSubString(0, '^'), "");
			cond = cond.Reduce();
			Assert.AreEqual(cond.Chars, "");
			Assert.AreEqual(cond1.Chars, "人智导");
			Assert.Throws<InvalidOperationException>(() => cond = cond.Reduce());
		}

		[Test()]
		public void TestPinyinDict()
		{
			var pydic = new PinyinDict("/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/拼音汉字表/拼音汉字表.txt");
			Assert.True(pydic.GetChars("you").Contains('有'));
			Assert.True(!pydic.GetChars("you").Contains('没'));
			Assert.True(pydic.GetPinyins('行').Contains("hang"));
		}

		//[Test]
		//public void TestModel2()
		//{ 
		//	var model = new NGram2Model_DictDict();
		//	model.Analyze("而且，候选人需获得三分之二选票才能当选，否则需举行第二轮投票。");
		//	Console.WriteLine(JsonConvert.SerializeObject(model));
		//	Assert.Warn("Check the output.");
		//}

		//[Test]
		//public void TestModel3()
		//{
		//	var model = new NGram3Model();
		//	model.Analyze("而且，候选人需获得三分之二选票才能当选，否则需举行第二轮投票。");
		//	Console.WriteLine(JsonConvert.SerializeObject(model));
		//	Assert.Warn("Check the output.");
		//}

		[Test]
		public void TestDistribute()
		{
			var d1 = new Distribute<int>(new Dictionary<int, float>
			{
				[1] = 0.2f,
				[2] = 0.5f,
				[3] = 0.3f
			});
			var fd = new Dictionary<int, Distribute<int>>
			{
				[1] = Distribute<int>.Single(2),
				[2] = Distribute<int>.Single(3),
				[3] = Distribute<int>.Single(4)
			};
			Distribute<int> d2 = d1.ExpandAndMerge(key => fd[key]);
			d2.Print();
			Assert.Warn("Check the output.");
		}

		[Test, Category("Basic")]
		public void TestFormat()
		{
			int i = 1;
			Assert.AreEqual($"{i:00}", "01");
			i = 11;
			Assert.AreEqual($"{i:00}", "11");
		}

		[Test, Category("Basic")]
		public void TestJsonWriter()
		{
			using (var file = File.CreateText("/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/JsonWriterTest.txt"))
			{
				var writer = new JsonTextWriter(file);
				writer.WriteStartObject();
				writer.WritePropertyName("name1");
				writer.WriteValue(1);
				writer.WritePropertyName("array");
				writer.WriteStartArray();
				writer.WriteValue(123);
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
		}
	}
}
