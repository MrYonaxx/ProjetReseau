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
        // LEs AoE sont g�r�s par le serveur
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
            // Envois un RPC de d�gats sur les players

            // Ces trucs peuvent �tre utilis�s pour plus de pr�cisions dans la syncro entre les clients et les serveurs
            // Je sais pas comment c'est impl�ment� par contre je doute que �a r�gle tout de mani�re magique
            //NetworkManager.LocalTime.Time;
            //NetworkManager.ServerTime.Time;
        }
    }
}
