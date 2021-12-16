using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

public class HUDPlayer : MonoBehaviour
{
    //[SerializeField]
    //ulong idPlayer = 0;
    [SerializeField]
    Transform healthBar = null;
    [SerializeField]
    TextMeshProUGUI textPlayerName = null;

    float maxHP = 0;
    Character character = null;


    private void OnDestroy()
    {
        if(character != null)
            character.GetHealthVariable().OnValueChanged -= DrawHP;
    }

    public void AssignCharacter(Character c, int id)
    {
        gameObject.SetActive(true);

        character = c;
        character.GetHealthVariable().OnValueChanged += DrawHP;
        maxHP = character.GetMaxHealth();
        textPlayerName.text = "Player " + (id).ToString();
        DrawHP(0, character.GetHealthVariable().Value);
    }

    public void DrawHP(int oldHP, int newHP)
    {
        newHP = (int)Mathf.Clamp(newHP, 0, maxHP);
        healthBar.transform.localScale = new Vector3(newHP / maxHP, 1, 1);
    }
}
