using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // IF THE ROOM IS ACTIVE, CLOSE DOORS AND CREATE A BLACK PANEL AROUND 
    // PUBLIC BOOL TO SEE IF A ROOM WAS ALREADY ENTERED BY THE PLAYER
    
    public enum roomType
    {
        start, wave, boss, chest
    }

    public roomType thisRoom;

    [Header("Player")]
    public GameObject player;

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
    [HideInInspector] public bool spawned;
    public bool opened;
    GameObject[] enemiesLeft;

    private void Start()
    {
        //
    }

    void Update()
    {
        enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemiesLeft.Length == 0)
        {
            if(numOfWaves > 0)
            {
                SpawnWave();
            }
            else
            {
                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(false);
                }

                background.SetActive(false);
            }
        }
        
        if(enemiesLeft.Length > 0)
        {
            for (int i = 0; i < door.Length; i++)
            {
                door[i].SetActive(true);
            }
        }

        if (opened)
        {
            //minimapImage.SetActive(true);
        }
        else
        {
            //minimapImage.SetActive(false);
        }
    }

    public void SpawnWave()
    {
        spawned = true;
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
        numOfWaves--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            background.SetActive(true);
        }
    }
}
