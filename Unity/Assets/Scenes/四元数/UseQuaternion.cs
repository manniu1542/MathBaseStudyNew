using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UseQuaternion : MonoBehaviour
{
    public bool isSlerp;

    public float speed = 0.5f;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        //欧拉=》四元数
        Quaternion q = Quaternion.Euler(new Vector3(90, 0, 0));
        Debug.Log($"Quaternion.Euler(new Vector3(90, 0, 60)) = {q}");
        //四元数=》欧拉
        Debug.Log($"q.eulerAngles = {q.eulerAngles}");
        //计算两个旋转角的角度差标量
        Quaternion q2 = Quaternion.Euler(new Vector3(60, 45, 45));
        float angel = Quaternion.Angle(q, q2);
        Debug.Log($"angel = {angel}");
        //计算旋转朝向四元数，让当前物体的x轴 指向  朝向目标的方向，（传入两个方向的方向的法向量） 有万向锁问题主要传入的参数问题。最短旋转，计算效率高
        // transform.rotation = Quaternion.FromToRotation(transform.right, (target.position - transform.position).normalized);
        
        //计算旋转朝向四元数，让当前物体的x轴 朝向  目标方向，（传入方向的方向的法向量默认世界坐标的z轴为前方。让z轴朝向 转向的朝向轴）比 Quaternion.FromToRotation计算更为复杂。里面处理了
        Vector3 targetDirection = (target.position - transform.position).normalized;
        // Vector3 customUp = new Vector3(0, 1, 0); // 让物体的 向上的轴 尽可能 的上？ 暂时不理解。可以不传入这个参数
        transform.rotation = Quaternion.LookRotation(targetDirection);
        
        
    }
   

    void Update()
    {

  
    }

    /// <summary>
    /// 按照旋转量来累加旋转到指定 方向,梯度一致maxDegressDelta保证每次只旋转这么多度。计算更为繁琐了
    /// </summary>
    public void RotateTowardsRotation()
    {
     
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(180, 0, 60)), 0.1f);
    }

    #region 按时间来进行插值旋转

    public float timeSlerpUnclamped;
    /// <summary>
    /// 持续插值旋转
    /// </summary>
    public void SlerpUnclampedRotation()
    {
        timeSlerpUnclamped += Time.deltaTime * speed;
        transform.rotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(new Vector3(180, 0, 60)), timeSlerpUnclamped );
    }
    
    public void SlerpRotation()
    {
        //旋转计算精确。计算繁琐。 旋转表现好
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(180, 0, 60)),
            Time.deltaTime * speed);
    }

    public void lerpRotation()
    {
        //线性旋转 性能好，旋转角度小于90度根Quaternion.Slerp 差异不大。 或者一些2d的旋转 ，不涉及 3d都可用这个旋转
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(361, 0, 60)),
            Time.deltaTime * speed);
    }

    #endregion
    
  
}