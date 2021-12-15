using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP1_AoEArround : BossState
{
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    NetworkObject bossAoE = null;
    [SerializeField]
    float aoeScale = 5;

    [SerializeField]
    BossState[] endStates = null;

    //List<Character> players;
    //Character target = null;

    float t = 0f;

    public override void StartState(Boss boss)
    {
        t = Random.Range(time.x, time.y);
    }

    public override void UpdateState(Boss boss)
    {
        t -= Time.deltaTime;
        if(t <= 0)
        {
            CreateAoE(transform);
            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack3);
            boss.SetState(endStates[Random.Range(0, endStates.Length - 1)]);
        }

    }


    private void CreateAoE(Transform target)
    {
        NetworkObject go = Instantiate(bossAoE, target.position, Quaternion.identity);
        go.Spawn();
        go.transform.localScale = new Vector3(aoeScale, 1, aoeScale);
    }
}
