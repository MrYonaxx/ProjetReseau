using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class AreaOfEffect : NetworkBehaviour
{
    [SerializeField]
    float time = 1;
    float t = 0f;

    List<Character> characters;

    // Quand l'objet spawn
    public override void OnNetworkSpawn()
    {
        // LEs AoE sont gérés par le serveur
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    // Add le player
    private void OnTriggerEnter(Collider other)
    {

    }

    // Quit le player
    private void OnTriggerExit(Collider other)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t > time)
        {
            // Envois un RPC de dégats sur les players

            // Ces trucs peuvent être utilisés pour plus de précisions dans la syncro entre les clients et les serveurs
            // Je sais pas comment c'est implémenté par contre je doute que ça règle tout de manière magique
            //NetworkManager.LocalTime.Time;
            //NetworkManager.ServerTime.Time;
        }
    }
}
