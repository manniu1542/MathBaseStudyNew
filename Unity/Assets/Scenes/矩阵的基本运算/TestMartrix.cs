using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestMartrix : MonoBehaviour
{
    private SMatrix matrix1;

    private SMatrix matrix2;

    public Transform target;

    public Transform worldPos;

    public Transform LocalPos;

    // Start is called before the first frame update
    void Start()
    {
    
        matrix1 = new SMatrix(new float[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        matrix2 = new SMatrix(new float[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
        var result1 = matrix2.MatrixMul(2);
        var result = SMatrixHelper.MatrixMul(matrix1, result1);

        Vector3 v3 = new Vector3(1, 2, 3);
        v3 = SMatrixHelper.V3LinearTransformation(v3, result1);


        Vector3 v32 = SMatrixHelper.V3CrossProduct(Vector3.right, Vector3.forward);
        Debug.Log(v32.ToString());
        Discrete();
    }



    public void Update()
    {
        var v = SMatrixHelper.WorldPointToLocalPoint(worldPos.position, target);
        Debug.Log("世界点=》本地 矩阵转换：" + v);
        var v2 = SMatrixHelper.LocalPointToWorldPoint(LocalPos.localPosition, LocalPos.parent);
        Debug.Log("本地点=》世界 矩阵转换：" + v2);
    }

    /// <summary>
    /// 离散积分
    /// </summary>
    /// <returns></returns>
    private SMatrix integralMatrix;

    /// <summary>
    /// 离散差分
    /// </summary>
    /// <returns></returns>
    private SMatrix diffMatrix;


    public void Discrete()
    {
        //先差分（滤波）在积分（还原），可还原矩阵
        integralMatrix = new SMatrix(new float[,] { { 1, 1, 1 }, { 0, 1, 1 }, { 0, 0, 1 } });
        diffMatrix = new SMatrix(new float[,] { { 1, -1, 0 }, { 0, 1, -1 }, { 0, 0, 1 } });
        // 原始向量（理想线性趋势）
        Vector3 original = new Vector3(1f, 2f, 3f);

        // 模拟噪声扰动（高频噪声）
        Vector3 noisy = original + new Vector3(0.1f, 0.4f, -0.2f);
        Debug.Log("带噪声：" + noisy);

        // 差分（提取变化量 / 滤波）
        Vector3 filtered = SMatrixHelper.V3LinearTransformation(noisy, diffMatrix);
        Debug.Log("差分后：" + filtered);

        // 积分（尝试恢复趋势）
        Vector3 recovered = SMatrixHelper.V3LinearTransformation(filtered, integralMatrix);
        Debug.Log("恢复后：" + recovered);

        // 对比误差
        float error = (original - recovered).magnitude;
        Debug.Log("还原误差：" + error);
    }
}