using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToFace : MonoBehaviour
{
    public Transform targetA;

    public Transform targetB;

    //视野范围，（视野远暂时设置位无穷）
    public int viewAnagelA = 90;
    public int viewAnagelB = 90;

    /// <summary>
    /// A正面向量 到AB方向向量的夹角 是视野范围的一半。最大cos值
    /// </summary>
    private float cosAB_ALimit;

    private float cosBA_BLimit;

    // Start is called before the first frame update
    private void Start()
    {
        cosAB_ALimit = Mathf.Cos(viewAnagelA / 2 * Mathf.Deg2Rad);
        cosBA_BLimit = Mathf.Cos(viewAnagelB / 2 * Mathf.Deg2Rad);
    }

    bool IsFaceToFace()
    {
        // 目的检测两个物体是否面对面， 已知两个物体的点位，以及正面向量， 
        //需要满足1，两个人的正面向量 需要小于0钝角。这两个人的朝向才有可能相对。
        // 2.两个人的AB,A->B的向量 ，与 A的正面向量，需要》0锐角，。A才可以看到B。
        //B->A的向量 ，与 B的正面向量，需要》0锐角，。B才可以看到A。
        Vector3 fowradA = targetA.forward;
        Vector3 fowradB = targetB.forward;
        float cosAB = MathSHelper.DotV3(fowradA, fowradB);
        if (cosAB > 0)
        {
            Debug.Log("此时两人不是面朝彼此的");
            return false;
        }

        Vector3 v3AB = targetB.position - targetA.position;
        v3AB = v3AB.normalized;
        float cosAB_A = MathSHelper.DotV3(v3AB, fowradA);
        if (cosAB_A < cosAB_ALimit)
        {
            Debug.Log("此时A视野范围内看不到B");
            return false;
        }

        Vector3 v3BA = -v3AB;

        float cosBA_B = MathSHelper.DotV3(v3BA, fowradB);
        if (cosBA_B < cosBA_BLimit)
        {
            Debug.Log("此时B视野范围内看不到A");
            return false;
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("此时A-B是否是面对面：" + IsFaceToFace());
    }

    private void OnDrawGizmos()
    {
        if (targetA != null && targetB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(targetA.position, targetA.position + targetA.forward * 2);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(targetB.position, targetB.position + targetB.forward * 2);

            // 绘制 A 和 B 的视野范围
            DrawFieldOfView(targetA, viewAnagelA, Color.yellow);
            DrawFieldOfView(targetB, viewAnagelB, Color.green);
        }
    }

    /// <summary>
    /// 绘制视野扇形区域
    /// </summary>
    void DrawFieldOfView(Transform target, float viewAngle, Color color)
    {
        Gizmos.color = color;

        float viewRadius = 3f; // 视野范围半径
        int segmentCount = 20; // 线段数量，越大越平滑

        Vector3 origin = target.position;
        Vector3 forward = target.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        // 画扇形的左右边界
        Gizmos.DrawLine(origin, origin + leftBoundary * viewRadius);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewRadius);

        // 画扇形的弧线（用多个线段拼接成弧形）
        Vector3 lastPoint = origin + leftBoundary * viewRadius;
        for (int i = 1; i <= segmentCount; i++)
        {
            float angle = -viewAngle / 2 + (viewAngle / segmentCount) * i;
            Vector3 nextPoint = origin + (Quaternion.Euler(0, angle, 0) * forward) * viewRadius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}