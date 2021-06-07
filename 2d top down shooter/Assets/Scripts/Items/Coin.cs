using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinMin, coinMax;
    public Sprite openedSprite;
    public GameObject openFX, minimapImage;

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
                    Instantiate(openFX, transform.position, Quaternion.identity);

                    if (minimapImage != null)
                    {
                        Destroy(minimapImage);
                    }

                    opened = true;
                }
            }
        }
    }
}
