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

    [SerializeField]
    BossState currentState = null;

    // Start is called before the first frame update
    void Awake()
    {
        health = new NetworkVariable<int>(maxHP);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SetState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer)
        {
            currentState.UpdateState(this);
        }
    }

    public void SetState(BossState newState)
    {
        currentState = newState;
        currentState.StartState(this);
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
