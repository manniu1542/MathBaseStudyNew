using UnityEngine;

public class ViewRangeGizmo : MonoBehaviour
{
    public float viewDistance = 5f; // 观察距离
    public float viewAngle = 45f; // 视野角度（左右各一半）
    public int segmentCount = 10; // 细分数量（值越大越平滑）

    public bool isEnter = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = isEnter ? Color.red : Color.green;
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward * viewDistance;

        // 计算扇形的左边界和右边界
        float halfAngle = viewAngle * 0.5f;
        Vector3 leftBoundary = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, halfAngle, 0) * forward;

        // 绘制扇形的两条边
        Gizmos.DrawLine(origin, origin + leftBoundary);
        Gizmos.DrawLine(origin, origin + rightBoundary);

        // 逐步绘制扇形的弧线
        Vector3 previousPoint = origin + leftBoundary;
        for (int i = 1; i <= segmentCount; i++)
        {
            float angleStep = (viewAngle / segmentCount) * i - halfAngle;
            Vector3 newPoint = Quaternion.Euler(0, angleStep, 0) * forward + origin;

            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}