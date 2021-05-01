using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShieldAbility : MonoBehaviour
{
    public float cooldown;
    public float bubbleTime;

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
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    if (can)
                    {
                        StartCoroutine(Bubble());
                    }
                }
            }
            else
            {
                player.GetComponent<PlayerSkills>().bubbleShield.SetActive(false);
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    IEnumerator Bubble()
    {
        player.GetComponent<PlayerSkills>().bubbleShield.SetActive(true);
        cooltime = cooldown;
        yield return new WaitForSeconds(bubbleTime);
        player.GetComponent<PlayerSkills>().bubbleShield.SetActive(false);
    }
}
