using System.Collections.Generic;
using System;

namespace Trie
{
	internal class TrieNode
	{
		protected Dictionary<char, TrieNode> next = new Dictionary<char, TrieNode>();
		internal char Char { get; }

		internal bool IsLeaf => next.Count == 0;
		internal IEnumerable<KeyValuePair<char, TrieNode>> NextEdges => next;
		internal ICollection<char> NextChars => next.Keys;

		internal TrieNode(char c)
		{
			Char = c;
		}

		protected virtual TrieNode NewNode(char c)
		{
			return new TrieNode(c);
		}

		internal TrieNode AddNext(char c)
		{
			var node = NewNode(c);
			this.next.Add(c, node);
			return node;
		}

		internal TrieNode GetNext(char c)
		{
			TrieNode node = null;
			next.TryGetValue(c, out node);
			return node;
		}

		internal TrieNode GetOrAdd(char c)
		{
			return GetNext(c) ?? AddNext(c);
		}

		internal TrieNode GetOrAdd(IEnumerable<char> chars)
		{
			var node = this;
			foreach (var c in chars)
				node = node.GetOrAdd(c);
			return node;
		}

		internal TrieNode GetNext(IEnumerable<char> chars)
		{
			var node = this;
			foreach (var c in chars)
			{
				node = node.GetNext(c);
				if (node == null)
					break;
			}
			return node;
		}
	}
}
