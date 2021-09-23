using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinMin, coinMax;
    public float speed, flybackDistance, timeToDestroy;
    public bool timeDestroy, destroyAfter;
    public Sprite openedSprite;
    public AudioClip clip;

    bool opened;
    AudioSource aud;
    Animator anim;
    GameObject player;
    SpriteRenderer sprite;
    Collider2D coll;
    Vector3 velRef = Vector3.zero;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();

        if (timeDestroy)
        {
            Invoke("DestroyThisCoin", timeToDestroy);
        }
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, player.transform.position) <= flybackDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
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
                    aud.PlayOneShot(clip);
                    sprite.enabled = false;
                    coll.enabled = false;

                    opened = true;

                    if (destroyAfter)
                    {
                        Destroy(gameObject, 2);
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
