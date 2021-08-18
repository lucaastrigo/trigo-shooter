using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestHealth : MonoBehaviour
{
    public int healthKit;
    public float timeToDestroy;
    public bool timeDestroy;
    public bool destroyAfter;
    public Sprite opened;

    bool open;
    GameObject player;
    SpriteRenderer sprite;
    Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        Invoke("DestroyThisHeart", timeToDestroy);
    }

    void DestroyThisHeart()
    {
        if (anim != null)
        {
            anim.SetTrigger("vanish");
        }
    }

    public void DestroyThis()
    {
        Instantiate(Resources.Load("Particle FX/HealthChestFX"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open && player.GetComponent<Player>().currentHealth < player.GetComponent<Player>().health)
        {
            Instantiate(Resources.Load("Particle FX/HealthChestFX"), transform.position, Quaternion.identity);
            open = true;
            player.GetComponent<Player>().MoreHealth(healthKit);

            if (destroyAfter)
            {
                Destroy(gameObject);
            }
            else
            {
                sprite.sprite = opened;
            }
        }
    }
}
