using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAbility : MonoBehaviour
{
    public float freezeTime;
    public float cooldown;

    public float time, cooltime;
    bool can;
    GameObject player;
    Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }

    void Update()
    {
        skill.cooldown = cooltime;
        skill.maxCooldown = cooldown;

        time -= Time.deltaTime;

        if (time <= 0)
        {
            time = 0;
            Global.globalSpeed = 1;
        }
        else
        {
            Global.globalSpeed = 0;
        }

        if (cooltime <= 0)
        {
            cooltime = 0;
            can = true;
        }
        else
        {
            cooltime -= Time.deltaTime;
            can = false;
        }

        if (player != null)
        {
            if (skill.skillOn)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (can)
                    {
                        time = freezeTime;
                        cooltime = cooldown;
                    }
                }
            }
            else
            {
                Global.globalSpeed = 1;
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
