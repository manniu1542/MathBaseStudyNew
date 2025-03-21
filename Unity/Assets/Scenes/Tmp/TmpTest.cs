using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpTest : MonoBehaviour
{
    public Vector3 needNv3;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 v3 = MathSHelper.Cross(Vector3.up, Vector3.right);
        Debug.Log(v3);
        Debug.Log(Vector3.Cross(Vector3.up, Vector3.right));
        // z y 正的。x是负的。
        // z y负的。x是正的。
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(MathSHelper.GetNoraml(needNv3));
    }
}