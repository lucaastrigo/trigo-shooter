using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinMin, coinMax;
    public float timeToDestroy;
    public bool timeDestroy, destroyAfter;
    public Sprite openedSprite;

    bool opened;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        Invoke("DestroyThisCoin", timeToDestroy);
    }

    void DestroyThisCoin()
    {
        if(anim != null)
        {
            anim.SetTrigger("vanish");
        }
    }

    public void DestroyThis()
    {
        Instantiate(Resources.Load("Particle FX/CoinChestFX"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.GetComponent<Player>() != null)
            {
                if (!opened)
                {
                    collision.gameObject.GetComponent<Player>().Receive(Random.Range(coinMin, coinMax));
                    GetComponent<SpriteRenderer>().sprite = openedSprite;
                    Instantiate(Resources.Load("Particle FX/CoinChestFX"), transform.position, Quaternion.identity);

                    opened = true;

                    if (destroyAfter)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().sprite = openedSprite;
                    }
                }
            }
        }
    }
}
