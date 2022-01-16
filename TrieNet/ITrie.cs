// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;

namespace Gma.DataStructures.StringSearch
{
    /// <summary>
    /// Interface to be implemented by a data structure 
    /// which allows adding values <see cref="TValue"/> associated with <b>string</b> keys.
    /// The interface allows retrieval of multiple values.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ITrie<TValue> : IReadOnlyTrie<TValue>
    {
        /// <summary>
        /// Adds a new item to the trie.
        /// </summary>
        /// <param name="key">key. The underlying value of key is supposed to be immutable, or the trie behavior will be undefined.</param>
        /// <param name="value">value.</param>
        void Add(ReadOnlyMemory<char> key, TValue value);

        void Add(string key, TValue value)
        {
            Add(key.AsMemory(), value);
        }
    }

    public interface IReadOnlyTrie<out TValue>
    {
        IEnumerable<TValue> Retrieve(ReadOnlySpan<char> query);

        IEnumerable<TValue> Retrieve(ReadOnlyMemory<char> query) => Retrieve(query.Span);

        IEnumerable<TValue> Retrieve(string query) => Retrieve(query.AsSpan());

    }
}