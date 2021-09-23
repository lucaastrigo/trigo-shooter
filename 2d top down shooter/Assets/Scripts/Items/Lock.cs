using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    bool inTrigger;

    private void Update()
    {
        if(inTrigger && Input.GetKeyDown(KeyCode.E) && Player.hasKey)
        {
            if (transform.parent != null)
            {
                Player.hasKey = false;
                transform.parent.gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }
}
