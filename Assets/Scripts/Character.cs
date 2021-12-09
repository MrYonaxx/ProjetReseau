using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Character : NetworkBehaviour
{

    [SerializeField]
    NetworkVariable<int> health;

    [Header("Stats")]
    [SerializeField]
    float speed = 3;

    [SerializeField]
    int damage = 3;
    [SerializeField]
    float attackRange = 3;
    [SerializeField]
    float globalCooldown = 3;

    [SerializeField]
    LayerMask layerMaskTargeting;

    [Header("UI")]
    [SerializeField]
    TextMeshPro textCharacterName = null;

    CharacterController characterController = null;

    ulong targetID = 0;
    ITargetable target;

    float timeAutoAttack = 0f;


    public event ActionTarget OnTargetSelected;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            CameraController.Instance.SetFocus(this.transform);
            characterController = GetComponent<CharacterController>();
        }
        else
        {
            enabled = false;
        }
        textCharacterName.text = "Player" + NetworkObjectId.ToString();
    }

    // Note de recherche :
    // Les clients n'ont pas le droit de modifier leur transform
    // Dans l'exemple boss room, quand un input est détecté, un RPC est envoyé au serveur qui ensuite déplace le personnage pour renvoyer l'info au client
    // Pour que les clients aient le droit de modifier le transform il faut un ClientNetworkTransform accesible jsp où (flemme de chercher, ils avaient qu'a l'inclure de base)
    // Le client Network Transform est sans doute moins sécuritaire 
    private void Update()
    {
        if(IsOwner)
        {
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            SelectTarget();
            AutoAttack();
        }
        textCharacterName.transform.LookAt(CameraController.Instance.Cam.transform);
    }

    private void Move(float x, float z)
    {
        Transform camera = CameraController.Instance.Cam.transform;
        Vector3 direction = camera.forward * z + camera.right * x;
        direction.Normalize();
        direction *= speed;
        direction *= Time.deltaTime;
        characterController.Move(new Vector3(direction.x, 0, direction.z));

        // Set Orientation
        if(x != 0 || z != 0)
            this.transform.rotation = Quaternion.Euler(0, camera.rotation.eulerAngles.y, 0);
    }


    private void SelectTarget()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;
            Ray ray = CameraController.Instance.Cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 20, layerMaskTargeting))
            {
                target = rayHit.collider.GetComponent<ITargetable>();
                if (target != null)
                    targetID = target.GetNetworkID();
                OnTargetSelected?.Invoke(target);
            }
        }
    }

    private void AutoAttack()
    {
        if (target == null)
            return;
        timeAutoAttack += Time.deltaTime;
        if (timeAutoAttack >= globalCooldown)
        {
            float distance = Vector3.Distance(transform.position, target.GetPos());
            if (distance < attackRange)
            {
                AttackMessage attackMessage = new AttackMessage();
                attackMessage.Damage = damage;

                // Puisque on peut appelé les Rpc présents uniquement dans le même script, j'envois l'info de qui je veux
                // taper à la version serveur de mon objet, ce dernier retrouve l'objet targetID et le tabasse
                AttackServerRpc(targetID, attackMessage, NetworkManager.LocalTime.Time);

                Debug.Log("Je tape");
                timeAutoAttack = 0;
            }
        }
    }


    [ServerRpc]
    public void AttackServerRpc(ulong id, AttackMessage attackMessage, double time)
    {
        if(targetID != id)
        {
            // normalement GetComponent peut jamais échouer (normalement)
            target = GetNetworkObject(id).GetComponent<ITargetable>();
        }
        target.TakeDamage(attackMessage, time);

    }

}
