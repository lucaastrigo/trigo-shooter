using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousDamage : MonoBehaviour
{
    bool onTrigger;

    void Start()
    {
        //
    }

    void Update()
    {
        if (onTrigger)
        {
            //if on time, burn and reset time
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if enemy is here, assign GameObject enemy to him
        //this should look like explode method
        onTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if there is not a single enemy here, GameObject enemies is null
        onTrigger = false;
    }
}
