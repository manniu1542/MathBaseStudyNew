using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnterEnemyRange : MonoBehaviour
{
    public ViewRangeGizmo scrVrg;

    //人进入敌人的范围判断： 1.  敌人到人的向量。 2.与敌人正前方 所成的夹角。 3.小于等于 敌人可视范围夹角的一半，敌人可能看到 ，4. 再接着判断玩家距离敌人的距离 小于等于 敌人的可视范围 
    // Start is called before the first frame update
    bool CheckEnterEnmey()
    {
        Vector3 enemy2self = transform.position - scrVrg.transform.position;
        enemy2self.y = 0;
        Vector3 enemyForward = scrVrg.transform.forward;
        enemyForward.y = 0;

        //简短判断 人在敌人的 正方向
        float dot = MathSHelper.DotV3(enemy2self, enemyForward);
        if (dot < 0)
        {
            Debug.Log("人在敌人的背面");
            return false;
        }


        float cosA = dot / (enemy2self.magnitude * enemyForward.magnitude);

        float angle = Mathf.Acos(cosA) * Mathf.Rad2Deg;
        Debug.Log("当前夹角：" + angle);
        float enmeyViewHalf = scrVrg.viewAngle / 2;
        if (enmeyViewHalf < angle)
        {
            Debug.Log("人在敌人 视野 范围外");
            return false;
        }

        float e2s_distance = enemy2self.magnitude;

        if (e2s_distance > scrVrg.viewDistance)
        {
            Debug.Log("人在敌人 敏感 范围外");
            return false;
        }


        return true;
    }



    // Update is called once per frame
    void Update()
    {
        scrVrg.isEnter = CheckEnterEnmey();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, scrVrg.transform.position);
    }
}