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
        NetworkManager.Singleton.OnClientConnectedCallback += AssignCharacter;
    }

    private void OnDestroy()
    {
        //NetworkManager.Singleton.OnClientConnectedCallback -= AssignCharacter;
    }



    public void AssignCharacter(ulong id)
    {
        hudPlayers[index].AssignCharacter(id);

        if(index == 0) // On check tout les client déjà existants
        {
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count-1; i++)
            {
                index++;
                hudPlayers[index].AssignCharacter((ulong)i);
            }
        }
        index++;
    }

}
