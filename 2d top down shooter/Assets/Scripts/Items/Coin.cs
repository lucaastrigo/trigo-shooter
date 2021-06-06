using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinMin, coinMax;
    public Sprite openedSprite;

    bool opened;

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
                    opened = true;
                }
            }
        }
    }
}
