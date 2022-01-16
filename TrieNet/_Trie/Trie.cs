// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;

namespace Gma.DataStructures.StringSearch
{
    [Serializable]
    public class Trie<TValue> : TrieNode<TValue>, ITrie<TValue>
    {
        public IEnumerable<TValue> Retrieve(ReadOnlySpan<char> query)
        {
            return Retrieve(query, 0);
        }

        public void Add(ReadOnlyMemory<char> key, TValue value)
        {
            Add(key, 0, value);
        }
    }
}