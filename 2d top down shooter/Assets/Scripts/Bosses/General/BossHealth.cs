using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public GameObject healthBar;

    Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        healthBar.GetComponent<HealthBar>().SetHealth(enemy.health);
        healthBar.GetComponent<HealthBar>().SetMaxHealth(enemy.health);
    }

    void Update()
    {
        healthBar.GetComponent<HealthBar>().SetHealth(enemy.health);
    }
}
