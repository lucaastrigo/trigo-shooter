using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // IF THE ROOM IS ACTIVE, CLOSE DOORS AND CREATE A BLACK PANEL AROUND 
    // PUBLIC BOOL TO SEE IF A ROOM WAS ALREADY ENTERED BY THE PLAYER
    
    public enum roomType
    {
        start, wave, chest, special
    }

    public roomType thisRoom;

    [Header("Loot")]
    [Range(0, 100)] public int healthLootChance, ammoLootChance, coinLootChance;
    public bool lootable;
    public GameObject healthLoot, ammoLoot, coinChestLoot;

    [Header("Enemies")]
    public GameObject[] enemies;

    [Header("Map")]
    public int waveMax;
    public GameObject minimapImages;
    public GameObject weaponChest;
    public GameObject healthChest;
    public GameObject ammoChest;
    public GameObject skillChest;
    public GameObject[] door;
    public GameObject firstground, background;

    [HideInInspector] public int numOfWaves, _numOfWaves;
    [HideInInspector] public bool startedWave, finishedWave, inWave, hadWave;
    [HideInInspector] public bool opened;
    bool onTrigger;
    GameObject[] enemiesLeft;
    Camera minimapCamera;

    private void Start()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
        minimapImages.SetActive(false);
    }

    void Update()
    {
        firstground.SetActive(!opened);

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
        numOfWaves = Random.Range(1, waveMax);
        _numOfWaves = numOfWaves;
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
        Minimap.minimap.SetActive(false);
        Minimap.map.transform.GetChild(0).gameObject.SetActive(false);
        inWave = true;
        numOfWaves--;
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
    }

    public void FinishWave()
    {
        Minimap.minimap.SetActive(true);
        inWave = false;
        finishedWave = true;

        if (hadWave)
        {
            if (lootable)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (Random.Range(0, 100) <= healthLootChance)
                {
                    Instantiate(healthLoot, new Vector2(transform.position.x + 3, transform.position.y), Quaternion.identity);
                }

                if (Random.Range(0, 100) <= ammoLootChance)
                {
                    Instantiate(ammoLoot, new Vector2(transform.position.x - 3, transform.position.y), Quaternion.identity);
                }

                if (Random.Range(0, 100) <= coinLootChance)
                {
                    Instantiate(coinChestLoot, transform.position, Quaternion.identity);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            minimapCamera.transform.localPosition = transform.position;

            opened = true;

            minimapImages.SetActive(true);

            if (thisRoom == roomType.wave || thisRoom == roomType.special)
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
