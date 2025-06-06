﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Runtime.InteropServices;
using System.XUnit;

namespace System.Drawing.Drawing2D.Tests;

public partial class MatrixTests
{
    private static Matrix CreateDisposedMatrix()
    {
        Matrix matrix = new();
        matrix.Dispose();
        return matrix;
    }

    [Fact]
    public void Ctor_Default()
    {
        using Matrix matrix = new();
        Assert.Equal([1, 0, 0, 1, 0, 0], matrix.Elements);
        Assert.True(matrix.IsIdentity);
        Assert.True(matrix.IsInvertible);
        Assert.Equal(0, matrix.OffsetX);
        Assert.Equal(0, matrix.OffsetY);
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(float.PositiveInfinity)]
    public void Ctor_FloatingPointBoundsInElements(float f)
    {
        Ctor_Elements(f, 0, 0, 1, 0, 0, false, false);
        Ctor_Elements(1, f, 0, 1, 0, 0, false, false);
        Ctor_Elements(1, 0, f, 1, 0, 0, false, false);
        Ctor_Elements(1, 0, 0, f, 0, 0, false, false);
        Ctor_Elements(1, 0, 0, 1, f, 0, false, false);
        Ctor_Elements(1, 0, 0, 1, 0, f, false, false);
    }

    [Theory]
    [MemberData(nameof(MatrixElements_TestData))]
    public void Ctor_Elements(float m11, float m12, float m21, float m22, float dx, float dy, bool isIdentity, bool isInvertible)
    {
        using Matrix matrix = new(m11, m12, m21, m22, dx, dy);
        Assert.Equal([m11, m12, m21, m22, dx, dy], matrix.Elements);
        Assert.Equal(isIdentity, matrix.IsIdentity);
        Assert.Equal(isInvertible, matrix.IsInvertible);
        Assert.Equal(dx, matrix.OffsetX);
        Assert.Equal(dy, matrix.OffsetY);
    }

    public static TheoryData<float, float, float, float, float, float, bool, bool> MatrixElements_TestData
        => new()
        {
            { 1, 0, 0, 1, 0, 0, true, true },
            { 0, 1, 2, 1, 3, 4, false, true },
            { 0, 0, 0, 0, 0, 0, false, false },
            { 1, 2, 3, 4, 5, 6, false, true },
            { -1, -2, -3, -4, -5, -6, false, true },
            { 123, 24, 82, 16, 47, 30, false, false },
            { 156, 46, 0, 0, 106, 19, false, false },
            { 146, 66, 158, 104, 42, 150, false, true },
            { 119, 140, 145, 74, 102, 58, false, true },
            { 1.1f, 0.1f, -0.1f, 0.9f, 0, 0, false, true },
            { 1.01f, 0.01f, -0.01f, 0.99f, 0, 0, false, true },
            { 1.001f, 0.001f, -0.001f, 0.999f, 0, 0, false, true },
            { 1.0001f, 0.0001f, -0.0001f, 0.9999f, 0, 0, true, true },
            { 1.0009f, 0.0009f, -0.0009f, 0.99995f, 0, 0, false, true }
        };

