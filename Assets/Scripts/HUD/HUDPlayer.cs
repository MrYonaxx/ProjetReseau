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

    // Start is called before the first frame update
    /*void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AssignCharacter;
    }*/

    private void OnDestroy()
    {
        if(character != null)
            character.GetHealthVariable().OnValueChanged -= DrawHP;
    }




    // Faire un singleton avec une network Variable et quand les perso spawn ils previennent le singleton

    public void AssignCharacter(ulong id)
    {
        gameObject.SetActive(true);

        character = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).GetComponent<Character>();
        character.GetHealthVariable().OnValueChanged += DrawHP;
        maxHP = character.GetMaxHealth();
        textPlayerName.text = "Player " + (id + 1).ToString();

        NetworkManager.Singleton.OnClientConnectedCallback -= AssignCharacter;

        if(id == NetworkManager.Singleton.LocalClientId)
        {

        }
    }

    public void DrawHP(int oldHP, int newHP)
    {
        newHP = (int)Mathf.Clamp(newHP, 0, maxHP);
        healthBar.transform.localScale = new Vector3(newHP / maxHP, 1, 1);
    }
}
