using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

public class HUDTarget : MonoBehaviour
{

    [SerializeField]
    GameObject panelTarget = null;
    [SerializeField]
    Transform healthBar = null;
    [SerializeField]
    TextMeshProUGUI textHealthPercentage = null;

    float maxHP = 0;
    Character c = null;
    ITargetable previousTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AssignCharacter;
    }

    void OnDestroy()
    {
        if (c != null)
            c.OnTargetSelected -= DrawTarget;
        if (previousTarget != null)
            previousTarget.GetHealthVariable().OnValueChanged -= DrawHP;
    }




    public void AssignCharacter(ulong id)
    {
        if (id == NetworkManager.Singleton.LocalClientId)
        {
            c = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Character>();
            c.OnTargetSelected += DrawTarget;
            NetworkManager.Singleton.OnClientConnectedCallback -= AssignCharacter;
        }
    }

    public void DrawTarget(ITargetable targetable)
    {
        // unsubscribe le précédent target
        if(previousTarget != null)
        {
            previousTarget.GetHealthVariable().OnValueChanged -= DrawHP;
        }

        if (targetable == null)
        {
            panelTarget.gameObject.SetActive(false);
            return;
        }
        panelTarget.gameObject.SetActive(true);


        // Initialise la nouvelle target
        maxHP = targetable.GetMaxHealth();
        targetable.GetHealthVariable().OnValueChanged += DrawHP;
        DrawHP(targetable.GetHealthVariable().Value, targetable.GetHealthVariable().Value);

        previousTarget = targetable;
    }

    public void DrawHP(int oldHP, int newHP)
    {
        healthBar.transform.localScale = new Vector3(newHP / maxHP, 1, 1);
        textHealthPercentage.text = ((newHP / maxHP) * 100).ToString();
    }
}
