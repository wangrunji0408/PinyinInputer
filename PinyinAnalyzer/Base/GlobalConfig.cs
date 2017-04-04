using Microsoft.Extensions.Configuration;

namespace PinyinAnalyzer
{
    public static class GlobalConfig
    {
        private static IConfiguration config = new ConfigurationBuilder().AddJsonFile("PinyinAnalyzer.config.json").Build();
        public static string DefaultPinyinFile => config["DefaultPinyinFile"];
        public static string ModelDirectory => config["ModelDirectory"];
    }
}