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

    List<Transform> characters;

    // Quand l'objet spawn
    public override void OnNetworkSpawn()
    {
        // LEs AoE sont g�r�s par le serveur
        if (!IsServer)
        {      
            enabled = false;
            return;
        }
        characters = new List<Transform>();
    }

    // Add le player
    private void OnTriggerEnter(Collider other)
    {
        characters.Add(other.transform);
    }

    // Quit le player
    private void OnTriggerExit(Collider other)
    {
        characters.Remove(other.transform);
    }

    // Update is called once per frame
    void Update()
    {  
        t += Time.deltaTime;
        if(t > time)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].gameObject.SetActive(false);
            }
            Destroy(this.gameObject);
            // Envois un RPC de d�gats sur les players

            // Ces trucs peuvent �tre utilis�s pour plus de pr�cisions dans la syncro entre les clients et les serveurs
            // Je sais pas comment c'est impl�ment� par contre je doute que �a r�gle tout de mani�re magique
            //NetworkManager.LocalTime.Time;
            //NetworkManager.ServerTime.Time;
        }
    }
}
