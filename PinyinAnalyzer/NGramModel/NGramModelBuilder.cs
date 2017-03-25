using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	public static class NGramModelBuilder
	{
		static string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		static string pinyinPath = $"{path}拼音汉字表/拼音汉字表.txt";
		static Dictionary<Type, string> ngramPath = new Dictionary<Type, string>
		{
			[typeof(NGram1Model)] = $"{path}count1.txt",
			[typeof(NGram2Model)] = $"{path}count2.txt",
			[typeof(NGram3Model)] = $"{path}count3.txt",
		};
		static PinyinDict pydict = new PinyinDict(pinyinPath);

		public static TModel LoadFromDefaultFile<TModel> ()
			where TModel: NGramModel
		{
			TModel model;
			using (var fileReader = File.OpenText(ngramPath[typeof(TModel)]))
			{
				Console.WriteLine($"Loading {typeof(TModel).Name} from file...");
				model = new JsonSerializer().Deserialize<TModel>(new JsonTextReader(fileReader));
				Console.WriteLine("Success.");
			}
			model.PinyinDict = pydict;
			return model;
		}

		public static NGram123Model Load12FromDefaultFile()
		{
			var ng1 = LoadFromDefaultFile<NGram1Model>();
			var ng2 = LoadFromDefaultFile<NGram2Model>();
			var ng3 = new NGram3Model();
			var model = new NGram123Model(ng1, ng2, ng3);
			model.PinyinDict = pydict;
			return model;
		}
	}
}
