using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public int healthValue;

    bool skilled, canTake;
    Player player;
    [HideInInspector] public Skill skill;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        skill = GetComponent<Skill>();
    }

    void Update()
    {
        if(skill.skillOn && !skilled)
        {
            Activate();
        }

        if (canTake && Input.GetKeyDown(KeyCode.E))
        {
            Activate();
            Instantiate(Resources.Load("Particle FX/HealthChestFX"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void Activate()
    {
        skilled = true;

        player.health += healthValue;
        player.healthBar.GetComponent<HealthBar>().SetHealth(player.currentHealth);
        player.healthBar.GetComponent<HealthBar>().SetMaxHealth(player.health);
        player.MoreHealth(healthValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canTake = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canTake = false;
        }
    }
}
