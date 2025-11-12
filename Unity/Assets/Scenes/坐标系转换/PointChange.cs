using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointChange : MonoBehaviour
{
    public Transform tfLocal;

    public Transform tfW2LParent;
    public Transform tfWolrd;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 local = MathSHelper.WorldPointTolocalPoint(tfW2LParent, tfWolrd.position);
        Debug.Log("世界转本地坐标：" + local);
        Vector3 localUnity = tfW2LParent.InverseTransformPoint(tfWolrd.position);
        Debug.Log("世界转本地坐标（Unity内置）：" + localUnity);


        Vector3 world = MathSHelper.LocalPointToWorldPoint(tfLocal, tfLocal.localPosition);
        Debug.Log("本地转世界坐标：" + world);
        Vector3 worldUnity = tfLocal.TransformPoint(Vector3.zero);
        Debug.Log("本地转世界坐标（Unity内置）：" + worldUnity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
