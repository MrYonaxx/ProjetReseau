using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Boss : NetworkBehaviour, ITargetable
{

    [SerializeField]
    int maxHP = 500;

    NetworkVariable<int> health;

    // Start is called before the first frame update
    void Awake()
    {
        health = new NetworkVariable<int>(maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer)
        {

        }
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public ulong GetNetworkID()
    {
        return NetworkObjectId;
    }

    public NetworkVariable<int> GetHealthVariable()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHP;
    }

    public void TakeDamage(AttackMessage attackMessage, double time)
    {
        health.Value = health.Value - attackMessage.Damage;
    }

}
