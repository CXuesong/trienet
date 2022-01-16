﻿// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;

namespace Gma.DataStructures.StringSearch
{
    [Serializable]
    public class PatriciaTrie<TValue> :
        PatriciaTrieNode<TValue>,
        ITrie<TValue>
    {
        public PatriciaTrie()
            : base(
                new StringPartition(string.Empty),
                new Queue<TValue>(),
                new Dictionary<char, PatriciaTrieNode<TValue>>())
        {
        }

        public IEnumerable<TValue> Retrieve(ReadOnlySpan<char> query)
        {
            return Retrieve(query, 0);
        }

        public virtual void Add(ReadOnlyMemory<char> key, TValue value)
        {
            Add(new StringPartition(key), value);
        }

        internal override void Add(StringPartition keyRest, TValue value)
        {
            GetOrCreateChild(keyRest, value);
        }
    }
}