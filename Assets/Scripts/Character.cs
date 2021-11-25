using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Character : NetworkBehaviour
{
    [SerializeField]
    float speed = 3;

    private void Update()
    {
        if(IsOwner || IsClient)
            this.transform.position += new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);
    }
}
