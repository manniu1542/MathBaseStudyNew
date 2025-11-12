using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数学帮助类。（右手坐标系的数值 ，如需转在unity渲染需要z反转为-z）
/// </summary>
public static class MathSHelper
{
    /// <summary>
    /// 点乘
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public static float DotV3(Vector3 one, Vector3 two)
    {
        if (one == Vector3.zero || two == Vector3.zero)
            return 0;
        return one.x * two.x + one.y * two.y + one.z * two.z;
    }

    /// <summary>
    /// 判断是否是法向量
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool IsNoramlV3(ref Vector3 normal)
    {
        float normalMagnitude = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
        bool isNoraml = normalMagnitude >= 0.998 && normalMagnitude < 1.002;

        //确认是法向量
        if (!isNoraml)
        {
            Debug.Log("传入的法向量有误！");
        }

        return isNoraml;
    }

    /// <summary>
    /// 求得反射向量  （前提是计算， 入射向量，与法向量 夹角小于等于90，才有物理几何意义）
    ///  2个以 N向量的对称向量（A,B）  符合A+B = 2N,可以 画向量平行四边形的图 ，这个N向量的模长还得符合 A向量在N向量方向的投影模长。
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public static Vector3 ReflectV3(Vector3 inDir, Vector3 normal, bool isOutDirNoraml = true)
    {
        Vector3 outDir = Vector3.zero;

        //确认是法向量
        if (!IsNoramlV3(ref normal))
        {
            normal = normal.normalized; // 自动归一化
        }

        //确认夹角小于等于90 (这个dot也是inDir在N向量上的投影长度)
        float dot = DotV3(inDir, normal);
        if (dot < 0)
        {
            Debug.Log("入射向量和法向量夹角大于等于 90°，尝试反转法向量！");
            normal = -normal;
        }

        outDir = 2 * dot * normal - inDir;
        if (isOutDirNoraml)
        {
            outDir = outDir.normalized;
        }

        return outDir;
    }

    /// <summary>
    /// 单位向量
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3 GetNoraml(Vector3 v3)
    {
        if (v3.x == 0 && v3.y == 0 && v3.z == 0)
        {
            Debug.LogError("零向量没有模长!");
        }

        //当前响亮的模长
        float magnitude = Mathf.Sqrt(v3.x * v3.x + v3.y * v3.y + v3.z * v3.z);
        //缩放系数 ,给这个向量缩放多少，才能使这个向量的模场 =1
        float kScale = 1 / magnitude;

        return v3 * kScale;
    }


    /// <summary>
    /// 相对于任意轴的方向量n缩放k倍
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3 PRelativeToNScaledK(Vector3 P, Vector3 N, float k)
    {
        N.Normalize();
        float dotPN = DotV3(P, N);
        Vector3 kProjectionPN = N * dotPN;
        kProjectionPN = (k - 1) * kProjectionPN;
        return P + kProjectionPN;
    }

