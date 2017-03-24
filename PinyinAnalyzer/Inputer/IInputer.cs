using System;
using System.Collections.Generic;

namespace PinyinAnalyzer
{
	public interface IInputer
	{
		void Clear();
		void Input(char c);
		string InputString { get;}
		IEnumerable<string> Results { get;}
	}
}
