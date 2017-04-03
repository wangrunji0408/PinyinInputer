using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PinyinAnalyzer
{
	public static class NGramModelFileLoader
	{
		static readonly string path = "/Users/wangrunji/Documents/大学文件/大二下/课程文件/人工智能导论/拼音输入法/";
		//static readonly Dictionary<Type, string> ngramPath = new Dictionary<Type, string>
		//{
		//	[typeof(NGram1Model)] = $"{path}new_1gram.txt",
		//    [typeof(NGram2Model)] = $"{path}new_2gram_1.txt",
		//	[typeof(NGram2Model_DictDict)] = $"{path}new_2gram.txt",
		//	[typeof(NGram3Model)] = $"{path}new_3gram.txt",
		//};
		//public static PinyinDict pydict { get; } = new PinyinDict(pinyinPath);

		//static Logger logger = LogManager.GetCurrentClassLogger();

		static string GetPath(Type type)
		{
			return $"{path}{type.Name}.txt";
		}

		public static NGramModelBase NewByName(string modelName)
		{
			switch (modelName)
			{
				case "1": return new NGram1Model();
				case "2": return new NGram2Model();
				case "3": return new NGram3Model();
				case "n": return new NGramNModel();
				default: throw new ArgumentException();
			}
		}

		public static NGramModelBase LoadByName(string modelName)
		{
			switch (modelName)
			{
				case "1": return Load<NGram1Model>();
				case "2": return Load<NGram2Model>();
				case "3": return Load<NGram3Model>();
				case "12m": return Load12_MaxN();
				case "12l": return Load12_Lambda();
				case "123l": return Load123_Lambda();
				case "n": return Load<NGramNModel>();
				default: throw new ArgumentException();
			}
			//var type = Type.GetType(modelName);
			//object model;
			//using (var fileReader = File.OpenText(GetPath(type)))
			//{
			//	Console.WriteLine($"Loading {modelName} from file...");
			//	model = new JsonSerializer().Deserialize(fileReader, type);
			//	Console.WriteLine($"Success.");
			//}
			//return model as NGramModelBase;
		}

		public static TModel Load<TModel> ()
			where TModel: NGramModelBase
		{
			TModel model;
			using (var fileReader = File.OpenText(GetPath(typeof(TModel))))
			{
				Console.WriteLine($"Loading {typeof(TModel).Name} from file...");
				model = new JsonSerializer().Deserialize<TModel>(new JsonTextReader(fileReader));
				Console.WriteLine($"Success.");
			}
			return model;
		}

	    public static void Save<TModel>(this TModel model)
	        where TModel: NGramModelBase
	    {
			using (var fileWriter = File.CreateText(GetPath(model.GetType())))
	        {
				Console.WriteLine($"Saving {model.GetType().Name} to file...");
	            new JsonSerializer().Serialize(fileWriter, model);
				Console.WriteLine("Success.");
	        }
	    }

	    public static NGramBindModel Load12_MaxN()
		{
			var ng1 = Load<NGram1Model>();
			var ng2 = Load<NGram2Model>();
			var model = new NGramBindModel(new NGramModelBase[] { ng1, ng2 });
			//model.PinyinDict = pydict;
			return model;
		}

		public static NGramBindModel Load12_Lambda()
		{
			var ng1 = Load<NGram1Model>();
			var ng2 = Load<NGram2Model>();
			var model = new NGramBindModel(new NGramModelBase[] { ng1, ng2 });
			//model.PinyinDict = pydict;
			model.MixDistributeStrategy = NGramBindModel.MixDistributeStrategy_Lambda;
			return model;
		}

		public static NGramBindModel Load123_Lambda()
		{
			var ng1 = Load<NGram1Model>();
			var ng2 = Load<NGram2Model>();
			var ng3 = Load<NGram3Model>();
			var model = new NGramBindModel(new NGramModelBase[] { ng1, ng2, ng3 });
			//model.PinyinDict = pydict;
			model.MixDistributeStrategy = NGramBindModel.MixDistributeStrategy_Lambda;
			return model;
		}
	}
}
