using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniChest : MonoBehaviour
{
    public AudioClip clip;

    [Header("Chest Setting")]
    public GameObject[] weapons;

    [Header("Image Setting")]
    public Sprite openedSprite;

    bool open;
    public GameObject weaponToDrop;
    public GameObject secondWeaponToDrop;
    SpriteRenderer sprite;
    AudioSource aud;
    GameObject skillStorage;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        aud = GetComponent<AudioSource>();

        if (skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            if(ValueStorage.value.weaponValue == weapons[i].name || ValueStorage.value.weaponValue == weapons[i].name + "(Clone)")
            {
                weaponToDrop = weapons[i];
                weaponToDrop.GetComponent<Weapon>().LoadData();
            }

            if (ValueStorage.value.secondWeaponValue == weapons[i].name || ValueStorage.value.secondWeaponValue == weapons[i].name + "(Clone)")
            {
                if((ValueStorage.value.weaponValue == "PISTOL" || ValueStorage.value.weaponValue == "PISTOL(Clone)") && (ValueStorage.value.secondWeaponValue == "PISTOL" || ValueStorage.value.secondWeaponValue == "PISTOL(Clone)"))
                {
                    secondWeaponToDrop = null;
                }
                else
                {
                    secondWeaponToDrop = weapons[i];
                    secondWeaponToDrop.GetComponent<Weapon>().LoadData();
                }
            }
        }
    }

    private void Update()
    {
        if (skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }
    }

    void Drop()
    {
        if(secondWeaponToDrop != null)
        {
            weaponToDrop.GetComponent<SpriteRenderer>().sortingOrder = 6;
            secondWeaponToDrop.GetComponent<SpriteRenderer>().sortingOrder = 6;

            Instantiate(weaponToDrop, new Vector2(transform.position.x + 0.4f, transform.position.y + 0.5f), Quaternion.identity);
            Instantiate(secondWeaponToDrop, new Vector2(transform.position.x - 0.4f, transform.position.y + 0.5f), Quaternion.identity);
        }
        else
        {
            weaponToDrop.GetComponent<SpriteRenderer>().sortingOrder = 6;

            Instantiate(weaponToDrop, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
        }

        Instantiate(Resources.Load("Particle FX/WeaponChestFX"), transform.position, Quaternion.identity);
        aud.PlayOneShot(clip);
        open = true;
        sprite.sprite = openedSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open)
        {
            Drop();
        }
    }
}