    public static IEnumerable<object[]> Ctor_Rectangle_Points_TestData()
    {
        yield return new object[] { new Rectangle(1, 4, 8, 16), new Point[] { new(32, 64), new(128, 256), new(512, 1024) }, new float[] { 12, 24, 30, 60, -100, -200 }, false, false };
        yield return new object[] { new Rectangle(0, 0, 2, 4), new Point[] { new(8, 16), new(32, 64), new(128, 256) }, new float[] { 12, 24, 30, 60, 8, 16 }, false, false };
        yield return new object[] { new Rectangle(0, 0, 1, 1), new Point[] { new(0, 0), new(0, 0), new(0, 0) }, new float[] { 0, 0, 0, 0, 0, 0 }, false, false };
        yield return new object[] { new Rectangle(0, 0, 1, 1), new Point[] { new(0, 0), new(1, 0), new(0, 1) }, new float[] { 1, 0, 0, 1, 0, 0 }, true, true };
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Points_TestData))]
    public void Ctor_Rectangle_Points(Rectangle rect, Point[] plgpnts, float[] expectedElements, bool isIdentity, bool isInvertible)
    {
        using Matrix matrix = new(rect, plgpnts);
        Assert.Equal(expectedElements, matrix.Elements);
        Assert.Equal(isIdentity, matrix.IsIdentity);
        Assert.Equal(isInvertible, matrix.IsInvertible);
        Assert.Equal(expectedElements[4], matrix.OffsetX);
        Assert.Equal(expectedElements[5], matrix.OffsetY);
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Points_TestData))]
    public void Ctor_RectangleF_Points(Rectangle rect, Point[] plgpnts, float[] expectedElements, bool isIdentity, bool isInvertible)
    {
        using Matrix matrix = new(rect, plgpnts.Select(p => (PointF)p).ToArray());
        Assert.Equal(expectedElements, matrix.Elements);
        Assert.Equal(isIdentity, matrix.IsIdentity);
        Assert.Equal(isInvertible, matrix.IsInvertible);
        Assert.Equal(expectedElements[4], matrix.OffsetX);
        Assert.Equal(expectedElements[5], matrix.OffsetY);
    }

    [Fact]
    public void Ctor_NullPoints_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("plgpts", () => new Matrix(default(RectangleF), null));
        AssertExtensions.Throws<ArgumentNullException>("plgpts", () => new Matrix(default, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(4)]
    public void Ctor_PointsLengthNotThree_ThrowsArgumentNullException(int length)
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Matrix(default, new PointF[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => new Matrix(default, new Point[length]));
    }

    [Fact]
    public void Ctor_WidthZero_ThrowsExternalException()
    {
        Assert.Throws<ExternalException>(() => new Matrix(new Rectangle(1, 1, 0, 1), new Point[3]));
        Assert.Throws<ExternalException>(() => new Matrix(new RectangleF(1, 1, 0, 1), new PointF[3]));
    }

    [Fact]
    public void Ctor_HeightZero_ThrowsExternalException()
    {
        Assert.Throws<ExternalException>(() => new Matrix(new Rectangle(1, 1, 1, 0), new Point[3]));
        Assert.Throws<ExternalException>(() => new Matrix(new RectangleF(1, 1, 1, 0), new PointF[3]));
    }

    [Fact]
    public void Clone_Matrix_ReturnsExpected()
    {
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        using Matrix clone = Assert.IsType<Matrix>(matrix.Clone());
        Assert.NotSame(matrix, clone);
        Assert.Equal([1, 2, 3, 4, 5, 6], clone.Elements);
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Clone());
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        yield return new object[] { new Matrix(), new Matrix(1, 0, 0, 1, 0, 0), true };
        yield return new object[] { new Matrix(), new Matrix(123, 24, 82, 16, 47, 30), false };
        yield return new object[] { new Matrix(), new Matrix(1.1f, 0.1f, -0.1f, 0.9f, 0, 0), false };
        yield return new object[] { new Matrix(), new Matrix(1.01f, 0.01f, -0.01f, 0.99f, 0, 0), false };
        yield return new object[] { new Matrix(), new Matrix(1.001f, 0.001f, -0.001f, 0.999f, 0, 0), false };
        yield return new object[] { new Matrix(), new Matrix(1.0001f, 0.0001f, -0.0001f, 0.9999f, 0, 0), false };
        yield return new object[] { new Matrix(), new Matrix(1.0009f, 0.0009f, -0.0009f, 0.99995f, 0, 0), false };

        Matrix matrix = new(1, 2, 3, 4, 5, 6);
        yield return new object[] { matrix, matrix, true };
        yield return new object[] { matrix.Clone(), matrix.Clone(), true };
        yield return new object[] { matrix.Clone(), new Matrix(1, 2, 3, 4, 5, 6), true };
        yield return new object[] { matrix.Clone(), new Matrix(2, 2, 3, 4, 5, 6), false };
        yield return new object[] { matrix.Clone(), new Matrix(1, 3, 3, 4, 5, 6), false };
        yield return new object[] { matrix.Clone(), new Matrix(1, 2, 4, 4, 5, 6), false };
        yield return new object[] { matrix.Clone(), new Matrix(1, 2, 3, 5, 5, 6), false };
        yield return new object[] { matrix.Clone(), new Matrix(1, 2, 3, 4, 6, 6), false };
        yield return new object[] { matrix.Clone(), new Matrix(1, 2, 3, 4, 5, 7), false };

        yield return new object[] { new Matrix(), null, false };
        yield return new object[] { new Matrix(), new(), false };
    }

    [Theory]
    [MemberData(nameof(Equals_TestData))]
    public void Equals_Other_ReturnsExpected(Matrix matrix, object other, bool expected)
    {
        using (matrix)
        using (other as IDisposable)
        {
            Assert.Equal(expected, matrix.Equals(other));
            if (other is Matrix otherMatrix)
            {
                Assert.Equal(ReferenceEquals(matrix, other), matrix.GetHashCode().Equals(other.GetHashCode()));
            }
        }
    }

    [Fact]
    public void Equals_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Equals(new Matrix()));
    }

    [Fact]
    public void Equals_DisposedOther_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Matrix().Equals(CreateDisposedMatrix()));
    }

    [Fact]
    public void Elements_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Elements);
    }

    public static IEnumerable<object[]> Invert_TestData()
    {
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), new float[] { -2, 1, 1.5f, -0.5f, 1, -2 } };
        yield return new object[] { new Matrix(1, 0, 0, 1, 8, 8), new float[] { 1, 0, 0, 1, -8, -8 } };
        yield return new object[] { new Matrix(), new float[] { 1, 0, 0, 1, 0, 0 } };
    }

    [Theory]
    [MemberData(nameof(Invert_TestData))]
    public void Invert_Matrix_Success(Matrix matrix, float[] expectedElements)
    {
        using (matrix)
        {
            matrix.Invert();
            Assert.Equal(expectedElements, matrix.Elements);
        }
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Invert_FloatBounds_ThrowsArgumentException(float f)
    {
        using Matrix matrix1 = new(f, 2, 3, 4, 5, 6);
        using Matrix matrix2 = new(1, f, 3, 4, 5, 6);
        using Matrix matrix3 = new(1, 2, f, 4, 5, 6);
        using Matrix matrix4 = new(1, 2, 3, f, 5, 6);
        using Matrix matrix5 = new(1, 2, 3, 4, f, 6);
        using Matrix matrix6 = new(1, 2, 3, 4, 5, f);
        AssertExtensions.Throws<ArgumentException>(null, matrix1.Invert);
        AssertExtensions.Throws<ArgumentException>(null, matrix2.Invert);
        AssertExtensions.Throws<ArgumentException>(null, matrix3.Invert);
        AssertExtensions.Throws<ArgumentException>(null, matrix4.Invert);
        AssertExtensions.Throws<ArgumentException>(null, matrix5.Invert);
        AssertExtensions.Throws<ArgumentException>(null, matrix6.Invert);
    }

    [Fact]
    public void Invert_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Invert());
    }

    [Fact]
    public void IsIdentity_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().IsIdentity);
    }

    [Fact]
    public void IsInvertible_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().IsInvertible);
    }

    public static IEnumerable<object[]> Multiply_TestData()
    {
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(10, 20, 30, 40, 50, 60), MatrixOrder.Prepend, new float[] { 700, 1000, 1500, 2200, 2350, 3460 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(10, 20, 30, 40, 50, 60), MatrixOrder.Append, new float[] { 700, 1000, 1500, 2200, 2350, 3460 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(), MatrixOrder.Prepend, new float[] { 10, 20, 30, 40, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(), MatrixOrder.Append, new float[] { 10, 20, 30, 40, 50, 60 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(0, 0, 0, 0, 0, 0), MatrixOrder.Prepend, new float[] { 0, 0, 0, 0, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(0, 0, 0, 0, 0, 0), MatrixOrder.Append, new float[] { 0, 0, 0, 0, 0, 0 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(1, 1, 1, 1, 1, 1), MatrixOrder.Prepend, new float[] { 40, 60, 40, 60, 90, 120 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(1, 1, 1, 1, 1, 1), MatrixOrder.Append, new float[] { 30, 30, 70, 70, 111, 111 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN), MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN), MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity), MatrixOrder.Prepend, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity), MatrixOrder.Append, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity), MatrixOrder.Prepend, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity), MatrixOrder.Append, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue), MatrixOrder.Prepend, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), new Matrix(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue), MatrixOrder.Append, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue } };
    }

    [Theory]
    [MemberData(nameof(Multiply_TestData))]
    public void Multiply_Matrix_Success(Matrix matrix, Matrix multiple, MatrixOrder order, float[] expected)
    {
        using (matrix)
        using (multiple)
        {
            if (order == MatrixOrder.Prepend)
            {
                using Matrix clone1 = matrix.Clone();
                clone1.Multiply(multiple);
                Assert.Equal(expected, clone1.Elements, new FloatingPointToleranceComparerer<float>(0.00001f));
            }

            matrix.Multiply(multiple, order);
            Assert.Equal(expected, matrix.Elements);
        }
    }

    [Fact]
    public void Multiply_NullMatrix_ThrowsArgumentNullException()
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => matrix.Multiply(null));
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => matrix.Multiply(null, MatrixOrder.Prepend));
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void Multiply_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        using Matrix other = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Multiply(other, order));
    }

    [Fact]
    public void Multiply_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        using Matrix other = new();
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Multiply(other));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Multiply(other, MatrixOrder.Prepend));
    }

    [Fact]
    public void Multiply_DisposedMatrix_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Multiply(disposedMatrix));
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Multiply(disposedMatrix, MatrixOrder.Prepend));
    }

    [Fact]
    public void Multiply_SameMatrix_ThrowsInvalidOperationException()
    {
        using Matrix matrix = new();
        Assert.Throws<InvalidOperationException>(() => matrix.Multiply(matrix));
        Assert.Throws<InvalidOperationException>(() => matrix.Multiply(matrix, MatrixOrder.Prepend));
    }

    [Fact]
    public void Reset_Matrix_ReturnsExpected()
    {
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        matrix.Reset();
        Assert.Equal([1, 0, 0, 1, 0, 0], matrix.Elements);

        matrix.Reset();
        Assert.Equal([1, 0, 0, 1, 0, 0], matrix.Elements);
    }

    [Fact]
    public void Reset_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Reset());
    }

    public static IEnumerable<object[]> Rotate_TestData()
    {
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 180, PointF.Empty, MatrixOrder.Prepend, new float[] { -9.999996f, -19.9999943f, -30.0000019f, -40.0000038f, 50, 60 }, null, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 180, PointF.Empty, MatrixOrder.Append, new float[] { -9.999996f, -20, -30f, -40f, -50, -60 }, null, false };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 540, PointF.Empty, MatrixOrder.Prepend, new float[] { -9.999996f, -19.9999943f, -30.0000019f, -40.0000038f, 50, 60 }, null, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 540, PointF.Empty, MatrixOrder.Append, new float[] { -9.999996f, -20, -30f, -40f, -50, -60 }, null, false };

        yield return new object[] { new Matrix(), 45, PointF.Empty, MatrixOrder.Prepend, new float[] { 0.707106769f, 0.707106769f, -0.707106829f, 0.707106769f, 0, 0 }, null, false };
        yield return new object[] { new Matrix(), 45, PointF.Empty, MatrixOrder.Append, new float[] { 0.707106769f, 0.707106769f, -0.707106829f, 0.707106769f, 0, 0 }, null, false };

        using (Matrix rotated45 = new())
        {
            rotated45.Rotate(45);
            yield return new object[] { rotated45.Clone(), 135, PointF.Empty, MatrixOrder.Prepend, new float[] { -1, 0, 0, -1, 0, 0 }, null, false };
            yield return new object[] { rotated45.Clone(), 135, PointF.Empty, MatrixOrder.Append, new float[] { -1, 0, 0, -1, 0, 0 }, null, false };
        }

        yield return new object[] { new Matrix(), 90, PointF.Empty, MatrixOrder.Prepend, new float[] { 0, 1, -1, 0, 0, 0 }, null, false };
        yield return new object[] { new Matrix(), 90, PointF.Empty, MatrixOrder.Append, new float[] { 0, 1, -1, 0, 0, 0 }, null, false };

        using (Matrix rotated90 = new())
        {
            rotated90.Rotate(90);
            yield return new object[] { rotated90.Clone(), 270, PointF.Empty, MatrixOrder.Prepend, new float[] { 1, 0, 0, 1, 0, 0 }, null, true };
            yield return new object[] { rotated90.Clone(), 270, PointF.Empty, MatrixOrder.Append, new float[] { 1, 0, 0, 1, 0, 0 }, null, true };
        }

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 180, new PointF(10, 10), MatrixOrder.Prepend, new float[] { -10, -20, -30, -40, 850, 1260 }, null, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 180, new PointF(10, 10), MatrixOrder.Append, new float[] { -10, -20, -30, -40, -30, -40 }, null, false };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, PointF.Empty, MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, 50, 60 }, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, PointF.Empty, MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, null, false };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, PointF.Empty, MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, 50, 60 }, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, PointF.Empty, MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, null, false };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, PointF.Empty, MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, 50, 60 }, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, false };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, PointF.Empty, MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN }, null, false };
    }

    [Theory]
    [MemberData(nameof(Rotate_TestData))]
    public void Rotate_Matrix_Success(Matrix matrix, float angle, PointF point, MatrixOrder order, float[] expectedElements, float[] expectedElementsRotateAt, bool isIdentity)
    {
        using (matrix)
        {
            if (order == MatrixOrder.Prepend)
            {
                if (point == Point.Empty)
                {
                    using Matrix clone1 = matrix.Clone();
                    clone1.Rotate(angle);
                    AssertEqualFloatArray(expectedElements, clone1.Elements);
                    Assert.Equal(isIdentity, clone1.IsIdentity);
                }

                using Matrix clone2 = matrix.Clone();
                clone2.RotateAt(angle, point);
                AssertEqualFloatArray(expectedElementsRotateAt ?? expectedElements, clone2.Elements);
                Assert.False(clone2.IsIdentity);
            }

            if (point == Point.Empty)
            {
                using Matrix clone3 = matrix.Clone();
                clone3.Rotate(angle, order);
                AssertEqualFloatArray(expectedElements, clone3.Elements);
                Assert.Equal(isIdentity, clone3.IsIdentity);
            }

            using Matrix clone4 = matrix.Clone();
            clone4.RotateAt(angle, point, order);
            AssertEqualFloatArray(expectedElementsRotateAt ?? expectedElements, clone4.Elements);
            Assert.False(clone4.IsIdentity);
        }
    }

    [Fact]
    public void Rotate_Disposed_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => CreateDisposedMatrix().Rotate(1, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void Rotate_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Rotate(1, order));
    }

    [Fact]
    public void RotateAt_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.RotateAt(1, PointF.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.RotateAt(1, PointF.Empty, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void RotateAt_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.RotateAt(1, PointF.Empty, order));
    }

    public static IEnumerable<object[]> Scale_TestData()
    {
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 2, 4, MatrixOrder.Prepend, new float[] { 20, 40, 120, 160, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 2, 4, MatrixOrder.Append, new float[] { 20, 80, 60, 160, 100, 240 } };

        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0.5, 0.25, MatrixOrder.Prepend, new float[] { 10, 20, 30, 40, 50, 60 } };
        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0.5, 0.25, MatrixOrder.Append, new float[] { 10, 10, 60, 40, 25, 15 } };

        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0, 0, MatrixOrder.Prepend, new float[] { 0, 0, 0, 0, 50, 60 } };
        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0, 0, MatrixOrder.Append, new float[] { 0, 0, 0, 0, 0, 0 } };

        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 1, 1, MatrixOrder.Prepend, new float[] { 20, 40, 120, 160, 50, 60 } };
        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 1, 1, MatrixOrder.Append, new float[] { 20, 40, 120, 160, 50, 60 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), -2, -4, MatrixOrder.Prepend, new float[] { -20, -40, -120, -160, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), -2, -4, MatrixOrder.Append, new float[] { -20, -80, -60, -160, -100, -240 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, float.NaN, MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, float.NaN, MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Prepend, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Append, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Prepend, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Append, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.MaxValue, float.MaxValue, MatrixOrder.Prepend, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.MaxValue, float.MaxValue, MatrixOrder.Append, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue } };
    }

    [Theory]
    [MemberData(nameof(Scale_TestData))]
    public void Scale_Matrix_Succss(Matrix matrix, float scaleX, float scaleY, MatrixOrder order, float[] expectedElements)
    {
        using (matrix)
        {
            if (order == MatrixOrder.Prepend)
            {
                using Matrix clone = matrix.Clone();
                clone.Scale(scaleX, scaleY);
                Assert.Equal(expectedElements, clone.Elements, new FloatingPointToleranceComparerer<float>(0.00001f));
            }

            matrix.Scale(scaleX, scaleY, order);
            Assert.Equal(expectedElements, matrix.Elements);
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void Scale_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Shear(1, 2, order));
    }

    [Fact]
    public void Scale_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Scale(1, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Scale(1, 2, MatrixOrder.Append));
    }

    public static IEnumerable<object[]> Shear_TestData()
    {
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 2, 4, MatrixOrder.Prepend, new float[] { 130, 180, 50, 80, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), 2, 4, MatrixOrder.Append, new float[] { 50, 60, 110, 160, 170, 260 } };

        yield return new object[] { new Matrix(5, 3, 9, 2, 2, 1), 10, 20, MatrixOrder.Prepend, new float[] { 185, 43, 59, 32, 2, 1 } };
        yield return new object[] { new Matrix(5, 3, 9, 2, 2, 1), 10, 20, MatrixOrder.Append, new float[] { 35, 103, 29, 182, 12, 41 } };

        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0, 0, MatrixOrder.Prepend, new float[] { 20, 40, 120, 160, 50, 60 } };
        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 0, 0, MatrixOrder.Append, new float[] { 20, 40, 120, 160, 50, 60 } };

        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 1, 1, MatrixOrder.Prepend, new float[] { 140, 200, 140, 200, 50, 60 } };
        yield return new object[] { new Matrix(20, 40, 120, 160, 50, 60), 1, 1, MatrixOrder.Append, new float[] { 60, 60, 280, 280, 110, 110 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), -2, -4, MatrixOrder.Prepend, new float[] { -110, -140, 10, 0, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), -2, -4, MatrixOrder.Append, new float[] { -30, -20, -50, -80, -70, -140 } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, float.NaN, MatrixOrder.Prepend, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NaN, float.NaN, MatrixOrder.Append, new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Prepend, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Append, new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Prepend, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Append, new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity } };

        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.MaxValue, float.MaxValue, MatrixOrder.Prepend, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, 50, 60 } };
        yield return new object[] { new Matrix(10, 20, 30, 40, 50, 60), float.MaxValue, float.MaxValue, MatrixOrder.Append, new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue } };
    }

    [Theory]
    [MemberData(nameof(Shear_TestData))]
    public void Shear_Matrix_Succss(Matrix matrix, float shearX, float shearY, MatrixOrder order, float[] expectedElements)
    {
        using (matrix)
        {
            if (order == MatrixOrder.Prepend)
            {
                using Matrix clone = matrix.Clone();
                clone.Shear(shearX, shearY);
                Assert.Equal(expectedElements, clone.Elements, new FloatingPointToleranceComparerer<float>(0.00001f));
            }

            matrix.Shear(shearX, shearY, order);
            Assert.Equal(expectedElements, matrix.Elements);
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void Shear_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Shear(1, 2, order));
    }

    [Fact]
    public void Shear_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Shear(1, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Shear(1, 2, MatrixOrder.Append));
    }

    public static IEnumerable<object[]> Translate_TestData()
    {
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), 5, 10, MatrixOrder.Prepend, new float[] { 2, 4, 6, 8, 80, 112 } };
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), 5, 10, MatrixOrder.Append, new float[] { 2, 4, 6, 8, 15, 22 } };

        yield return new object[] { new Matrix(), 5, 10, MatrixOrder.Prepend, new float[] { 1, 0, 0, 1, 5, 10 } };
        yield return new object[] { new Matrix(), 5, 10, MatrixOrder.Append, new float[] { 1, 0, 0, 1, 5, 10 } };

        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.NaN, float.NaN, MatrixOrder.Prepend, new float[] { 1, 2, 3, 4, float.NaN, float.NaN } };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.NaN, float.NaN, MatrixOrder.Append, new float[] { 1, 2, 3, 4, float.NaN, float.NaN } };

        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Prepend, new float[] { 1, 2, 3, 4, float.PositiveInfinity, float.PositiveInfinity } };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.PositiveInfinity, float.PositiveInfinity, MatrixOrder.Append, new float[] { 1, 2, 3, 4, float.PositiveInfinity, float.PositiveInfinity } };

        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Prepend, new float[] { 1, 2, 3, 4, float.NegativeInfinity, float.NegativeInfinity } };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.NegativeInfinity, float.NegativeInfinity, MatrixOrder.Append, new float[] { 1, 2, 3, 4, float.NegativeInfinity, float.NegativeInfinity } };

        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.MaxValue, float.MaxValue, MatrixOrder.Prepend, new float[] { 1, 2, 3, 4, float.MaxValue, float.MaxValue } };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), float.MaxValue, float.MaxValue, MatrixOrder.Append, new float[] { 1, 2, 3, 4, float.MaxValue, float.MaxValue } };
    }

    [Theory]
    [MemberData(nameof(Translate_TestData))]
    public void Translate_Matrix_Success(Matrix matrix, float offsetX, float offsetY, MatrixOrder order, float[] expectedElements)
    {
        using (matrix)
        {
            if (order == MatrixOrder.Prepend)
            {
                using Matrix clone = matrix.Clone();
                clone.Translate(offsetX, offsetY);
                Assert.Equal(expectedElements, clone.Elements, new FloatingPointToleranceComparerer<float>(0.00001f));
            }

            matrix.Translate(offsetX, offsetY, order);
            Assert.Equal(expectedElements, matrix.Elements, new FloatingPointToleranceComparerer<float>(0.00001f));
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void Translate_InvalidMatrixOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.Translate(1, 2, order));
    }

    [Fact]
    public void Translate_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Translate(1, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.Translate(1, 2, MatrixOrder.Append));
    }

    public static IEnumerable<object[]> TransformPoints_TestData()
    {
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), new Point[] { new(2, 4), new(4, 8) }, new Point[] { new(38, 52), new(66, 92) } };
        yield return new object[] { new Matrix(), new Point[] { new(2, 4), new(4, 8) }, new Point[] { new(2, 4), new(4, 8) } };
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), new Point[1], new Point[] { new(10, 12) } };
    }

    [Theory]
    [MemberData(nameof(TransformPoints_TestData))]
    public void TransformPoints_Point_Success(Matrix matrix, Point[] points, Point[] expectedPoints)
    {
        using (matrix)
        {
            matrix.TransformPoints(points);
            Assert.Equal(expectedPoints, points);
        }
    }

    [Theory]
    [MemberData(nameof(TransformPoints_TestData))]
    public void TransformPoints_PointF_Success(Matrix matrix, Point[] points, Point[] expectedPoints)
    {
        using (matrix)
        {
            PointF[] pointFs = points.Select(p => (PointF)p).ToArray();
            matrix.TransformPoints(pointFs);
            Assert.Equal(expectedPoints.Select(p => (PointF)p), pointFs);
        }
    }

    [Fact]
    public void TransformPoints_NullPoints_ThrowsArgumentNullException()
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentNullException>("pts", () => matrix.TransformPoints((Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("pts", () => matrix.TransformPoints((PointF[])null));
    }

    [Fact]
    public void TransformPoints_EmptyPoints_ThrowsArgumentException()
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.TransformPoints(Array.Empty<Point>()));
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.TransformPoints(Array.Empty<PointF>()));
    }

    [Fact]
    public void TransformPoints_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.TransformPoints(new Point[1]));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.TransformPoints(new PointF[1]));
    }

    public static IEnumerable<object[]> TransformVectors_TestData()
    {
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), new Point[] { new(2, 4), new(4, 8) }, new Point[] { new(28, 40), new(56, 80) } };
        yield return new object[] { new Matrix(), new Point[] { new(2, 4), new(4, 8) }, new Point[] { new(2, 4), new(4, 8) } };
        yield return new object[] { new Matrix(2, 4, 6, 8, 10, 12), new Point[1], new Point[1] };
    }

    [Theory]
    [MemberData(nameof(TransformVectors_TestData))]
    public void TransformVectors_Point_Success(Matrix matrix, Point[] points, Point[] expectedPoints)
    {
        using (matrix)
        {
            matrix.TransformVectors(points);
            Assert.Equal(expectedPoints, points);
        }
    }

    [Theory]
    [MemberData(nameof(TransformVectors_TestData))]
    public void TransformVectors_PointF_Success(Matrix matrix, Point[] points, Point[] expectedPoints)
    {
        using (matrix)
        {
            PointF[] pointFs = points.Select(p => (PointF)p).ToArray();
            matrix.TransformVectors(pointFs);
            Assert.Equal(expectedPoints.Select(p => (PointF)p), pointFs);
        }
    }

    [Theory]
    [MemberData(nameof(TransformVectors_TestData))]
    public void VectorTransformPoints_Points_Success(Matrix matrix, Point[] points, Point[] expectedPoints)
    {
        using (matrix)
        {
            matrix.VectorTransformPoints(points);
            Assert.Equal(expectedPoints, points);
        }
    }

    [Fact]
    public void TransformVectors_NullPoints_ThrowsArgumentNullException()
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentNullException>("pts", () => matrix.VectorTransformPoints(null));
        AssertExtensions.Throws<ArgumentNullException>("pts", () => matrix.TransformVectors((Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("pts", () => matrix.TransformVectors((PointF[])null));
    }

    [Fact]
    public void TransformVectors_EmptyPoints_ThrowsArgumentException()
    {
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.VectorTransformPoints([]));
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.TransformVectors(Array.Empty<Point>()));
        AssertExtensions.Throws<ArgumentException>(null, () => matrix.TransformVectors(Array.Empty<PointF>()));
    }

    [Fact]
    public void TransformVectors_Disposed_ThrowsArgumentException()
    {
        Matrix disposedMatrix = CreateDisposedMatrix();

        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.VectorTransformPoints(new Point[1]));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.TransformPoints(new Point[1]));
        AssertExtensions.Throws<ArgumentException>(null, () => disposedMatrix.TransformVectors(new PointF[1]));
    }

    private static void AssertEqualFloatArray(float[] expected, float[] actual)
    {
        Assert.Equal(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            try
            {
                Assert.Equal(expected[i], actual[i], precision: 3);
            }
            catch
            {
                Console.WriteLine(i);
                Console.WriteLine($"Expected: {string.Join(", ", expected)}");
                Console.WriteLine($"Actual: {string.Join(", ", actual)}");
                throw;
            }
        }
    }
}
