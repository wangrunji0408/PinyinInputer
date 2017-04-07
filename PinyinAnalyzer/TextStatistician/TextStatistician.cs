using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PinyinAnalyzer
{
	/// <summary>
	/// 文本频率统计器
	/// </summary>
	public class TextStatistician
	{
	    public string SourceName { get; private set; }
	    Statistic<string> Statistic { get; set; } = new Statistic<string>();
	    public float MinRate { get; set; } = 1e-6f;

	    public int Total => Statistic["*"];

		public TextStatistician()
		{
		}

		public TextStatistician(string filePath)
		{
			Load(filePath);
		}

		IEnumerable<string> SubStringSelector(string str)
		{
			//return CharPairSelector(str, 3);
			return DynamicSubStringSelector(str);
		}

		IEnumerable<string> CharPairSelector(string str, int maxK = 2)
		{
			for (int k = 1; k <= maxK; ++k)
	 			for (int i = 0; i <= str.Length - k; ++i)
					yield return str.Substring(i, k);
		}

		IEnumerable<string> DynamicSubStringSelector(string str)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < str.Length; ++i)
			{
				builder.Clear();
				builder.Append(str[i]);
				yield return builder.ToString();
				for (int j = i + 1; j < str.Length; ++j)
				{
					if (Statistic[builder.ToString()] < Math.Max(5, Total * MinRate))
						break;
					builder.Append(str[j]);
					yield return builder.ToString();
				}
			}
		}

		public bool InCharSet(char c)
		{
			const char CHINESE_CHAR_MIN = (char)0x4e00;
			const char CHINESE_CHAR_MAX = (char)0x9fbb;
			// const int CHINESE_CHAR_COUNT = CHINESE_CHAR_MAX - CHINESE_CHAR_MIN + 1;
			return c >= CHINESE_CHAR_MIN && c <= CHINESE_CHAR_MAX;
		}

		public void Analyze(TextReader reader) 
		{
			var builder = new StringBuilder('^');
			while (reader.Peek() != -1)
			{
				var c = (char)reader.Read();
				if (!InCharSet(c))
				{
					if (builder.Length >= 2)
					{
						builder.Append('$');
						Statistic.Add("*", builder.Length);
						foreach (var str in SubStringSelector(builder.ToString()))
							Statistic.Add(str);
					}
					
					builder.Clear();
					builder.Append('^');
				}
				else
					builder.Append(c);
			}
		}
		public void Analyze(string str)
		{
			Analyze(new StringReader(str));
		}
		public void AnalyzeFile(string filePath)
		{
			using (var file = File.OpenText(filePath))
				Analyze(file);
		}
		public void AnalyzeFiles(IEnumerable<string> filePaths)
		{
			foreach (var filePath in filePaths)
				AnalyzeFile(filePath);
		}

		public int GetStringCount(string str)
		{
			return Statistic[str];
		}

		public IEnumerable<KeyValuePair<string, int>> StringFrequency => Statistic.KeyFrequency;

		public void RemoveLowFrequency(int minf)
		{
			Statistic = Statistic.Where(pair => pair.Value >= minf);
		}

		public void MergeFrom(TextStatistician another)
		{
			foreach (var pair in another.Statistic.KeyFrequency)
				Statistic.Add(pair.Key, pair.Value);
		}

		public void Load(string filePath)
		{
		    SourceName = new FileInfo(filePath).Name;
			using (var file = File.OpenText(filePath))
				Statistic.ReadFromCsv(file, str => str);
		}
		public void Save(string filePath)
		{
			using (var file = File.CreateText(filePath))
				Statistic.WriteToCsv(file);
		}
	}
}
