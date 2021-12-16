using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class AreaOfHeal : NetworkBehaviour
{
    [SerializeField]
    int damage = -2;
    [SerializeField]
    int timeRespawn = -2;
    [SerializeField]
    Transform[] positions;

    bool active = false;
    float t = 0f;
    Animator animator = null;
    Vector3 originScale;

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
        originScale = transform.localScale;
        t = timeRespawn;
    }

    // Add le player
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;
        if (t < timeRespawn)
            return;
        if (!active)
            return;

        Character character = other.GetComponent<Character>();
        if(character != null)
        {
            AttackMessage attackDmg = new AttackMessage();
            attackDmg.Damage = damage;

            character.TakeDamage(attackDmg);

            //transform.localScale = Vector3.zero;
            animator.SetBool("Appear", false);
            t = 0f;
        }

    }

    public void SetActive(bool b)
    {
        active = b;
        animator.SetBool("Appear", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;
        if (!active)
            return;

        if (t < timeRespawn)
        {
            t += Time.deltaTime;
            if(t>= timeRespawn)
            {
                this.transform.localScale = originScale;
                this.transform.position = positions[Random.Range(0, positions.Length)].position;
                animator.SetBool("Appear", true);
            }
        }
    }
}
