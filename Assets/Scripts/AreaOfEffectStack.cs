using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class AreaOfEffectStack : NetworkBehaviour
{
    [SerializeField]
    float time = 1;
    [SerializeField]
    float damage = 3;
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
        if (!IsServer)
            return;
        Character character = other.GetComponent<Character>();
        if(character != null)
            characters.Add(character);
    }

    // Quit le player
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer)
            return;
        Character character = other.GetComponent<Character>();
        if (character != null)
            characters.Remove(character);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;

        t += Time.deltaTime;
        if(t > time)
        {
            AttackMessage attackDmg = new AttackMessage();
            attackDmg.Damage = (int)(damage / characters.Count);
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].TakeDamage(attackDmg);
            }
            Destroy(this.gameObject);
        }
    }
}
