using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;

public delegate void ActionAttack(AttackData attack);

[System.Serializable]
public class AttackData
{
    public int ID = 0;
    public AnimationClip Attack;
    public float AnimationDamageTime = 5; // en frames
    public float Cooldown = 5;
    public int Damage = 5;

    float t = 0;
    bool canUse = true;


    public void UpdateCooldown()
    {
        if(!canUse)
        {
            t += Time.deltaTime;
            if (t > Cooldown)
                canUse = true;
        }

    }

    public void Use()
    {
        canUse = false;
        t = 0;
    }
}
public class Character : NetworkBehaviour, ITargetable
{
    [SerializeField]
    int maxHealth = 3;
    [SerializeField]
    NetworkVariable<int> health;

    [Header("Stats")]
    [SerializeField]
    float speed = 3;
    [SerializeField]
    float globalCooldown = 2;

    [SerializeField]
    LayerMask layerMaskTargeting;

    [Header("Auto Attacks")]
    [SerializeField]
    int damage = 3;
    [SerializeField]
    float attackRange = 3.5f;
    [SerializeField]
    float attackCooldown = 3;
    [SerializeField] // en frames
    float animationTime = 25;

    [SerializeField]
    List<AttackData> skillsDatas;

    [Header("UI")]
    [SerializeField]
    TextMeshPro textCharacterName = null;

    Animator animator = null;
    CharacterController characterController = null;

    ulong targetID = 0;
    ITargetable target;

    float timeAutoAttack = 3f;
    float timeGlobalCooldown = 3f;


    public event ActionTarget OnTargetSelected;
    public event ActionAttack OnSkillUsed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            CameraController.Instance.SetFocus(this.transform);
            characterController = GetComponent<CharacterController>();
            animationTime /= 30f;

        }
        animator = GetComponentInChildren<Animator>();
        health = new NetworkVariable<int>(maxHealth);
        textCharacterName.text = "Player " + (OwnerClientId+1).ToString();
    }

    // Note de recherche :
    // Les clients n'ont pas le droit de modifier leur transform
    // Dans l'exemple boss room, quand un input est détecté, un RPC est envoyé au serveur qui ensuite déplace le personnage pour renvoyer l'info au client
    // Pour que les clients aient le droit de modifier le transform il faut un ClientNetworkTransform
    private void Update()
    {
        if (IsOwner)
        {
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            SelectTarget();
            AutoAttack();
            UpdateSkills();
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

        animator.SetFloat("DirectionX", x);
        animator.SetFloat("DirectionZ", z);

        // Set Orientation
        if (x != 0 || z != 0)
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
        if (timeAutoAttack >= attackCooldown)
        {
            float distance = Vector3.Distance(transform.position, target.GetPos());
            if (distance < attackRange)
            {
                AttackMessage attackMessage = new AttackMessage();
                attackMessage.Damage = damage;

                // Puisque on peut appelé les Rpc présents uniquement dans le même script, j'envois l'info de qui je veux
                // taper à la version serveur de mon objet, ce dernier retrouve l'objet targetID et le tabasse
                AttackServerRpc(targetID, attackMessage, NetworkManager.LocalTime.Time + animationTime);

                timeAutoAttack = 0;
                animator.SetTrigger("Attack1");
            }
        }
    }

    private void UpdateSkills()
    {
        for (int i = 0; i < skillsDatas.Count; i++)
        {
            skillsDatas[i].UpdateCooldown();
        }

        if (target == null)
            return;

        timeGlobalCooldown += Time.deltaTime;
        if (timeGlobalCooldown >= globalCooldown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivateSkills(skillsDatas[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivateSkills(skillsDatas[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivateSkills(skillsDatas[2]);
            }
        }
    }

    private void ActivateSkills(AttackData attack)
    {
        float distance = Vector3.Distance(transform.position, target.GetPos());
        if (distance < attackRange)
        {
            AttackMessage attackMessage = new AttackMessage();
            attackMessage.Damage = attack.Damage;
            attack.Use();

            AttackServerRpc(targetID, attackMessage, NetworkManager.LocalTime.Time + (attack.AnimationDamageTime / 30f));

            timeAutoAttack = 0;
            timeGlobalCooldown = 0;
            animator.Play(attack.Attack.name);

            OnSkillUsed.Invoke(attack);
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

        // On tente de synchroniser l'animation d'attaque et le damage
        float timeToWait = (float)(time - NetworkManager.ServerTime.Time);
        StartCoroutine(WaitAttackRPC(attackMessage, timeToWait));
    }

    private IEnumerator WaitAttackRPC(AttackMessage attackMessage, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        target.TakeDamage(attackMessage);
    }



    [ClientRpc]
    public void AnimationDamageClientRpc()
    {
        animator.SetTrigger("Damage");
        timeAutoAttack = 0;
    }


    // Partie ITargetable
    // Appelé par le serveur généralement
    public void TakeDamage(AttackMessage attackMessage)
    {
        health.Value = health.Value - attackMessage.Damage;

        if(attackMessage.Damage > 0)
            AnimationDamageClientRpc();
    }

    public Vector3 GetPos()
    {
        return this.transform.position;
    }

    public ulong GetNetworkID()
    {
        return NetworkObjectId;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public NetworkVariable<int> GetHealthVariable()
    {
        return health;
    }

}
