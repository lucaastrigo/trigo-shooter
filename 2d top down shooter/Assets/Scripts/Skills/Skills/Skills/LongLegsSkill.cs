using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongLegsSkill : MonoBehaviour
{
    float newSpeed;
    float speedK;
    GameObject player;

    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
        player = GameObject.FindGameObjectWithTag("Player");
        speedK = player.GetComponent<PlayerMovement>().speed;
        newSpeed = speedK * 1.5f;
    }

    /*
    private void Update()
    {
        if(player != null)
        {
            if (skill.skillOn)
            {
                player.GetComponent<PlayerMovement>().speed = newSpeed;
            }
            else
            {
                //player.GetComponent<PlayerMovement>().speed = speedK;
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
    */
}
