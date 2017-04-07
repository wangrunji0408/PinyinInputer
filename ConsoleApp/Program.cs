using System.Collections.Generic;
using CommandLine;

namespace PinyinAnalyzer.ConsoleApp
{
	interface IOption
	{
		
	}

	[Verb("qpinyin", HelpText = "交互查询拼音数据库")]
	class QPinyinOption: IOption
	{
		[Value(0, MetaName = "FILE", HelpText = "数据库文件地址")]
		public string FilePath { get; set; }
	}

	[Verb("solve", HelpText = "输入拼音文件，输出汉字序列")]
	class SolveOption : IOption
	{
		[Value(0, Required = true, MetaName = "FILE")]
		public string InputFile { get; set; }

		[Value(1, Required = true, MetaName = "FILE")]
		public string OutputFile { get; set; }

		[Value(2, Default = "n", MetaName = "NAME")]
		public string ModelName { get; set; }
	}

	[Verb("qsolve", HelpText = "交互翻译拼音")]
	class QSolveOption : IOption
	{
		[Value(0, Default = "n", MetaName = "NAME")]
		public string ModelName { get; set; }
	}

	[Verb("qmodel", HelpText = "交互查询语言模型的概率分布")]
	class QModelOption : IOption
	{
		[Value(0, Default = "n", MetaName = "NAME")]
		public string ModelName { get; set; }
	}

	[Verb("qstat", HelpText = "交互查询文本统计结果")]
	class QStatOption : IOption
	{
		[Value(0, MetaName = "FILE")]
		public string FilePath { get; set; }
	}

	[Verb("analyze", HelpText = "执行文本统计")]
	class AnalyzeOption : IOption
	{
		[Value(0, MetaName = "FILE", Required = true, HelpText = "待统计的文件")]
		public IEnumerable<string> FilePaths { get; set; }

	    [Option('m', HelpText = "是否合并统计结果。若是，需指定输出文件地址。")]
	    public bool Merge { get; set; }

	    [Option('o', HelpText = "输出文件地址")]
		public string OutputFile { get; set; }

	    [Option('d', HelpText = "输出目录。若不指定则为每个文件的所在目录。")]
	    public string OutputDir { get; set; }

	    [Option('r', Default = 0, HelpText = "保存时去掉频率低于此的项")]
	    public float MinRate { get; set; }

//	    [Option('s', Default = "n", HelpText = "分析策略。1|2|3|n")]
	    public string Strategy { get; set; }
	}

	[Verb("merge", HelpText = "合并统计文件")]
	class MergeOption : IOption
	{
		[Value(0, MetaName = "FILE", Required = true, HelpText = "待合并的统计信息，csv格式")]
		public IEnumerable<string> FilePaths { get; set; }

		[Option('o', Required = true, HelpText = "合并后保存到的文件地址")]
		public string OutputFile { get; set; }

	    [Option('r', Default = 0, HelpText = "保存时去掉频率低于此的项")]
	    public float MinRate { get; set; }
	}

    [Verb("convstat", HelpText = "将外部词库文件转为统计文件")]
    class ConvStatOption : IOption
    {
        [Value(0, MetaName = "FILE", Required = true, HelpText = "待转换文件")]
        public IEnumerable<string> FilePaths { get; set; }

        [Option('m', HelpText = "是否合并统计结果。若是，需指定输出文件地址。")]
        public bool Merge { get; set; }

        [Option('o', HelpText = "输出文件地址")]
        public string OutputFile { get; set; }

        [Option('d', HelpText = "输出目录。若不指定则为每个文件的所在目录。")]
        public string OutputDir { get; set; }

        [Option('f', Default = "wordlist", Required = true, HelpText = "输入文件格式: 'wordlist' | 'wordcount' | 'wordcount_force1'")]
        public string Format { get; set; }
    }

	[Verb("build", HelpText = "根据统计文件生成语言模型")]
	class BuildOption : IOption
	{
		[Value(0, Required = true, HelpText = "统计信息")]
		public string StatFile { get; set; }
		[Option('m', HelpText = "模型名：1|2|3|n")]
		public IEnumerable<string> ModelNames { get; set; }
	}

	[Verb("test", HelpText = "在指定测试集上测试模型")]
	class TestOption : IOption
	{
	    [Option('m', Required = true, HelpText = "模型名：1|2|3|n|12m|12l|123l")]
	    public IEnumerable<string> ModelNames { get; set; }

	    [Option('s', HelpText = "统计文件。若指定，则现场生成模型。")]
	    public IEnumerable<string> StatFiles { get; set; }

	    [Value(0, Required = true, MetaName = "FILE")]
		public string InputFile { get; set; }

		[Value(1, Required = false, MetaName = "FILE")]
		public string OutputFile { get; set; }

	    [Option('f', Required = false, Default = "pinyin_chinese", HelpText = "测试输入格式 'pinyin_chinese': 拼音、中文交替, 'chinese_only': 每行只有中文")]
	    public string Format { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			var result = CommandLine.Parser.Default.ParseArguments
			                        <QPinyinOption, QSolveOption, QModelOption, QStatOption, SolveOption, AnalyzeOption, ConvStatOption, MergeOption, BuildOption, TestOption>(args);
			result.WithParsed<QPinyinOption>(opt => Function.QPinyin(opt.FilePath))
				  .WithParsed<QSolveOption>(opt => Function.QSolve(opt.ModelName))
				  .WithParsed<QModelOption>(opt => Function.QModel(opt.ModelName))
				  .WithParsed<QStatOption>(opt => Function.QStatistic(opt.FilePath))
				  .WithParsed<SolveOption>(opt => Function.Solve(opt.InputFile, opt.OutputFile, opt.ModelName))
				  .WithParsed<AnalyzeOption>(opt => Function.Analyze(opt))
		          .WithParsed<ConvStatOption>(opt => Function.ConvStat(opt))
				  .WithParsed<MergeOption>(opt => Function.Merge(opt.FilePaths, opt.OutputFile, opt.MinRate))
			      .WithParsed<BuildOption>(opt => Function.BuildModel(opt.StatFile, opt.ModelNames))
			      .WithParsed<TestOption>(opt => Function.TestOnData(opt));
			
		}
	}
}
