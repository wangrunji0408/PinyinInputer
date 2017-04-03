using System;
using System.IO;
using System.Linq;
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

		[Option('o', HelpText = "（可选项）输出文件地址。若指定，则合并所有文件统计信息，保存到指定文件；若未指定，则对每个文件在旁边生成独自的统计信息。")]
		public string OutputFile { get; set; }

		public bool Merge => OutputFile != "";
	}

	[Verb("merge", HelpText = "合并统计文件")]
	class MergeOption : IOption
	{
		[Value(0, MetaName = "FILE", Required = true, HelpText = "待合并的统计信息，csv格式")]
		public IEnumerable<string> FilePaths { get; set; }

		[Value(1, Required = true, HelpText = "合并后保存到的文件地址")]
		public string OutputFile { get; set; }
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
		[Value(0, Required = true, HelpText = "模型名：1|2|3|n|12m|12l|123l")]
		public string ModelName { get; set; }

		[Value(1, Required = true, MetaName = "FILE")]
		public string InputFile { get; set; }

		[Value(2, Required = false, MetaName = "FILE")]
		public string OutputFile { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			var result = CommandLine.Parser.Default.ParseArguments
			                        <QPinyinOption, QSolveOption, QModelOption, QStatOption, SolveOption, AnalyzeOption, MergeOption, BuildOption, TestOption>(args);
			result.WithParsed<QPinyinOption>(opt => Function.QPinyin(opt.FilePath))
				  .WithParsed<QSolveOption>(opt => Function.QSolve(opt.ModelName))
				  .WithParsed<QModelOption>(opt => Function.QModel(opt.ModelName))
				  .WithParsed<QStatOption>(opt => Function.QStatistic(opt.FilePath))
				  .WithParsed<SolveOption>(opt => Function.Solve(opt.InputFile, opt.OutputFile, opt.ModelName))
				  .WithParsed<AnalyzeOption>(opt => Function.Analyze(opt.FilePaths, opt.OutputFile))
				  .WithParsed<MergeOption>(opt => Function.Merge(opt.FilePaths, opt.OutputFile))
			      .WithParsed<BuildOption>(opt => Function.BuildModel(opt.StatFile, opt.ModelNames))
			      .WithParsed<TestOption>(opt => Function.TestOnData(opt.ModelName, opt.InputFile, opt.OutputFile));
			
		}
	}
}
