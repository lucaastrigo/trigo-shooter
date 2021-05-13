using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // IF THE ROOM IS ACTIVE, CLOSE DOORS AND CREATE A BLACK PANEL AROUND 
    // PUBLIC BOOL TO SEE IF A ROOM WAS ALREADY ENTERED BY THE PLAYER
    
    public enum roomType
    {
        start, wave, boss, chest, special
    }

    public roomType thisRoom;

    [Header("Loot")]
    public bool lootable;
    public GameObject healthLoot, ammoLoot, coinLoot;

    [Header("Enemies")]
    public GameObject[] enemies;

    [Header("Map")]
    public GameObject weaponChest;
    public GameObject healthChest;
    public GameObject ammoChest;
    public GameObject skillChest;
    public GameObject[] door;
    public GameObject background;
    //public GameObject minimapImage;

    [HideInInspector] public int numOfWaves;
    public bool startedWave, finishedWave, inWave, hadWave;
    public bool opened;
    bool onTrigger;
    GameObject[] enemiesLeft;

    private void Start()
    {
        //
    }

    void Update()
    {
        //minimap only shows image when room is already OPENED 

        switch (thisRoom)
        {
            case roomType.start:

                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(false);
                }

                background.SetActive(false);

                break;

            case roomType.wave:

                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(onTrigger && inWave);
                }

                background.SetActive(onTrigger && inWave);

                break;

            case roomType.chest:

                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(false);
                }

                background.SetActive(false);

                break;

            case roomType.special:

                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(onTrigger && inWave);
                }

                background.SetActive(onTrigger && inWave);

                break;

            case roomType.boss:
                //
                break;
        }

        if (opened)
        {
            if (thisRoom == roomType.wave || thisRoom == roomType.special)
            {
                if (!startedWave)
                {
                    SetupWave();
                }
            }
        }

        enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemiesLeft.Length <= 0 && inWave && numOfWaves > 0)
        {
            StartWave();

        }else if(enemiesLeft.Length <= 0 && numOfWaves <= 0 && !finishedWave && startedWave && inWave)
        {
            FinishWave();
        }
    }

    void SetupWave()
    {
        numOfWaves = Random.Range(1, 4);
        startedWave = true;

        for (int i = 0; i < door.Length; i++)
        {
            door[i].SetActive(true);
        }

        background.SetActive(true);

        if (numOfWaves > 0)
        {
            StartWave();
            hadWave = true;
        }
        else
        {
            hadWave = false;
        }
    }

    public void StartWave()
    {
        inWave = true;
        numOfWaves--;
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
    }

    public void FinishWave()
    {
        inWave = false;
        finishedWave = true;

        if (hadWave)
        {
            if (lootable)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                GameObject weapon = player.GetComponentInChildren<Weapon>().gameObject;

                if (Random.Range(0, 11) >= player.GetComponent<Player>().currentHealth)
                {
                    Instantiate(healthLoot, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);

                }

                if (weapon != null)
                {
                    if (Random.Range(0, weapon.GetComponent<Weapon>().maxAmmo) >= weapon.GetComponent<Weapon>().currentAmmo)
                    {
                        Instantiate(ammoLoot, new Vector2(transform.position.x - 1, transform.position.y), Quaternion.identity);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            opened = true;

            if(thisRoom == roomType.wave || thisRoom == roomType.special)
            {
                if (!startedWave)
                {
                    SetupWave();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onTrigger = false;
        }
    }
}
