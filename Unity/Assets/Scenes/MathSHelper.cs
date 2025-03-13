using UnityEngine;


public static class MathSHelper
{
    public static float DotV3(Vector3 one, Vector3 two)
    {
        return one.x * two.x + one.y * two.y + one.z * two.z;
    }
}