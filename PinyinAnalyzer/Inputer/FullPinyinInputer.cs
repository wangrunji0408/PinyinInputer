using System;
using System.Collections.Generic;

namespace PinyinAnalyzer
{
	/// <summary>
	/// 完整拼音输入法。
	/// 一次只能输入一个字的完整拼音。以char为单位输入时，必须以单引号[']来标志一个字的拼音输入结束。
	/// </summary>
	public abstract class FullPinyinInputer: IInputer
	{
		public abstract IEnumerable<string> Results { get; }
		public abstract IEnumerable<string> SubResult { get; }
		public abstract void Input(string pinyin);
		public abstract void ConfirmSubResult(int index);

		public string InputString { get; private set; }
	    public virtual string Name => "FullPinyinInputer";

	    public virtual void Clear()
		{
			InputString = "";
			_pinyin = "";
		}

		string _pinyin = "";

		public void Input(char c) 
		{
			InputString += c;
			if (c == '\'')
			{
				Input(_pinyin);
				_pinyin = "";
			}
			else
				_pinyin += c;
		}
	}
}
