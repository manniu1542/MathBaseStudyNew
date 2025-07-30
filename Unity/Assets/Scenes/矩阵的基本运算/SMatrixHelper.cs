using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SMatrix
{
    float[,] matrix;
    public int row;
    public int col;

    public SMatrix(float[,] matrix)
    {
        this.matrix = matrix;
        row = this.matrix.GetLength(0);
        col = this.matrix.GetLength(1);
    }

    public SMatrix(int r, int c)
    {
        row = r;
        col = c;
        this.matrix = new float[r, c];
    }

    public float this[int r, int c]
    {
        get => matrix[r, c];
        set => matrix[r, c] = value;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Matrix [{row}x{col}]:");

        for (int i = 0; i < row; i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < col; j++)
            {
                sb.Append(matrix[i, j].ToString("F2")); // F2表示保留2位小数
                if (j < col - 1) sb.Append(", ");
            }

            sb.AppendLine(" ]");
        }

        return sb.ToString();
    }
}

public class SMatrixHelper
{
    public static SMatrix MatrixAdd(SMatrix matrix1, SMatrix matrix2)
    {
        if (matrix1.row != matrix2.row || matrix1.col != matrix2.col)
        {
            Debug.Log("无法计算两个向量的加法");
            return null;
        }

        SMatrix result = new SMatrix(matrix1.row, matrix1.col);
        for (int i = 0; i < result.row; i++)
        {
            for (int j = 0; j < result.col; j++)
            {
                result[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return result;
    }

    public static SMatrix MatrixMul(SMatrix matrix1, SMatrix matrix2)
    {
        if (matrix1.col != matrix2.row)
        {
            Debug.Log("无法计算两个向量的乘法");
            return null;
        }

        SMatrix result = new SMatrix(matrix1.row, matrix2.col);
        int kCount = matrix1.col;
        for (int i = 0; i < result.row; i++)
        {
            for (int j = 0; j < result.col; j++)
            {
                for (int k = 0; k < kCount; k++)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return result;
    }

    public static SMatrix MatrixMul(SMatrix matrix1, float value)
    {
        SMatrix result = new SMatrix(matrix1.row, matrix1.col);
        for (int i = 0; i < result.row; i++)
        {
            for (int j = 0; j < result.col; j++)
            {
                result[i, j] = matrix1[i, j] * value;
            }
        }

        return result;
    }

    /// <summary>
    /// 向量的线性变换 ， 平移，缩放，旋转，对称，反射
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Vector3 V3LinearTransformation(Vector3 v3, SMatrix matrix)
    {
        SMatrix v3Matrix = new SMatrix(new float[,] { { v3.x, v3.y, v3.z } });
        var result = MatrixMul(v3Matrix, matrix);
        return new Vector3(result[0, 0], result[0, 1], result[0, 2]);
    }

    /// <summary>
    /// 矩阵计算叉积 （右手定则的叉乘）
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Vector3 V3CrossProduct(Vector3 v31, Vector3 v32)
    {
        SMatrix v3Matrix1 = new SMatrix(new float[,] { { v31.x, v31.y, v31.z } });
        //斜对称矩阵
        SMatrix v3Matrix2 = new SMatrix(new float[,]
        {
            { 0, -v32.z, v32.y },
            { v32.z, 0, -v32.x },
            { -v32.y, v32.x, 0 }
        });
        var result = MatrixMul(v3Matrix1, v3Matrix2);
        return new Vector3(result[0, 0], result[0, 1], result[0, 2]);
    }
}