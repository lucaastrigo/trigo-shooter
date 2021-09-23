using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TwoStages : MonoBehaviour
{
    public float vulnerableTime, delay;
    public GameObject hitlessReward;
    public GameObject[] enemyWaves;

    int playerInitialHealth;
    float time, delayTime;
    bool vulnerablized;
    bool finalized;
    Enemy enemy;
    Player player;
    GameObject[] enemies;
    GameObject rays;

    public UnityEvent vulnerabling, invulnerabling, finalize;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        playerInitialHealth = player.currentHealth;
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if(!enemy.dead)
        {
            if(delayTime >= delay)
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
            else
            {
                delayTime += Time.deltaTime;
            }
        }
        else if(enemy.dead)
        {
            if (!finalized)
            {
                if (!hitDetect())
                {
                    Hitless();
                }

                finalize.Invoke();
                finalized = true;
            }
        }
    }

    bool hitDetect()
    {
        if(player.currentHealth < playerInitialHealth)
        {
            return true;
        }
        else
        {
            return false;
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

    public void SpawnStairs(GameObject stairs)
    {
        Instantiate(stairs, new Vector3(transform.position.x, transform.position.y + 3, 0), Quaternion.identity);
    }

    public void SpawnEnemies()
    {
        int i = Random.Range(0, enemyWaves.Length - 1);
        Instantiate(enemyWaves[i], Vector3.zero, Quaternion.identity);
    }

    public void ActivateRays(GameObject boneRays)
    {
        GameObject raysToDestroy = Instantiate(boneRays, transform.position, Quaternion.identity);
        rays = raysToDestroy;
    }

    public void DeactivateRays()
    {
        Destroy(rays);
    }

    void Hitless()
    {
        Instantiate(hitlessReward, new Vector3(transform.position.x, transform.position.y - 3, 0), Quaternion.identity);
    }
}
