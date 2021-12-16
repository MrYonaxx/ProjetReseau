using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateStandby : NetworkBehaviour
{
    [SerializeField]
    Boss boss = null;
    [SerializeField]
    BossState stateP1 = null;

    void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            Character c = other.GetComponent<Character>();
            if (c != null)
            {
                boss.SetState(stateP1);
            }
            Destroy(this.gameObject);
        }
    }

}
