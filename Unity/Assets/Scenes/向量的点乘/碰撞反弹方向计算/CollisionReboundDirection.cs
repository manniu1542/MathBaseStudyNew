using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReboundDirection : MonoBehaviour
{
    //给小球个爆发力，让球碰到上方的板子， 求得 反弹的方向， 验证（碰撞到上方板的点，与落到地面时的点 构建的向量 与  计算出的反弹的方向 向量 方向一致）

    Rigidbody rb;

    /// <summary>
    /// 给篮球的 临时速度值
    /// </summary>
    [Range(0, 100)] public int power;


    //向-x轴，与y轴这个方向 投掷，
    public Vector3 throwingDirection = new Vector3(-Mathf.Cos(45 * Mathf.Deg2Rad), Mathf.Sin(45 * Mathf.Deg2Rad), 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
     
    }

    public void ThrowBall()
    {
        rb.AddForce(throwingDirection * power, ForceMode.Impulse);
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
        string name = collision.collider.gameObject.name;
        if (collision.collider.gameObject.name == "Plane")
        {
            hasLanded = true;
            rb.isKinematic = false; // 让球静止，不受物理影响（如果不再移动）
        }

        if (name == "碰撞的板")
        {
            Debug.Log("unity计算的碰撞完后的 方向：" + rb.velocity.normalized);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        string name = collision.collider.gameObject.name;
        if (name == "碰撞的板")
        {
            //因为 2个以 N向量的对称向量（A,B）  符合A+B = 2N,可以 画向量平行四边形的图 ，这个N向量的模长还得符合 A向量在N向量方向的投影模长。
            //注意 要锁定 小球的旋转避免， 旋转造成的 坐标 跟预期不同， 所以要锁定小球的旋转
            //求B的单位向量,

            //A =  rb.velocity   ,N = -collision.collider.gameObject.transform.forward  *A在N上的投影模长
            Vector3 A = -rb.velocity;
            Vector3 N_noraml = collision.collider.gameObject.transform.forward;
            float A2N_Project = MathSHelper.DotV3(A, N_noraml) / N_noraml.magnitude;
            Vector3 N = N_noraml * A2N_Project;
            Vector3 B = 2 * N - A;
          
            Debug.Log($"Plane 的 forward: {collision.collider.gameObject.transform.forward}");
            Debug.Log($"实际碰撞点法线: {collision.contacts[0].normal}");

            Debug.Log("自己计算的碰撞完后的速度方向：" + B.normalized);
            Debug.Log("自己的反射速度方向：" +MathSHelper.ReflectV3(A,N_noraml));
        }


        // 确保只执行一次，避免重复调用
        if (hasLanded && collision.collider.gameObject.name == "Plane")
        {
            Debug.Log("停止");
            hasLanded = false;

            rb.velocity = Vector3.zero; // 停止球的移动
            rb.angularVelocity = Vector3.zero; // 停止球的旋转
            rb.isKinematic = true; // 让球静止，不受物理影响（如果不再移动）
        }
    }
}