using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    public GameObject laserSight;

    void Update()
    {
        if (gameObject.GetComponent<Weapon>().onOff)
        {
            laserSight.SetActive(true);
        }
        else
        {
            laserSight.SetActive(false);
        }
    }
}
