using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP1 : BossState
{
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    NetworkObject bossAoE = null;

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
            t = Random.Range(time.x, time.y) * 3;
            target = NetworkManager.Singleton.ConnectedClients[(ulong)Random.Range(0, NetworkManager.Singleton.ConnectedClients.Count)].PlayerObject.GetComponent<Character>();
        }

    }


    private void CreateAoE(Transform target)
    {
        NetworkObject go = Instantiate(bossAoE, target.position, Quaternion.identity);
        go.Spawn();
    }
}
