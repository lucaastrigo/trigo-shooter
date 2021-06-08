using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerAbility : MonoBehaviour
{
    GameObject player;
    Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }

    void Update()
    {
        if (player != null)
        {
            if (skill.skillOn)
            {
                player.GetComponent<PlayerSkills>().plunger.SetActive(true);
            }
            else
            {
                player.GetComponent<PlayerSkills>().plunger.SetActive(false);
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
