using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Sprite newSprite;
    public GameObject FX;
    public Collider2D trigger;
    public Collider2D collider;

    bool destroyed;
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void DestroyProp()
    {
        Instantiate(FX, transform.position, Quaternion.identity);
        sprite.sprite = newSprite;
        trigger.enabled = false;
        destroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (!destroyed)
            {
                DestroyProp();
            }
        }
    }
}
