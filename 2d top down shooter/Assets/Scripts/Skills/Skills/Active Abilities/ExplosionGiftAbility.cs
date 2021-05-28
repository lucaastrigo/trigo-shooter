using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGiftAbility : MonoBehaviour
{
    public float cooldown;
    public GameObject explosionGift;

    float cooltime;
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

        if(cooltime <= 0)
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
                        Instantiate(explosionGift, player.transform.position, Quaternion.identity);
                        cooltime = cooldown;
                    }
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
