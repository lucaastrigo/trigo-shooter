using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public int damage;
    public float attackCooldown;

    float attackTime;
    bool canAttack;
    GameObject player;

    void Update()
    {
        if(player == null)
        {
            player = GameObject.Find("Player");
        }

        if (attackTime <= 0)
        {
            canAttack = true;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }

    void Attack()
    {
        player.GetComponent<Player>().TakeDamage(damage);
        attackTime = attackCooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canAttack)
            {
                Attack();
            }
        }
    }
}
