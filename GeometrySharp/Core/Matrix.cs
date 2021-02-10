﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Geometry;

// ToDo this class has to be tested.
// ToDo this class has to be commented on all of the parts.
// ToDo remove code that is not necessary.
namespace GeometrySharp.Core
{
    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<IList<double>>
    {
        private readonly List<IList<double>> matrixData;

        /// <summary>
        /// Initialize an empty matrix.
        /// </summary>
        public Matrix()
        {
            matrixData = new List<IList<double>>();
        }

        /// <summary>
        /// Constructs a matrix by given number of rows and columns.
        /// All the parameters are set to zero.
        /// </summary>
        /// <param name="row">A positive integer, for the number of rows.</param>
        /// <param name="column">A positive integer, for the number of columns.</param>
        public static Matrix Construct(int row, int column)
        {
            var tempMatrix = new Matrix();
            if (row == 0 || column == 0)
                throw new Exception("Matrix must be at least one row or column");
            for (int i = 0; i < column; i++)
            {
                var tempRow = Sets.RepeatData(0.0, row);
                tempMatrix.Add(tempRow);
            }

            return tempMatrix;
        }

        /// <summary>
        /// Creates an identity matrix of a given size.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>Identity matrix of the given size.</returns>
        public static Matrix Identity(int size)
        {
            var m = new Matrix();
            var zeros = Vector3.Zero2d(size, size);
            for (int i = 0; i < size; i++)
            {
                zeros[i][i] = 1.0;
                m.Add(zeros[i]);
            }
            return m;
        }

        /// <summary>
        /// Multiply a `Matrix` by a constant.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Muliplication(double a, Matrix b)
        {
            Matrix r = new Matrix();
            foreach (var l in b)
                r.Add((Vector3)l * a);
            return r;
        }

        /// <summary>
        /// Multiply two matrices assuming they are of compatible dimensions.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>The product matrix of the inputs.</returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            var aRows = a.Count;
            var aCols = a[0].Count;

            var bRows = b.Count;
            var bCols = b[0].Count;

            if(aCols != bRows)
                throw new Exception("Non-conformable matrices.");

            var resultMatrix = new Matrix();

            for (int i = 0; i < aRows; ++i)
            {
                var tempRow = Sets.RepeatData(0.0, bCols);
                for (int j = 0; j < bCols; ++j)
                {
                    var value = 0.0;
                    for (int k = 0; k < aCols; ++k)
                    {
                        value += a[i][k] * b[k][j];
                    }
                    tempRow[j] = value;
                }
                resultMatrix.Add(tempRow);
            }

            return resultMatrix;
        }

        /// <summary>
        /// Add two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Addition(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add((Vector3)a[i]+(Vector3)b[i]);
            return r;
        }

        /// <summary>
        /// Divide each of entry of a Matrix by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Division(Matrix a, double b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add((Vector3)a[i]/ b);
            return r;
        }

        /// <summary>
        /// Subtract two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Subtraction(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add((Vector3)a[i] - (Vector3)b[i]);
            return r;
        }

        /// <summary>
        /// Multiply a `Matrix` by a `Vector3`
        /// </summary>
        /// <param name="a">The transformation matrix.</param>
        /// <param name="b">The vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public static Vector3 Dot(Matrix a, Vector3 b)
        {
            if(b.Count != a[0].Count)
                throw new ArgumentOutOfRangeException(nameof(b), "Vector3 and Matrix must have the same dimension.");
            Vector3 r = new Vector3();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vector3.Dot(new Vector3(a[i]), b));
            return r;
        }

        /// <summary>
        /// Transpose a matrix.
        /// This is like swapping rows with columns.
        /// </summary>
        /// <param name="a">Matrix that has to be transposed.</param>
        /// <returns>The matrix transposed.</returns>
        public Matrix Transpose()
        {
            if (this.Count == 0) return null;
            Matrix transposeMatrix = new Matrix();
            var rows = this.Count;
            var columns = this[0].Count;
            for (var c = 0; c < columns; c++)
            {
                var rr = new List<double>();
                for (var r = 0; r < rows; r++)
                {
                    rr.Add(this[r][c]);
                }
                transposeMatrix.Add(rr);
            }
            return transposeMatrix;
        }

        public Matrix Inverse()
        {
            return new Matrix();
        }

        /// <summary>
        /// Constructs the string representation the matrix.
        /// </summary>
        /// <returns>Text string.</returns>
        public override string ToString()
        {
            return string.Join("\n", this.Select(first => $"({string.Join(",", first)})"));
        }
    }
}
