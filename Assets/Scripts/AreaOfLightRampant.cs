using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class AreaOfLightRampant : NetworkBehaviour
{
    // 1 = bleu
    // 2 = vert
    // 3 = rose
    [SerializeField]
    int lightRampantID = 1;


    bool active = false;
    Animator animator = null;

    // Quand l'objet spawn
    public override void OnNetworkSpawn()
    {
        // LEs AoE sont gérés par le serveur
        if (!IsServer)
        {      
            enabled = false;
            return;
        }
        animator = GetComponentInChildren<Animator>();
    }

    // Add le player
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;
        if (!active)
            return;

        Character character = other.GetComponent<Character>();
        if(character != null)
        {
            if(character.lightRampantID == lightRampantID)
                character.SetLightRampant(0);
        }

    }

    public void SetActive(bool b)
    {
        active = b;
        animator.SetBool("Appear", true);
    }

}
