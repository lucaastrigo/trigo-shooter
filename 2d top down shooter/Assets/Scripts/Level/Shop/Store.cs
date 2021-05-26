using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Store : MonoBehaviour
{
    public enum itemType
    {
        ability, ammo, health, skill, weapon
    }

    public itemType _itemType;

    public int price;
    public GameObject objectToSell, openFX;
    public TextMeshProUGUI priceText;
    public Transform objectPosition;
    public Sprite openedSprite;
    public Vector3 offset;

    bool triggered, bought;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        priceText.enabled = triggered && !bought;
        priceText.transform.position = Camera.main.WorldToScreenPoint(transform.position + offset);
        priceText.GetComponent<TMP_Text>().text = "$" + price;

        if (triggered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(player.GetComponent<PlayerCurrency>().coins >= price)
                {
                    if (!bought)
                    {
                        Buy();
                    }
                }
            }
        }
    }

    void Buy()
    {
        bought = true;
        player.GetComponent<PlayerCurrency>().Purchase(price);
        Instantiate(openFX, objectPosition.position, Quaternion.identity);
        transform.parent.GetComponent<SpriteRenderer>().sprite = openedSprite;

        if (_itemType == itemType.ammo || _itemType == itemType.health)
        {
            Instantiate(objectToSell, objectPosition.position, Quaternion.identity);
        }
        else if(_itemType == itemType.ability)
        {
            if(objectToSell.GetComponent<ChestSkill>() != null)
            {
                objectToSell.GetComponent<ChestSkill>().CalculateSkill();
                Instantiate(objectToSell.GetComponent<ChestSkill>().s1, objectPosition.position, Quaternion.identity);
            }
        }
        else if(_itemType == itemType.skill)
        {
            if (objectToSell.GetComponent<ChestSkill>() != null)
            {
                objectToSell.GetComponent<ChestSkill>().CalculateSkill();
                Instantiate(objectToSell.GetComponent<ChestSkill>().s1, objectPosition.position, Quaternion.identity);
            }
        }
        else if(_itemType == itemType.weapon)
        {
            if(objectToSell.GetComponent<ChestWeapon>() != null)
            {
                objectToSell.GetComponent<ChestWeapon>().CalculateDrop();
                Instantiate(objectToSell.GetComponent<ChestWeapon>().weaponToDrop, objectPosition.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = false;
        }
    }
}
