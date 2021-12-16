using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP2 : BossState
{
    [Space]
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    AreaOfHeal healAoE = null;

    [SerializeField]
    BossState[] endStates = null;

    float t = 0f;

    public override void StartState(Boss boss)
    {
        t = Random.Range(time.x, time.y);
        boss.transform.LookAt(healAoE.transform, Vector3.up);
        boss.transform.eulerAngles = new Vector3(0, boss.transform.eulerAngles.y, 0);
        boss.PlayBossAnimationClientRpc(AnimationBoss.Attack3);
        healAoE.SetActive(true);
    }

    public override void UpdateState(Boss boss)
    {
        t -= Time.deltaTime;
        if(t <= 0)
        {
            boss.SetState(endStates[Random.Range(0, endStates.Length)]);
        }

    }

}
