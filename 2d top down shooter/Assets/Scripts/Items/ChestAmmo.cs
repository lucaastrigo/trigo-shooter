using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAmmo : MonoBehaviour
{
    [HideInInspector] public int ammoAmount;
    public float timeToDestroy;
    public bool timeDestroy;
    public bool destroyAfter;
    public Sprite opened;

    [Range(0, 1)]
    public float ammo;

    bool open;
    GameObject player;
    Weapon weapon;
    SpriteRenderer sprite;
    Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        Invoke("DestroyThisAmmo", timeToDestroy);
    }

    private void Update()
    {
        if(player.GetComponent<PlayerPickup>().equippedWeapon != null)
        {
            weapon = player.GetComponent<PlayerPickup>().equippedWeapon;
            float _ammo = weapon.maxAmmo * ammo;
            ammoAmount = Mathf.RoundToInt(_ammo);
        }
    }

    void DestroyThisAmmo()
    {
        if (anim != null)
        {
            anim.SetTrigger("vanish");
        }
    }

    public void DestroyThis()
    {
        Instantiate(Resources.Load("Particle FX/AmmoChestFX"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open && player.GetComponentInChildren<Weapon>() != null && weapon.GetComponent<Weapon>().currentAmmo < weapon.GetComponent<Weapon>().maxAmmo)
        {
            Instantiate(Resources.Load("Particle FX/AmmoChestFX"), transform.position, Quaternion.identity);
            open = true;
            weapon.GetComponent<Weapon>().MoreAmmo(ammoAmount);

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
