using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TwoStages : MonoBehaviour
{
    public float vulnerableTime;

    float time;
    bool vulnerablized, finalized;
    Enemy enemy;
    GameObject[] enemies;

    public UnityEvent vulnerabling, invulnerabling, finalize;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        print(enemies.Length);


        if(!enemy.dead)
        {
            if (enemies.Length == 1)
            {
                if (!vulnerablized)
                {
                    Vulnerablizing();
                }
            }

            if (time <= 0)
            {
                if (vulnerablized)
                {
                    Invulnerablizing();
                }
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
        else if(enemy.dead)
        {
            if (!finalized)
            {
                finalize.Invoke();
                finalized = true;
            }
        }
    }

    void Vulnerablizing() //toma dano
    {
        time = vulnerableTime;

        enemy.canBeHurt = true;
        vulnerabling.Invoke();
        vulnerablized = true;
    }

    void Invulnerablizing() //nao toma dano
    {
        enemy.canBeHurt = false;
        invulnerabling.Invoke();
        vulnerablized = false;
    }

    public void SpawnEnemies(GameObject enemies)
    {
        Instantiate(enemies, Vector3.zero, Quaternion.identity);
    }
}
