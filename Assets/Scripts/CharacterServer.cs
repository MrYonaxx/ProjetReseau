using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

// Note : Y'a autant de lag dans les déplacements avec le version server RPC que la version client network transform
public class CharacterServer : NetworkBehaviour
{
    [SerializeField]
    NetworkVariable<int> characterID;

    [SerializeField]
    float speed = 3;

    [Header("UI")]
    [SerializeField]
    TextMeshPro textCharacterName = null;

    CharacterController characterController = null;

    private void Awake()
    {
        if (IsOwner)
        {
            CameraController.Instance.SetFocus(this.transform);
        }
        characterController = GetComponent<CharacterController>();
        textCharacterName.text = "Player" + NetworkObjectId.ToString();
    }

    float speedX = 0;
    float speedZ = 0;

    // VERSION SERVER
    private void Update()
    {
        if (IsServer)
        {
            Debug.Log("Allé");
            characterController.Move(new Vector3(speedX, 0, speedZ));
        }

        if (IsClient)
        {
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    private void Move(float x, float z)
    {
        Transform camera = CameraController.Instance.Cam.transform;
        Vector3 direction = camera.forward * z + camera.right * x;
        direction.Normalize();
        direction *= speed;
        direction *= Time.deltaTime;
        MoveServerRpc(direction.x, direction.z);
    }

    // D'après le prof les RPC c'est le mal, et ça a l'air de l'être encore plus quand c'est pour des déplacements de base
    [ServerRpc]
    public void MoveServerRpc(float newSpeedX, float newSpeedZ)
    {
        speedX = newSpeedX;
        speedZ = newSpeedZ;
    }
}
