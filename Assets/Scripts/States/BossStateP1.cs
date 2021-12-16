using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP1 : BossState
{
    [Space]
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    NetworkObject bossAoE = null;
    [SerializeField]
    bool tryParent = false;

    [SerializeField]
    BossState[] endStates = null;


    List<Character> players;
    Character target = null;

    float t = 0f;

    public override void StartState(Boss boss)
    {
        t = Random.Range(time.x, time.y);
        target = PlayerTeam.Instance.players[Random.Range(0, PlayerTeam.Instance.players.Count)];
    }
    public override void UpdateState(Boss boss)
    {
        t -= Time.deltaTime;
        boss.transform.LookAt(target.transform, Vector3.up);
        boss.transform.eulerAngles = new Vector3(0, boss.transform.eulerAngles.y, 0);
        if(t <= 0)
        {
            CreateAoE(target.transform);
            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack1);
            t = Random.Range(time.x, time.y) * 3;
            target = PlayerTeam.Instance.players[Random.Range(0, PlayerTeam.Instance.players.Count)]; 

            if(!CheckNextPhase(boss))
                boss.SetState(endStates[Random.Range(0, endStates.Length)]);
        }

    }


    private void CreateAoE(Transform target)
    {
        NetworkObject go = Instantiate(bossAoE, target.position, Quaternion.identity);
        go.Spawn();
        if (tryParent)
            go.TrySetParent(target);
    }
}
