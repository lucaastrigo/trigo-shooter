using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public int healthValue;

    bool skilled;
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
    }

    void Activate()
    {
        skilled = true;

        player.health += healthValue;
        player.healthBar.GetComponent<HealthBar>().SetHealth(player.currentHealth);
        player.healthBar.GetComponent<HealthBar>().SetMaxHealth(player.health);
        player.MoreHealth(healthValue);
    }
}
