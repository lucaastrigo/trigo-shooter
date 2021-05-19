using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerCurrency : MonoBehaviour
{
    [HideInInspector] public int coins;

    TextMeshProUGUI text;

    void Start()
    {
        coins = ValueStorage.value.coinValue;

        text = GameObject.Find("text money").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        ValueStorage.value.coinValue = coins;

        text.text = "$" + coins.ToString();
    }

    public void Purchase(int amount)
    {
        coins -= amount;
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
