using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthierSkill : MonoBehaviour
{
    public int newHealth;
    public GameObject bigHealthChest;

    int healthK;
    GameObject player;
    Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
        player = GameObject.FindGameObjectWithTag("Player");
        healthK = player.GetComponent<Player>().health;
    }

    void Update()
    {
        if (player != null)
        {
            if (skill.skillOn)
            {
                player.GetComponent<Player>().health = newHealth;

                player.GetComponent<Player>().healthBar.GetComponent<HealthBar>().SetMaxHealth(newHealth);

                if (!skill.skilled)
                {
                    Instantiate(bigHealthChest, new Vector2(player.transform.position.x + 1, player.transform.position.y), Quaternion.identity);
                    skill.skilled = true;
                }

                player.GetComponent<Player>().healthBar.GetComponent<HealthBar>().SetHealth(player.GetComponent<Player>().currentHealth);
            }
            else
            {
                if (!skill.unskilled)
                {
                    player.GetComponent<Player>().health = healthK;

                    player.GetComponent<Player>().healthBar.GetComponent<HealthBar>().SetMaxHealth(healthK);

                    player.GetComponent<Player>().healthBar.GetComponent<HealthBar>().SetHealth(player.GetComponent<Player>().currentHealth);

                    skill.unskilled = true;
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
