using System;
using System.Collections.Generic;

namespace PinyinAnalyzer
{
	/// <summary>
	/// 通用输入法接口。支持交互输入字符串，并随时取得结果。
	/// </summary>
	public interface IInputer
	{
	    string Name { get; }

	    /// <summary>
		/// 清空输入缓冲区。开始新的输入。
		/// </summary>
		void Clear();

		/// <summary>
		/// 输入一个字符。
		/// 若输入单引号[']表示前后单独成字，例如：xi'an(西安)
		/// </summary>
		/// <returns>The input.</returns>
		/// <param name="c">输入的字符</param>
		void Input(char c);

		/// <value>输入缓冲区中的字符</value>
		string InputString { get;}

		/// <summary>
		/// 例如输入：womenailaodong（我们爱劳动）
		/// 可能输出：
		/// 1. 我们爱劳动
		/// 2. 我们爱老东
		/// </summary>
		/// <value>最终结果列表</value>
		IEnumerable<string> Results { get;}

		/// <summary>
		/// 例如输入：womenailaodong（我们爱劳动）
		/// 搜狗输入法给出的结果列表为：
		/// 1. 我们爱劳动
		/// 2. 我们来劳动
		/// 3. 我们爱
		/// 4. 我们
		/// 5. 我么
		/// </summary>
		/// <value>输入法实时结果列表</value>
		IEnumerable<string> SubResult { get; }

		/// <summary>
		/// 用户反馈，确认输入SubResult中的第index项。
		/// </summary>
		/// <param name="index">Index.</param>
		void ConfirmSubResult(int index);
	}
}
