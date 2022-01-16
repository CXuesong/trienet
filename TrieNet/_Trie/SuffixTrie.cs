// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gma.DataStructures.StringSearch
{
    [Serializable]
    public class SuffixTrie<T> : ITrie<T>
    {
        private readonly Trie<T> m_InnerTrie;
        private readonly int m_MinSuffixLength;

        public SuffixTrie(int minSuffixLength)
            : this(new Trie<T>(), minSuffixLength)
        {
        }

        private SuffixTrie(Trie<T> innerTrie, int minSuffixLength)
        {
            m_InnerTrie = innerTrie;
            m_MinSuffixLength = minSuffixLength;
        }

        public IEnumerable<T> Retrieve(ReadOnlySpan<char> query)
        {
            return
                m_InnerTrie
                    .Retrieve(query)
                    .Distinct();
        }

        public void Add(ReadOnlyMemory<char> key, T value)
        {
            foreach (var suffix in GetAllSuffixes(m_MinSuffixLength, key))
            {
                m_InnerTrie.Add(suffix, value);
            }
        }

        private static IEnumerable<ReadOnlyMemory<char>> GetAllSuffixes(int minSuffixLength, ReadOnlyMemory<char> word)
        {
            for (int i = word.Length - minSuffixLength; i >= 0; i--)
            {
                yield return word[i..];
            }
        }
    }
}