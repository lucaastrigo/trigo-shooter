using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage;
    public float cooldown;
    public GameObject hitFX;

    float time;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        time = cooldown;
    }

    private void Update()
    {
        if (Global.globalSpeed > 0)
        {
            if (time <= 0)
            {
                anim.SetTrigger("attack");

                time = cooldown;
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Instantiate(hitFX, transform.position, Quaternion.identity);
        }
    }
}
