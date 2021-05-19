using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Store : MonoBehaviour
{
    public int price;
    public GameObject objectToSell;
    public TextMeshProUGUI priceText;
    public Transform objectPosition;

    bool triggered, bought;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        priceText.enabled = triggered && !bought;
        priceText.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
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
        Instantiate(objectToSell, objectPosition.position, Quaternion.identity);
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
