using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedsterAbility : MonoBehaviour
{
    float newSpeed;
    float speedK;
    float time;
    GameObject player;
    Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
        player = GameObject.FindGameObjectWithTag("Player");
        speedK = player.GetComponent<PlayerMovement>().speed;
        newSpeed = speedK * 2;
    }

    void Update()
    {
        if (player != null)
        {
            if (skill.skillOn)
            {
                if (!skill.skilled)
                {
                    skill.skilled = true;
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    player.GetComponent<PlayerMovement>().speed = newSpeed;

                    if (player.GetComponent<PlayerMovement>().moving)
                    {
                        if (time <= 0)
                        {
                            Instantiate(player.GetComponent<PlayerMovement>().sprintFX, player.transform.position, Quaternion.identity);
                            time = 0.15f;
                        }
                        else
                        {
                            time -= Time.deltaTime;
                        }
                    }
                }
                else
                {
                    player.GetComponent<PlayerMovement>().speed = speedK;
                }
            }
            else
            {
                player.GetComponent<PlayerMovement>().speed = speedK;
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
