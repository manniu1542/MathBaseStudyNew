using UnityEngine;


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
    /// 向量的叉乘 (得出垂直于这两个向量的新向量，且垂直于他们)
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3 Cross(Vector3 one, Vector3 two)
    {
        float x = one.y * two.z - one.z * two.y;
        float y = one.z * two.x - one.x * two.z;
        float z = one.x * two.y - one.y * two.x;
        //右手坐标戏计算的叉乘，需要转换为左手坐标系，z轴负向
        z = -z;
        return new Vector3(x, y, z);
    }
}