using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStorage : MonoBehaviour
{
    public static WeaponStorage value;

    public GameObject[] weapons;

    private void Awake()
    {
        if (value == null)
        {
            value = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (value != this)
        {
            Destroy(gameObject);
        }
    }
}
