using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP3_LightRampant : BossState
{
    [Space]
    [Space]
    [SerializeField]
    Vector2Int time;
    [SerializeField]
    float timeBeforeDamage;
    [SerializeField]
    float damage;


    [Space]
    [SerializeField]
    BossState[] endStates = null;

    float t = 0f;
    bool lightRampantOn = false;

    public override void StartState(Boss boss)
    {
        t = Random.Range(time.x, time.y);
        lightRampantOn = false;
    }

    public override void UpdateState(Boss boss)
    {
        t -= Time.deltaTime;
        if(t <= 0 && !lightRampantOn)
        {
            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack1);
            List<int> lightRampant = new List<int>{ 1, 2, 3 };
            for (int i = 0; i < PlayerTeam.Instance.players.Count; i++)
            {
                int r = Random.Range(0, lightRampant.Count);
                PlayerTeam.Instance.players[i].SetLightRampant(lightRampant[r]);
                lightRampant.RemoveAt(r);
            }

            lightRampantOn = true;
            t = timeBeforeDamage;
        }
        else if(t <= 0)
        {
            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack3);
            AttackMessage attackMessage = new AttackMessage();
            attackMessage.Damage = (int)damage;
            for (int i = 0; i < PlayerTeam.Instance.players.Count; i++)
            {
                if (PlayerTeam.Instance.players[i].lightRampantID != 0)
                {
                    PlayerTeam.Instance.players[i].TakeDamage(attackMessage);
                    PlayerTeam.Instance.players[i].SetLightRampant(0);
                }
            }
            if (!CheckNextPhase(boss))
                boss.SetState(endStates[Random.Range(0, endStates.Length)]);
        }

    }


}
