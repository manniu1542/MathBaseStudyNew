using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 判断物体A是否在物体B的右边
/// </summary>
public class JudgeAToBRight : MonoBehaviour
{
    public Transform A;

    public Transform B;

    // Update is called once per frame
    void Update()
    {
        //物体B的正前方向量，
        Vector3 v3BF = B.transform.forward;
        v3BF.y = 0;
        Vector3 v3BA = B.transform.position - A.transform.position;
        if (v3BA.sqrMagnitude < 0.001f)
        {
            Debug.Log("这两个物体在同一个位置无法判断。");
        }

        v3BA.y = 0;
      
        Vector3 v3 = MathSHelper.Cross(v3BF, v3BA);

        //判断物体A是否在物体B的右边
        if (v3.y < 0)
        {
            Debug.Log("物体A是在物体B的右边");
        }
        else
        {
            Debug.Log("物体A是在物体B的左边");
        }
    }
    void OnDrawGizmos()
    {
        Debug.DrawRay(B.position, B.forward, Color.blue);    // B的前方向
        Debug.DrawRay(B.position, A.position - B.position, Color.green); // B→A的向量
        Debug.DrawRay(B.position, MathSHelper.Cross(B.forward, A.position - B.position), Color.red); // 叉乘结果
    }
}