using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Character : NetworkBehaviour
{
    [SerializeField]
    float speed = 3;
    [SerializeField]
    Transform t;

    private void Awake()
    {
        //if (IsOwner)
          //  GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
    }

    // Note de recherche :
    // Les clients n'ont pas le droit de modifier leur transform
    // Dans l'exemple boss room, quand un input est détecté, un RPC est envoyé au serveur qui ensuite déplace le personnage pour renvoyer l'info au client
    // Pour que les clients aient le droit de modifier le transform il faut un ClientNetworkTransform accesible jsp où (flemme de chercher, ils avaient qu'a l'inclure de base)

    private void Update()
    {
        if(IsOwner)
        {
            Debug.Log("Ouais c'est bien");
            this.transform.position += new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);
        }
        else
        {
            Debug.Log("Bah non en fait");
        }

    }

    // D'après le prof les RPC c'est le mal, et ça a l'air de l'être encore plus quand c'est pour des déplacements de base
    [ServerRpc]
    public void MoveServerRpc()
    {
        
    }
}
