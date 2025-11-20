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

public static class SMatrixHelper
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

    public static SMatrix MatrixMul(this SMatrix matrix1, float value)
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
        SMatrix v3Matrix2 = v32.ToSkewSymmetric();
        var result = MatrixMul(v3Matrix1, v3Matrix2);
        return new Vector3(result[0, 0], result[0, 1], result[0, 2]);
    }

    /// <summary>
    /// 向量转为斜对称矩阵为了叉乘做准备 （右手定则的叉乘）
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static SMatrix ToSkewSymmetric(this Vector3 v3)
    {
        return new SMatrix(new float[,]
        {
            { 0, -v3.z, v3.y },
            { v3.z, 0, -v3.x },
            { -v3.y, v3.x, 0 }
        });
    }

    /// <summary>
    /// 获取变换的 相对父级的 本地坐标系 构成的矩阵 (包含，平移。旋转，缩放，信息)
    /// </summary>
    public static SMatrix GetLocalRelativelyMatrix(this Transform tf)
    {
        // *** 1. 使用局部属性 ***
        Vector3 localScale = tf.localScale; // 局部缩放
        Vector3 localPosition = tf.localPosition; // 局部平移

        // *** 2. 获取局部方向向量 ***
        // 假设 Transform.localRotation 可以用来计算局部方向
        Quaternion localRot = tf.localRotation;

        // 标准基向量
        Vector3 localRight = localRot * Vector3.right; // 局部X轴
        Vector3 localUp = localRot * Vector3.up; // 局部Y轴
        Vector3 localForward = localRot * Vector3.forward; // 局部Z轴

        // 3. 构造局部变换矩阵 (行向量约定：平移在最后一行)
        // 每一行是 [r*sx, u*sy, f*sz] 在父级坐标系中的分量。
        // 因为 localRight/Up/Forward 已经是相对于父级定向的，我们直接使用它们的分量。
        return new SMatrix(new float[,]
        {
            // R * S 部分 (缩放后的局部基轴)
            { localRight.x * localScale.x, localRight.y * localScale.x, localRight.z * localScale.x, 0 },
            { localUp.x * localScale.y, localUp.y * localScale.y, localUp.z * localScale.y, 0 },
            { localForward.x * localScale.z, localForward.y * localScale.z, localForward.z * localScale.z, 0 },

            // T 部分 (局部平移)
            { localPosition.x, localPosition.y, localPosition.z, 1 }
        });
    }

    /// <summary>
    /// 本地坐标转换到世界坐标，TODO:TRS的逆。不知道为什么 算出的本地转世界的构建的矩阵不对
    /// </summary>
    /// <param name="wp"></param>
    /// <param name="localTF"></param>
    /// <returns></returns>
    public static Vector3 LocalPointToWorldPoint(Vector3 lp, Transform localTF)
    {
        if (localTF == null)
            return lp;
        List<SMatrix> list = new List<SMatrix>();
        list.Add(localTF.GetLocalRelativelyMatrix());

        var parent = localTF.parent;
        while (parent != null)
        {
            list.Add(parent.GetLocalRelativelyMatrix());
            parent = parent.parent;
        }

        SMatrix v = new SMatrix(new float[,]
        {
            { lp.x, lp.y, lp.z, 1 }
        });
        SMatrix m = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            m = MatrixMul(m, list[i]);
        }

        v = MatrixMul(v, m);
        return new Vector3(v[0, 0], v[0, 1], v[0, 2]);
    }

    /// <summary>
    /// 获取本地坐标系 构成的矩阵 (包含，平移。旋转，缩放，信息)
    /// </summary>
    /// <summary>
    /// 获取将世界坐标直接转换为本地坐标的单个矩阵 M_WorldToLocal = M_World^(-1)
    /// </summary>
    private static SMatrix GetWorldToLocalMatrix(this Transform tf)
    {
        // S^-1 部分 (缩放的逆)
        Vector3 worldLossyScale = tf.lossyScale;
        float sxInv = 1.0f / worldLossyScale.x;
        float syInv = 1.0f / worldLossyScale.y;
        float szInv = 1.0f / worldLossyScale.z;
        SMatrix M_S_inv = new SMatrix(new float[,]
        {
            { sxInv, 0, 0, 0 },
            { 0, syInv, 0, 0 },
            { 0, 0, szInv, 0 },
            { 0, 0, 0, 1 }
        });
        //TODO： 正交矩阵（旋转矩阵）他的逆矩阵==他的转置
        // 解决方法：使用 tf.rotation 来获取纯净的旋转轴
        Quaternion worldRotation = tf.rotation;
        Vector3 right = worldRotation * Vector3.right;
        Vector3 up = worldRotation * Vector3.up;
        Vector3 forward = worldRotation * Vector3.forward;
        
        SMatrix M_R_inv = new SMatrix(new float[,]
        {
            // R * S^-1 (转置的轴向量)
            { right.x, up.x, forward.x, 0 },
            { right.y, up.y, forward.y, 0 },
            { right.z, up.z, forward.z, 0 },
            { 0, 0, 0, 1 }
        });
      
        // 1. 获取tf的世界坐标和世界旋转 (Unity Transform自带的属性)
        Vector3 worldPosition = tf.position; // 世界平移 T
        SMatrix M_T_inv = new SMatrix(new float[,]
        {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, 0 },
            { -worldPosition.x, -worldPosition.y, -worldPosition.z, 1 } // T^-1 的平移分量
        });
        var M_RS_inv = MatrixMul(M_R_inv, M_S_inv);
        return MatrixMul(M_RS_inv, M_T_inv);
    }

    /// <summary>
    /// 世界坐标转换到本地坐标
    /// </summary>
    /// <param name="lp"></param>
    /// <param name="worldTF"></param>
    /// <returns></returns>
    public static Vector3 WorldPointToLocalPoint(Vector3 wp, Transform localTF)
    {
        SMatrix v = new SMatrix(new float[,]
        {
            { wp.x, wp.y, wp.z, 1 }
        });

        var m = localTF.GetWorldToLocalMatrix();
        v = MatrixMul(v, m);
        return new Vector3(v[0, 0], v[0, 1], v[0, 2]);
    }


    /// <summary>
    /// 矩阵的转置
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static SMatrix TransposeSelf(this SMatrix m)
    {
        float tmp;
        for (int i = 0; i < m.row; i++)
        {
            for (int j = i + 1; j < m.col; j++)
            {
                tmp = m[i, j];
                m[i, j] = m[j, i];
                m[j, i] = tmp;
            }
        }

        return m;
    }
}