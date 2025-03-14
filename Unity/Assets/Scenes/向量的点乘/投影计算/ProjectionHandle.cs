using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionHandle : MonoBehaviour
{
    //篮球投篮 ，斜向上 给个爆发力， 求 横向的 投影，以及 向上的投影 。分别是多少。 可以计算出。球落地的时候 ，横向 移动了多远么？

    Rigidbody rb;

    /// <summary>
    /// 给篮球的 临时速度值
    /// </summary>
    [Range(0, 100)] public int speed;

    //给篮球的 临时速度方向
    [Range(0, 90)] public float throwingAngle;

    //向-x轴，与y轴这个方向 投掷，
    private Vector3 throwingDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ThrowBall()
    {
        //-x轴
        float negativeSignX = -Mathf.Cos(throwingAngle * Mathf.Deg2Rad);
        //+y轴
        float correctSignY = Mathf.Sin(throwingAngle * Mathf.Deg2Rad);
        throwingDirection = new Vector3(negativeSignX, correctSignY, 0);


        Vector3 speedv3 = throwingDirection * speed;
        Debug.Log("投篮给球的临时速度：" + speedv3);
        rb.AddForce(speedv3, ForceMode.VelocityChange);

        // 这就是 -x轴的 投影，即为他的速度分量= (速度 点乘  -x轴单位向量 )/-x轴单位模长
        float _xSpeed = MathSHelper.DotV3(speedv3, Vector3.left) / 1;
        float ySpeed = MathSHelper.DotV3(speedv3, Vector3.up) / 1;

        Debug.Log($"此时给-x轴的速度分量是：{_xSpeed}，+y轴的速度分量是：{ySpeed}");

        float g = Physics.gravity.magnitude;
        //高度的速度公式是 ：   v末速度= v向上的初速度 +at;  求t是多少。（已知 有两段运动，1个是向上的 1个是落下， ）
        //向上时候 v末速度=0， v初速度值 = yspeed+g重力加速度； t上= - yspeed/g,  
        //TODO:以后 学习 位移 速度 加速度，匀速运动的公式，就可以推导出来，向上的速度，跟向下的速度一致了。
        //向下时候 v末速度值 = yspeed ， v初速度= 0，g重力加速度,  t下= - yspeed/g,   
        // 因为 g是 -9.81 所以 可以抵消 负号。
        float flightTime = (2 * ySpeed) / g;
        float horizontalDistance = _xSpeed * flightTime;

        Debug.Log($"球落地时，水平移动的距离：{horizontalDistance}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowBall();
        }
    }

    public bool hasLanded = false;
    void OnCollisionExit(Collision collision)
    { 
        Debug.Log("开始");
        hasLanded = true;
        rb.isKinematic = false; // 让球静止，不受物理影响（如果不再移动）
    }
    void OnCollisionEnter(Collision collision)
    {
        // 确保只执行一次，避免重复调用
        if (hasLanded)
        {
            Debug.Log("停止");
            hasLanded = false;
            
            rb.velocity = Vector3.zero; // 停止球的移动
            rb.angularVelocity = Vector3.zero; // 停止球的旋转
            rb.isKinematic = true; // 让球静止，不受物理影响（如果不再移动）
        }
    }
}