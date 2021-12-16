using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.UI;

public class HUDSkill : MonoBehaviour
{

    [SerializeField]
    Image imageCooldown = null;
    [SerializeField]
    int skillID = 0;
    [SerializeField]
    int gcd = 0;

    Character character;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AssignCharacter;
        this.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (character != null)
            character.OnSkillUsed -= DrawCooldown;
    }




    public void AssignCharacter(ulong id)
    {
        this.gameObject.SetActive(true);
        if (id == NetworkManager.Singleton.LocalClientId)
        {
            character = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Character>();
            character.OnSkillUsed += DrawCooldown;
            NetworkManager.Singleton.OnClientConnectedCallback -= AssignCharacter;
        }
    }


    public void DrawCooldown(AttackData attack)
    {

        if(attack.ID == skillID)
        {
            StopAllCoroutines();
            StartCoroutine(CooldownCoroutine(attack.Cooldown + gcd));
        }
        else
        {
            StartCoroutine(CooldownCoroutine(gcd));
        }

    }

    private IEnumerator CooldownCoroutine(float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            imageCooldown.fillAmount = 1 - (t / time);
            yield return null;
        }
    }
}
