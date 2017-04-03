using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinyinAnalyzer
{
	public class NGram3Model: NGramModel
	{
		public NGram3Model(): base(3)
		{
			
		}
	}
}