    /// <summary>
    /// 向量的叉乘 (得出垂直于这两个向量的新向量，且垂直于他们) 
    /// 【旋转判断为相反方向】  位置相对关系 与坐标系无关
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3 Cross(Vector3 one, Vector3 two)
    {
        float x = one.y * two.z - one.z * two.y;
        float y = one.z * two.x - one.x * two.z;
        float z = one.x * two.y - one.y * two.x;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 绕轴旋转 罗德里斯旋转公式  p新 = (p⋅n)n+(p−(p⋅n)n)cosθ+(n×p)sinθ
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="v3"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 RotationAboutTheAxis(Vector3 nAxis, Vector3 v3p, float angle)
    {
        nAxis.Normalize();
        Vector3 v3ON = DotV3(v3p, nAxis) * nAxis;
        Vector3 v3NM = (v3p - v3ON) * Mathf.Cos(angle * Mathf.Deg2Rad);
        Vector3 v3MP = Cross(nAxis, v3p) * Mathf.Sin(angle * Mathf.Deg2Rad);
        return v3ON + v3NM + v3MP;
    }

    #region 坐标系

      
    public static Vector3 LocalPointToWorldPoint(Transform tfLocal, Vector3 localPos)
    {
        // 世界坐标 = 本地坐标系原点 + 直立坐标
        return LocalPointToWorldPoint(tfLocal.right, tfLocal.up, tfLocal.forward,
            tfLocal.parent.position, localPos);
        ;
    }

    /// <summary>
    /// 将本地（物体）坐标系下的点转换为世界坐标系下的点。
    /// </summary>
    /// <param name="localBaseAxisX">本地坐标系的X轴（右方向）在世界坐标系下的表示</param>
    /// <param name="localBaseAxisY">本地坐标系的Y轴（上方向）在世界坐标系下的表示</param>
    /// <param name="localBaseAxisZ">本地坐标系的Z轴（前方向）在世界坐标系下的表示</param>
    /// <param name="localOriginPoint">本地坐标系原点在世界坐标系下的位置</param>
    /// <param name="localPoint">本地坐标系下的点</param>
    /// <returns>转换后的世界坐标系下的点</returns>
    public static Vector3 LocalPointToWorldPoint(
        Vector3 localBaseAxisX,
        Vector3 localBaseAxisY,
        Vector3 localBaseAxisZ,
        Vector3 localOriginPoint,
        Vector3 localPoint)
    {
        // 世界坐标 = 本地坐标系原点 + 直立坐标
        return localOriginPoint + LocalPointToUprightPoint(localBaseAxisX, localBaseAxisY, localBaseAxisZ, localPoint);
    }

    /// <summary>
    /// 将本地（物体）坐标系下的点转换为直立坐标系下的点。
    /// </summary>
    /// <param name="localBaseAxisX">本地坐标系的X轴（右方向）在世界坐标系下的表示</param>
    /// <param name="localBaseAxisY">本地坐标系的Y轴（上方向）在世界坐标系下的表示</param>
    /// <param name="localBaseAxisZ">本地坐标系的Z轴（前方向）在世界坐标系下的表示</param>
    /// <param name="localOriginPoint">本地坐标系原点在世界坐标系下的位置</param>
    /// <param name="localPoint">本地坐标系下的点</param>
    /// <returns>转换后的世界坐标系下的点</returns>
    public static Vector3 LocalPointToUprightPoint(
        Vector3 localBaseAxisX,
        Vector3 localBaseAxisY,
        Vector3 localBaseAxisZ,
        Vector3 localPoint)
    {
        // 计算直立坐标：
        // 直立坐标 = 本地坐标在直立坐标系（由 localBaseAxisX/Y/Z 定义）下的投影
        Vector3 v3Upright = localPoint.x * localBaseAxisX + localPoint.y * localBaseAxisY +
                            localPoint.z * localBaseAxisZ;


        return v3Upright;
    }

    /// <summary>
    /// 本地坐标 转 直立 坐标 
    /// </summary>
    /// <param name="nAxis"></param>
    /// <param name="v3p"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 WorldPointToUprightPoint(Vector3 localOrignPoint, Vector3 pointWolrd)
    {
        return pointWolrd - localOrignPoint;
    }

    /// <summary>
    /// 本地坐标 转 世界 坐标 
    /// </summary>
    /// <param name="nAxis"></param>
    /// <param name="v3p"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 WorldPointTolocalPoint(Vector3 localBaseAxisX, Vector3 localBaseAxisY, Vector3 localBaseAxisZ,
        Vector3 localOrignPoint, Vector3 pointWolrd)
    {
        //直立坐标
        Vector3 v3Upright = WorldPointToUprightPoint(localOrignPoint, pointWolrd);
        Vector3 pLocalV3 = Vector3.zero;
        //分量在 对应本地坐标轴上投影
        pLocalV3.x = DotV3(localBaseAxisX, v3Upright);
        pLocalV3.y = DotV3(localBaseAxisY, v3Upright);
        pLocalV3.z = DotV3(localBaseAxisZ, v3Upright);


        return pLocalV3;
    }

    public static Vector3 WorldPointTolocalPoint(Transform tfLocalParent, Vector3 worldPos)
    {
        return WorldPointTolocalPoint(tfLocalParent.right, tfLocalParent.up, tfLocalParent.forward,
            tfLocalParent.position, worldPos);
    }
    

    #endregion
  
    
    
    
}