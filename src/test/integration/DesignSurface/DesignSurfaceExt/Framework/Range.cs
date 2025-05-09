﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_0 || NETFRAMEWORK
using System.Numerics.Hashing;
#endif

namespace System;

public readonly struct Range : IEquatable<Range>
{
    /// <summary>Represent the inclusive start index of the Range.</summary>
    public Index Start { get; }

    /// <summary>Represent the exclusive end index of the Range.</summary>
    public Index End { get; }

    /// <summary>Construct a Range object using the start and end indexes.</summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="end">Represent the exclusive end index of the range.</param>
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Indicates whether the current Range object is equal to another object of the same type.</summary>
    /// <param name="value">An object to compare with this object</param>
    #pragma warning disable CA1725 // Parameter names should match base declaration
    public override bool Equals([NotNullWhen(true)] object? value) =>
        value is Range r &&
        r.Start.Equals(Start) &&
        r.End.Equals(End);
    #pragma warning restore CA1725

    /// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode()
    {
#if (!NETSTANDARD2_0 && !NETFRAMEWORK)
        return HashCode.Combine(Start.GetHashCode(), End.GetHashCode());
#else
        return HashHelpers.Combine(Start.GetHashCode(), End.GetHashCode());
#endif
    }

    /// <summary>Converts the value of the current Range object to its equivalent string representation.</summary>
    public override string ToString()
    {
#if (!NETSTANDARD2_0 && !NETFRAMEWORK)
        Span<char> span = stackalloc char[2 + (2 * 11)]; // 2 for "..", then for each index 1 for '^' and 10 for longest possible uint
        int pos = 0;

        if (Start.IsFromEnd)
        {
            span[0] = '^';
            pos = 1;
        }

        bool formatted = ((uint)Start.Value).TryFormat(span[pos..], out int charsWritten);
        Debug.Assert(formatted);
        pos += charsWritten;

        span[pos++] = '.';
        span[pos++] = '.';

        if (End.IsFromEnd)
        {
            span[pos++] = '^';
        }

        formatted = ((uint)End.Value).TryFormat(span[pos..], out charsWritten);
        Debug.Assert(formatted);
        pos += charsWritten;

        return new string(span[..pos]);
#else
        return Start.ToString() + ".." + End.ToString();
#endif
    }

    /// <summary>Create a Range object starting from start index to the end of the collection.</summary>
    public static Range StartAt(Index start) => new(start, Index.End);

    /// <summary>Create a Range object starting from first element in the collection to the end Index.</summary>
    public static Range EndAt(Index end) => new(Index.Start, end);

    /// <summary>Create a Range object starting from first element to the end.</summary>
    public static Range All => new(Index.Start, Index.End);

    /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
    /// <param name="length">The length of the collection that the range will be used with. length has to be a positive value.</param>
    /// <remarks>
    ///  <para>
    ///   For performance reason, we don't validate the input length parameter against negative values.
    ///   It is expected Range will be used with collections which always have non negative length/count.
    ///   We validate the range is inside the length scope though.
    ///  </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start = Start.GetOffset(length);
        int end = End.GetOffset(length);

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            ThrowArgumentOutOfRangeException();
        }

        return (start, end - start);
    }

    private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException("length");
}
