using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Skill : MonoBehaviour
{
    public bool activeSkill, hasCooldown;
    public bool skillOn, skilled, unskilled;
    [HideInInspector] public float cooldown, maxCooldown;

    GameObject player;

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (hasCooldown && skillOn && activeSkill)
        {
            float percent = cooldown / maxCooldown;
            player.GetComponent<Player>().aaplace[1].GetComponent<Image>().fillAmount = percent;
        }
    }
}
