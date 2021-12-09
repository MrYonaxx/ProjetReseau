using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// C'est pas generic pour un vrai jeu mais ça fera l'affaire pour le proto
public class BossState : NetworkBehaviour
{
    public virtual void StartState(Boss boss)
    {

    }
    public virtual void UpdateState(Boss boss)
    {

    }
}
