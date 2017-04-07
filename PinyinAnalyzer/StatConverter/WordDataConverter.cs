using System;
using System.IO;

namespace PinyinAnalyzer
{
    public class WordDataConverter
    {
        Statistic<string> Statistic { get; set; } = new Statistic<string>();

        void AddWord(string word, int count)
        {
            Statistic.Add("*", count);
            for(int i=1; i<=word.Length; ++i)
                Statistic.Add(word.Substring(0, i), count);
        }

        public void LoadThu(TextReader reader, bool forceOne = false)
        {
            while (reader.Peek() != -1)
            {
                var tokens = reader.ReadLine().Split('\t');
                var word = tokens[0].Trim();
                int count = forceOne? 1: int.Parse(tokens[1]);
                AddWord(word, count);
            }
        }

        public void LoadWordList(TextReader reader)
        {
            while (reader.Peek() != -1)
            {
                var word = reader.ReadLine().Trim();
                int count = 1;
                AddWord(word, count);
            }
        }

        public void LoadThu(string filePath, bool forceOne = false)
        {
            using (var file = File.OpenText(filePath))
                LoadThu(file, forceOne);
        }
        public void LoadWordList(string filePath)
        {
            using (var file = File.OpenText(filePath))
                LoadWordList(file);
        }
        public void Load(string filePath, string format = "wordlist")
        {
            using (var file = File.OpenText(filePath))
                switch (format)
                {
                    case "wordlist":
                        LoadWordList(file);
                        break;
                    case "wordcount":
                        LoadThu(file);
                        break;
                    case "wordcount_force1":
                        LoadThu(file, true);
                        break;
                    default:
                        throw new ArgumentException();
                }
        }
        public void Save(string filePath)
        {
            using (var file = File.CreateText(filePath))
                Statistic.WriteToCsv(file);
        }
    }
}