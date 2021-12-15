using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class AreaOfEffect : NetworkBehaviour
{
    [SerializeField]
    float time = 1;
    [SerializeField]
    int damage = 3;
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
        characters = new List<Character>();
    }

    // Add le player
    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponent<Character>();
        if(character)
            characters.Add(character);
    }

    // Quit le player
    private void OnTriggerExit(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character)
            characters.Remove(character);
    }

    // Update is called once per frame
    void Update()
    {  
        t += Time.deltaTime;
        if(t > time)
        {
            AttackMessage attackDmg = new AttackMessage();
            attackDmg.Damage = damage;
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].TakeDamage(attackDmg);
            }
            Destroy(this.gameObject);

            // Ces trucs peuvent être utilisés pour plus de précisions dans la syncro entre les clients et les serveurs
            // Je sais pas comment c'est implémenté par contre je doute que ça règle tout de manière magique
            //NetworkManager.LocalTime.Time;
            //NetworkManager.ServerTime.Time;
        }
    }
}
