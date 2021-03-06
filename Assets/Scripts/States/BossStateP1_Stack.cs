using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP1_Stack : BossState
{
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    NetworkObject bossAoE = null;

    [SerializeField]
    BossState[] endStates = null;

    List<Character> players;
    Character target = null;

    float t = 0f;

    public override void StartState(Boss boss)
    {
        t = Random.Range(time.x, time.y);
        target = NetworkManager.Singleton.ConnectedClients[(ulong)Random.Range(0, NetworkManager.Singleton.ConnectedClients.Count)].PlayerObject.GetComponent<Character>();
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
            target = NetworkManager.Singleton.ConnectedClients[(ulong)Random.Range(0, NetworkManager.Singleton.ConnectedClients.Count)].PlayerObject.GetComponent<Character>();
            
            if (!CheckNextPhase(boss))
                boss.SetState(endStates[Random.Range(0, endStates.Length)]);
        }

    }


    private void CreateAoE(Transform target)
    {
        NetworkObject go = Instantiate(bossAoE, target.position, Quaternion.identity);
        go.Spawn();
    }
}
