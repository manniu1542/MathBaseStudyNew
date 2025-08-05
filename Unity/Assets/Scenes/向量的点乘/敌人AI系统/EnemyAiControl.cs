using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyAiControl : MonoBehaviour
{
    // 正前方 60° 。 距离 自己10米内。
    //敌人转向玩家， 并移动追击 玩家。否则 就停止

    public Transform target;
    public float attDistance = 10f;
    public float attAngle = 60f / 2;

    public float moveSpeed = 10f;
    public float angleSpeed = 10f;

    void Start()
    {
    }

    bool IsCanFollowTarget()
    {
        Vector3 v3 = target.position - transform.position;
        //当前响亮的模长
        float magnitude = Mathf.Sqrt(v3.x * v3.x + v3.y * v3.y + v3.z * v3.z);
        if (magnitude < attDistance)
        {
            return false;
        }

        float cosθ = MathSHelper.DotV3(transform.forward, v3) / magnitude;
        float angle = Mathf.Acos(cosθ) * Mathf.Rad2Deg;
        if (angle > attAngle)
            return false;

        return true;
    }

    void LookAtTarget()
    {
        Vector3 toTarget = target.position - transform.position;
        Vector3 currentDir = transform.forward;

        Vector3 axis = MathSHelper.Cross(currentDir, toTarget);

        float sinθ = axis.magnitude / toTarget.magnitude;
        float angle = Mathf.Asin(sinθ) * Mathf.Rad2Deg;

        if (angle > 0.01f && axis != Vector3.zero)
        {
            Vector3 tmp = transform.rotation.eulerAngles;
            angle =    Mathf.Min(Time.deltaTime * angleSpeed, angle);
            tmp += axis.normalized * angle;
            transform.rotation = Quaternion.Euler(tmp);
        }
    }

    void MoveToTarget()
    {
        Vector3 v3 = target.position - transform.position;
        transform.position += v3.normalized * Time.deltaTime * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsCanFollowTarget()) return;

        LookAtTarget();
        MoveToTarget();
    }
}