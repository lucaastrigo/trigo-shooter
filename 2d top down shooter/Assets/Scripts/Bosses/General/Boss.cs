using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject stairs;

    void Start()
    {
        healthBar.GetComponent<HealthBar>().SetMaxHealth(GetComponent<Enemy>().health);
    }

    void Update()
    {
        healthBar.GetComponent<HealthBar>().SetHealth(GetComponent<Enemy>().health);

        GameObject[] enemyBullet = GameObject.FindGameObjectsWithTag("Enemy Bullet");
        for (int i = 0; i < enemyBullet.Length; i++)
        {
            Physics2D.IgnoreCollision(enemyBullet[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}
