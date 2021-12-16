using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP1_Spread : BossState
{
    [Space]
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    NetworkObject bossAoE = null;
    [SerializeField]
    float aoeScale = 2;

    [SerializeField]
    BossState[] endStates = null;

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
            for (int i = 0; i < PlayerTeam.Instance.players.Count; i++)
            {
                CreateAoE(PlayerTeam.Instance.players[i].transform);
            }

            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack2);
            t = Random.Range(time.x, time.y) * 3;

            if (!CheckNextPhase(boss))
                boss.SetState(endStates[Random.Range(0, endStates.Length)]);
        }
    }


    private void CreateAoE(Transform target)
    {
        NetworkObject go = Instantiate(bossAoE, target.position, Quaternion.identity);
        go.Spawn();
        go.TrySetParent(target);
        go.transform.localScale = new Vector3(aoeScale, 1, aoeScale);
    }
}
