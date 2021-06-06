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

    public int priceMin, priceMax;
    public GameObject objectToSell, openFX;
    public TextMeshProUGUI priceText;
    public Transform objectPosition;
    public Sprite openedSprite;
    public Vector3 offset;

    int price;
    bool triggered, bought;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponentInChildren<Animator>().speed = 0;
        price = Random.Range(priceMin, priceMax);
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
                if(player.GetComponent<Player>().coins >= price)
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
        GetComponentInChildren<Animator>().speed = 1;

        bought = true;
        player.GetComponent<Player>().Purchase(price);
        Instantiate(openFX, objectPosition.GetChild(0).position, Quaternion.identity);
        transform.parent.GetComponent<SpriteRenderer>().sprite = openedSprite;

        if (_itemType == itemType.ammo || _itemType == itemType.health)
        {
            GameObject child = Instantiate(objectToSell, objectPosition.GetChild(0).position, Quaternion.identity);
            child.transform.parent = objectPosition.GetChild(0);
        }
        else if(_itemType == itemType.ability)
        {
            if(objectToSell.GetComponent<ChestSkill>() != null)
            {
                objectToSell.GetComponent<ChestSkill>().CalculateSkill();
                objectToSell.GetComponent<SpriteRenderer>().sortingOrder = 1;
                GameObject child = Instantiate(objectToSell.GetComponent<ChestSkill>().s1, objectPosition.GetChild(0).position, Quaternion.identity);
                child.transform.parent = objectPosition.GetChild(0);
            }
        }
        else if(_itemType == itemType.skill)
        {
            if (objectToSell.GetComponent<ChestSkill>() != null)
            {
                objectToSell.GetComponent<ChestSkill>().CalculateSkill();
                objectToSell.GetComponent<SpriteRenderer>().sortingOrder = 1;
                GameObject child = Instantiate(objectToSell.GetComponent<ChestSkill>().s1, objectPosition.GetChild(0).position, Quaternion.identity);
                child.transform.parent = objectPosition.GetChild(0);
            }
        }
        else if(_itemType == itemType.weapon)
        {
            if(objectToSell.GetComponent<ChestWeapon>() != null)
            {
                objectToSell.GetComponent<ChestWeapon>().CalculateDrop();
                objectToSell.GetComponent<SpriteRenderer>().sortingOrder = 1;
                GameObject child = Instantiate(objectToSell.GetComponent<ChestWeapon>().weaponToDrop, objectPosition.GetChild(0).position, Quaternion.identity);
                child.transform.parent = objectPosition.GetChild(0);
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
