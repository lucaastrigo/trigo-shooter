using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Skill : MonoBehaviour
{
    public bool activeSkill, hasCooldown;
    public bool skillOn, skilled, unskilled;
    [HideInInspector] public float cooldown;

    GameObject player;

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (hasCooldown && skillOn)
        {
            player.GetComponent<Player>().aatimer.GetComponent<TMP_Text>().text = cooldown.ToString("F1");
        }

        if (!hasCooldown && skillOn)
        {
            player.GetComponent<Player>().aatimer.GetComponent<TMP_Text>().text = null;
        }
    }
}
