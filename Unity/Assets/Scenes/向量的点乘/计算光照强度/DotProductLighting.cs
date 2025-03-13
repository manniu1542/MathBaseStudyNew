using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotProductLighting : MonoBehaviour
{
    public Light light;
    public MeshRenderer render;

    /// <summary>
    /// 光照方向
    /// </summary>
    Vector3 lightDir;

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.ambientLight = Color.black;
        lightDir = light.transform.forward;
    }

    void SimulateCalculateLight()
    {
        //自己的法向量
        Vector3 selfNormalize = transform.up;
        //让自己的法向量和光照的方向一致。算夹角 更准确
        selfNormalize *= -1;
        //因为点乘可以知道 两个向量之间的角度关系
        float dot = Vector3.Dot(selfNormalize, lightDir);
        //因为算的两个向量都是 单位向量。所以 dot结果也是 两个向量夹角的cos值。 取值范围也是 【-1，1】,小于0的时候钝角 无光。black即可， ，lerp的计算范围是 【0，1】
        float progress = dot;
        Debug.Log("光照强度：" + progress);
        render.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.white, progress));
    }

    // Update is called once per frame
    void Update()
    {
        SimulateCalculateLight();
    }
}