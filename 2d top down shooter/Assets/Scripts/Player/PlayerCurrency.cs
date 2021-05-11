using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerCurrency : MonoBehaviour
{
    public int coins;

    TextMeshProUGUI text;

    void Start()
    {
        coins = ValueStorage.value.coinValue;

        text = GameObject.Find("text money").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        ValueStorage.value.coinValue = coins;

        text.text = coins.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            coins++;
            //play a sound fx
            Destroy(collision.gameObject);
        }
    }
}
