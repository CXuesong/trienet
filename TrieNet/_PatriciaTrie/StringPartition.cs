// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
#nullable enable
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Gma.DataStructures.StringSearch
{
    [Serializable]
    [DebuggerDisplay("{_Data.ToString()}")]
    public readonly struct StringPartition : IEnumerable<char>
    {
        private readonly ReadOnlyMemory<char> _Data;

        public StringPartition(string? origin)
        {
            _Data = origin.AsMemory();
        }

        public StringPartition(string? origin, int startIndex)
            : this(origin, startIndex, origin == null ? 0 : origin.Length - startIndex)
        {
        }

        public StringPartition(string? origin, int startIndex, int partitionLength)
        {
            // TODO compat.
            _Data = origin?.AsMemory(startIndex, Math.Min(partitionLength, origin.Length - startIndex)) ?? default;
        }

        public StringPartition(ReadOnlyMemory<char> origin)
        {
            _Data = origin;
        }

        public StringPartition(ReadOnlyMemory<char> origin, int startIndex)
        {
            _Data = origin[startIndex..];
        }

        public StringPartition(ReadOnlyMemory<char> origin, int startIndex, int partitionLength)
        {
            _Data = Slice(origin, startIndex, partitionLength);
        }

        // Slices Memory<char> in a StringPartition-compatible way.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char> Slice(ReadOnlyMemory<char> origin, int startIndex, int partitionLength)
        {
            return origin.Slice(startIndex, Math.Min(partitionLength, origin.Length - startIndex));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Slice(ReadOnlySpan<char> origin, int startIndex, int partitionLength)
        {
            return origin.Slice(startIndex, Math.Min(partitionLength, origin.Length - startIndex));
        }

        public char this[int index] => _Data.Span[index];

        public int Length => _Data.Length;

        #region IEnumerable<char> Members

        public Enumerator GetEnumerator() => new Enumerator(_Data);

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => new Enumerator(_Data);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_Data);

        #endregion

        public bool Equals(StringPartition other)
        {
            return _Data.Span.Equals(other._Data.Span, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is StringPartition p && Equals(p);
        }

        public override int GetHashCode()
        {
            return string.GetHashCode(_Data.Span, StringComparison.Ordinal);
        }

        public bool StartsWith(StringPartition other) => StartsWith(other._Data.Span);

        public bool StartsWith(ReadOnlySpan<char> other)
        {
            if (Length < other.Length)
            {
                return false;
            }

            for (int i = 0; i < other.Length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public SplitResult Split(int splitAt)
        {
            var head = new StringPartition(_Data[..splitAt]);
            var rest = new StringPartition(_Data[splitAt..]);
            return new SplitResult(head, rest);
        }

        public ZipResult ZipWith(StringPartition other)
        {
            int splitIndex = 0;
            using (IEnumerator<char> thisEnumerator = GetEnumerator())
            using (IEnumerator<char> otherEnumerator = other.GetEnumerator())
            {
                while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
                {
                    if (thisEnumerator.Current != otherEnumerator.Current)
                    {
                        break;
                    }
                    splitIndex++;
                }
            }

            SplitResult thisSplitted = Split(splitIndex);
            SplitResult otherSplitted = other.Split(splitIndex);

            StringPartition commonHead = thisSplitted.Head;
            StringPartition restThis = thisSplitted.Rest;
            StringPartition restOther = otherSplitted.Rest;
            return new ZipResult(commonHead, restThis, restOther);
        }

        public override string ToString()
        {
            var result = new string(this.ToArray());
            return string.Intern(result);
        }

        public static bool operator ==(StringPartition left, StringPartition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringPartition left, StringPartition right)
        {
            return !(left == right);
        }

        public struct Enumerator : IEnumerator<char>
        {
            private readonly ReadOnlyMemory<char> memory;
            private MemoryHandle handle;
            private unsafe char* cur;
            private unsafe char* end; // inclusive end

            internal Enumerator(ReadOnlyMemory<char> memory)
            {
                this.memory = memory;
                handle = default;
                unsafe
                {
                    cur = end = null;
                }
            }

            /// <inheritdoc />
            public unsafe bool MoveNext()
            {
                if (cur != null)
                {
                    if (cur < end)
                    {
                        cur++;
                        return true;
                    }
                    return false;
                }
                return MoveNextInit();
            }

            private bool MoveNextInit()
            {
                if (memory.Length == 0) return false;
                handle = memory.Pin();
                unsafe
                {
                    cur = (char*)handle.Pointer;
                    end = cur + memory.Length - 1;
                }
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                unsafe
                {
                    cur = end = null;
                }
                handle.Dispose();
                handle = default;
            }

            /// <inheritdoc />
            public unsafe char Current => cur != null ? *cur : '\0';

            /// <inheritdoc />
            unsafe object? IEnumerator.Current => cur != null ? *cur : '\0';

            /// <inheritdoc />
            public void Dispose()
            {
                Reset();
            }
        }
    }
}