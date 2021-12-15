using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;


public enum AnimationBoss
{
    Damage,
    Attack1,
    Attack2,
    Attack3,
    Damage2,
    Dead
}

public class Boss : NetworkBehaviour, ITargetable
{

    [SerializeField]
    int maxHP = 500;

    NetworkVariable<int> health;

    [SerializeField]
    BossState currentState = null;

    Animator animator = null;
    int hit = 1;
    bool isDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        hit = 1;
        health = new NetworkVariable<int>(maxHP);
        animator = GetComponentInChildren<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SetState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer && !isDead)
        {
            currentState.UpdateState(this);
        }
    }

    public void SetState(BossState newState)
    {
        currentState = newState;
        currentState.StartState(this);
    }


    // Aucun interet de jouer l'anim sur le serveur
    public void PlayBossAnimation(AnimationBoss id)
    {
        PlayBossAnimationClientRpc(id);
    }
    [ClientRpc]
    public void PlayBossAnimationClientRpc(AnimationBoss id)
    {
        switch(id)
        {
            case AnimationBoss.Damage:
                animator.SetTrigger("Damage");
                break;
            case AnimationBoss.Damage2:
                animator.SetTrigger("Damage2");
                break;
            case AnimationBoss.Attack1:
                animator.SetTrigger("Attack1");
                break;
            case AnimationBoss.Attack2:
                animator.SetTrigger("Attack2");
                break;
            case AnimationBoss.Attack3:
                animator.SetTrigger("Attack3");
                break;
            case AnimationBoss.Dead:
                animator.SetTrigger("Dead");
                break;
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

    public void TakeDamage(AttackMessage attackMessage)
    {
        if (isDead)
            return;

        health.Value = health.Value - attackMessage.Damage;
        if(health.Value < 0 && !isDead)
        {
            PlayBossAnimationClientRpc(AnimationBoss.Dead);
            isDead = true;
        }
        else if(attackMessage.Damage > 0)
        {
            hit *= -1;
            if(hit < 0)
                PlayBossAnimationClientRpc(AnimationBoss.Damage);
            else
                PlayBossAnimationClientRpc(AnimationBoss.Damage2);
        }
    }

}
