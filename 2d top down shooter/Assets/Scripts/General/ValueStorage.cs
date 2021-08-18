using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ValueStorage : MonoBehaviour
{
    public static ValueStorage value;

    public string weaponValue;
    public string secondWeaponValue;

    public int firstAmmo;
    public int secondAmmo;

    public int healthValue;
    public int maxHealthValue;

    public int coinValue;

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
        value.coinValue = 0;

        SceneManager.LoadScene("Lobby");
    }
}
