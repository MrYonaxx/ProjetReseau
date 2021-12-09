using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateStandby : BossState
{
    [SerializeField]
    BossState stateP1 = null;
    [SerializeField]
    float waitChangeState = 2f;


    float t = 0f;
    bool start = false;

    public override void StartState(Boss boss)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count != 0)
            start = true;
        else
            NetworkManager.Singleton.OnClientConnectedCallback += StartBoss;
        t = waitChangeState;
    }

    public override void UpdateState(Boss boss)
    {
        if (start)
        {
            t -= Time.deltaTime;
            if (t <= 0)
                boss.SetState(stateP1);
        }
    }

    void StartBoss(ulong id)
    {   
        start = true;
        NetworkManager.Singleton.OnClientConnectedCallback -= StartBoss;
    }
}
