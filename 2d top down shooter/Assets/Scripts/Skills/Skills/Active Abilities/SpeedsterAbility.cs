using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedsterAbility : MonoBehaviour
{
    public float newSpeed;

    float speedK = 3.5f;
    float time;
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
                if (!skill.skilled)
                {
                    speedK = player.GetComponent<PlayerMovement>().speed;
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
