using System;
using System.Collections.Generic;

namespace PinyinAnalyzer
{
	public abstract class PinyinInputer: IInputer
	{
		public abstract IEnumerable<string> Results { get; }
		public abstract void Input(string pinyin);


		public string InputString { get; private set; }

		public virtual void Clear()
		{
			InputString = "";
		}

		string _pinyin;

		public void Input(char c) 
		{
			InputString += c;
			if (c == 0 || c == ' ')
			{
				Input(_pinyin);
				_pinyin = "";
			}
			else
				_pinyin += c;
		}
	}
}
