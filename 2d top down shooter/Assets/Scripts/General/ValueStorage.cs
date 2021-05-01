﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ValueStorage : MonoBehaviour
{
    public static ValueStorage value;

    public string weaponValue;
    public string secondWeaponValue;

    public int firstIndex;
    public int secondIndex;

    public int healthValue;
    public int maxHealthValue;

    public List<int> WeaponAmmo = new List<int>();

    private void Awake()
    {
        if(value == null)
        {
            value = this;
            DontDestroyOnLoad(gameObject);
        }else if(value != this)
        {
            Destroy(gameObject);
        }
    }

    public void RestoreAll()
    {
        value.healthValue = 10;
        value.maxHealthValue = 10;
        value.weaponValue = "PISTOL";
        value.secondWeaponValue = "PISTOL";

        for (int i = 0; i <= WeaponAmmo.Count - 1; i++)
        {
            value.WeaponAmmo[i] = 1000;
        }

        for (int i = WeaponAmmo.Count - 1; i > 0; i--)
        {
            value.WeaponAmmo.RemoveAt(i);
        }

        SceneManager.LoadScene("Lobby");
    }
}