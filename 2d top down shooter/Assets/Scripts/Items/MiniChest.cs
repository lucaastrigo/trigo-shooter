﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniChest : MonoBehaviour
{
    [Header("Chest Setting")]
    public GameObject[] weapons;
    public GameObject openFX;

    [Header("Image Setting")]
    public Sprite openedSprite;

    bool open;
    GameObject weaponToDrop;
    GameObject secondWeaponToDrop;
    SpriteRenderer sprite;

    GameObject skillStorage;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < weapons.Length; i++)
        {
            if(ValueStorage.value.weaponValue == weapons[i].name)
            {
                weaponToDrop = weapons[i];
                weaponToDrop.GetComponent<Weapon>().weaponIndex = ValueStorage.value.firstIndex;
            }
            
            if(ValueStorage.value.secondWeaponValue == weapons[i].name)
            {
                secondWeaponToDrop = weapons[i];
                secondWeaponToDrop.GetComponent<Weapon>().weaponIndex = ValueStorage.value.secondIndex;
            }
        }
    }

    private void Update()
    {
        if (skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        if (!skillStorage.GetComponentInChildren<ExtraHolsterSkill>().skill.GetComponent<Skill>().skillOn)
        {
            secondWeaponToDrop = null;
        }
    }

    void Drop()
    {
        if(secondWeaponToDrop != null)
        {
            Instantiate(weaponToDrop, new Vector2(transform.position.x + 0.4f, transform.position.y + 0.1f), Quaternion.identity);
            Instantiate(secondWeaponToDrop, new Vector2(transform.position.x - 0.4f, transform.position.y + 0.1f), Quaternion.identity);
        }
        else
        {
            Instantiate(weaponToDrop, new Vector2(transform.position.x, transform.position.y + 0.1f), Quaternion.identity);
        }

        Instantiate(openFX, transform.position, Quaternion.identity);
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