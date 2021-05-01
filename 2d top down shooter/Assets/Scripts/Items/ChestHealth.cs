using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestHealth : MonoBehaviour
{
    public int healthKit;
    public Sprite opened;
    public GameObject openFX;

    bool open;
    GameObject player;
    SpriteRenderer sprite;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open && player.GetComponent<Player>().currentHealth < player.GetComponent<Player>().health)
        {
            Instantiate(openFX, transform.position, Quaternion.identity);
            open = true;
            player.GetComponent<Player>().MoreHealth(healthKit);
            sprite.sprite = opened;
        }
    }
}
