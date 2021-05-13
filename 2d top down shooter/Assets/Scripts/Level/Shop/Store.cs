using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Store : MonoBehaviour
{
    public int price;
    public GameObject objectToSell;
    public TextMeshProUGUI text;

    bool triggered;

    void Start()
    {
        //
    }

    void Update()
    {
        if (triggered)
        {
            if (Input.GetKey(KeyCode.E))
            {
                Buy();
            }
        }
    }

    void Buy()
    {
        //
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
