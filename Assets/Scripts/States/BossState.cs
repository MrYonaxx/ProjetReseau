using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossState : NetworkBehaviour
{

    [Header("Next Phase")]
    [SerializeField]
    float healthThreshold = 0.7f;
    [SerializeField]
    BossState nextPhaseState = null;

    public virtual void StartState(Boss boss)
    {

    }
    public virtual void UpdateState(Boss boss)
    {

    }

    protected bool CheckNextPhase(Boss boss)
    { 
        if(boss.GetHealthVariable().Value < (boss.GetMaxHealth() * healthThreshold))
        {
            boss.SetState(nextPhaseState);
            return true;
        }
        return false;
    }
}
