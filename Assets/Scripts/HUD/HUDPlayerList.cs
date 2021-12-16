using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

public class HUDPlayerList : MonoBehaviour
{

    [SerializeField]
    List<HUDPlayer> hudPlayers = new List<HUDPlayer>();

    int index = 0;
    bool firstTime = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTeam.Instance.OnPlayerAdded += AssignCharacter;
    }

    private void OnDestroy()
    {
        PlayerTeam.Instance.OnPlayerAdded -= AssignCharacter;
    }

    public void AssignCharacter(Character c, int id)
    {
        hudPlayers[index].AssignCharacter(c, id);
        index++;
    }

}
