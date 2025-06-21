using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInCamara : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
    }

    bool IsPointInCameraView(Vector3 point, Camera cam)
    {
        // 计算相机到点的向量
        Vector3 toPoint = point - cam.transform.position;
        float distance = toPoint.magnitude;

        // 1️⃣ 深度检测（near < distance < far）
        if (distance < cam.nearClipPlane || distance > cam.farClipPlane)
            return false;

        // 计算相机前向量
        Vector3 forward = cam.transform.forward;

        // 2️⃣ 左右可视范围（XZ 平面）
        Vector3 forwardXZ = new Vector3(forward.x, 0, forward.z).normalized;
        Vector3 toPointXZ = new Vector3(toPoint.x, 0, toPoint.z).normalized;
        float cosThetaXZ = Vector3.Dot(forwardXZ, toPointXZ);
        float fovHalfHorizontal = cam.fieldOfView * cam.aspect * 0.5f; // 水平方向的 FOV 一半
        if (cosThetaXZ < Mathf.Cos(fovHalfHorizontal * Mathf.Deg2Rad))
            return false;
        Debug.Log("--" + cam.aspect);
        // 3️⃣ 上下可视范围（ZY 平面）
        Vector3 forwardZY = new Vector3(0, forward.y, forward.z).normalized;
        Vector3 toPointZY = new Vector3(0, toPoint.y, toPoint.z).normalized;
        float cosThetaZY = Vector3.Dot(forwardZY, toPointZY);
        float fovHalfVertical = cam.fieldOfView * 0.5f; // 垂直方向的 FOV 一半
        if (cosThetaZY < Mathf.Cos(fovHalfVertical * Mathf.Deg2Rad))
            return false;

        // 通过所有检测，说明点在相机视野范围内
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("物体的中心点是否能被i相机看到：" + IsPointInCameraView(target.position, Camera.main));
    }
}