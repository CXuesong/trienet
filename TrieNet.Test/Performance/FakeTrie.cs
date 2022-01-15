// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;

namespace Gma.DataStructures.StringSearch.Test
{
    public class FakeTrie<T> : ITrie<T>
    {
        private readonly Stack<KeyValuePair<string, T>> m_Stack;

        public FakeTrie()
        {
            m_Stack = new Stack<KeyValuePair<string, T>>();
        }

        public IEnumerable<T> Retrieve(ReadOnlySpan<char> query)
        {
            return RetrieveCore(query.ToString());

            IEnumerable<T> RetrieveCore(string sQuery)
            {
                foreach (var keyValuePair in m_Stack)
                {
                    string key = keyValuePair.Key;
                    T value = keyValuePair.Value;
                    if (key.Contains(sQuery)) yield return value;
                }
            }
        }

        public void Add(string key, T value)
        {
            var keyValPair = new KeyValuePair<string, T>(key, value);
            m_Stack.Push(keyValPair);
        }
    }
}