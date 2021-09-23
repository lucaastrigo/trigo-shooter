using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float startingSpeed;
    public float maxSpeed;
    public float timeToStart;
    public bool delay;

    public float speed;
    bool canSpin;

    private void Start()
    {
        StartCoroutine(StartSpinning());
    }

    void FixedUpdate()
    {
        if (!delay)
        {
            transform.Rotate(0, 0, maxSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if(speed > maxSpeed)
            {
                speed -= 10 * Time.fixedDeltaTime;
            }

            transform.Rotate(0, 0, speed * Time.fixedDeltaTime);
        }
    }

    IEnumerator StartSpinning()
    {
        yield return new WaitForSeconds(timeToStart);
        canSpin = true;
        speed = startingSpeed;
    }
}
