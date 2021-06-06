using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAmmo : MonoBehaviour
{
    [HideInInspector] public int ammoAmount;
    public bool destroyAfter;
    public Sprite opened;
    public GameObject openFX;

    [Range(0, 1)]
    public float ammo;

    bool open;
    GameObject player;
    GameObject child;
    Weapon weapon;
    SpriteRenderer sprite;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open && player.GetComponentInChildren<Weapon>() != null && weapon.GetComponent<Weapon>().currentAmmo < weapon.GetComponent<Weapon>().maxAmmo)
        {
            Instantiate(openFX, transform.position, Quaternion.identity);
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
