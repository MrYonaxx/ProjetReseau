using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct AttackMessage
{
    public int Damage;
    public CharacterSkillAnimation AnimationID;
}

public delegate void ActionTarget(ITargetable target);
public interface ITargetable
{
    void TakeDamage(AttackMessage attackMessage);

    Vector3 GetPos();

    ulong GetNetworkID();

    int GetMaxHealth();
    NetworkVariable<int> GetHealthVariable();

}
