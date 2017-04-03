using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trie
{
	public class TrieCounter: IEnumerable<KeyValuePair<string, int>>
    {
		const char BEGIN_CHAR = '^';
		const char END_CHAR = '$';
		TrieNodeBase root = new TrieNodeBase(BEGIN_CHAR);

		public int this[string str]
		{
			get 
			{
				var node = GetNode(str);
				if (node == null)
					throw new KeyNotFoundException();
				return node.Count;
			}
		}

		TrieNodeBase GetNode(string str)
		{
			return root.GetNext(str)?.GetNext(END_CHAR);
		}

		public void Add(string str, int count = 1)
		{
			root.GetOrAdd(str).GetOrAdd(END_CHAR).Value = value;
		}

		public bool ContainsKey(string str)
		{
			return GetNode(str) != null;
		}

		internal void Debug_Dfs(TrieNodeBase node)
		{
			Console.WriteLine(node.Char);
			foreach (var edge in node.NextEdges)
				Debug_Dfs(edge.Value);
		}

		public void Debug_Dfs()
		{
			Debug_Dfs(root);
		}

		IEnumerable<KeyValuePair<string, TValue>> Dfs(TrieNodeBase node)
		{
			if (node == null)
				yield break;
			foreach (var edge in node.NextEdges)
			{
				if(edge.Key == END_CHAR)
					yield return new KeyValuePair<string, TValue>("", edge.Value.Value);
				else
					foreach (var pair in Dfs(edge.Value))
						yield return new KeyValuePair<string, TValue>(edge.Key + pair.Key, pair.Value);
			}
		}

		public IEnumerable<KeyValuePair<string, TValue>> GetAllStartsWith(string substr)
		{
			var node = root.GetNext(substr);
			return Dfs(node).Select(pair => new KeyValuePair<string, TValue>(substr + pair.Key, pair.Value));
		}

		public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
		{
			return Dfs(root).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
