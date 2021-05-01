using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpshooterSkill : MonoBehaviour
{
    [Range(0, 0.1f)]
    public float accuracyIncrease;

    float increase;
    bool skilled; //not using the -Skill script- 'skilled' variable because i need it to NOT restart when the player dies 
    GameObject player;
    [HideInInspector] public GameObject weapon;
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }

    void Update()
    {
        if (player != null)
        {
            if (weapon != null)
            {
                if (skill.skillOn)
                {
                    if (!skilled)
                    {
                        Activate();
                    }
                }
                else
                {
                    if (!skill.unskilled)
                    {
                        Deactivate();
                    }
                }
            }
            else
            {
                if (player.GetComponentInChildren<Weapon>() != null)
                {
                    weapon = player.GetComponentInChildren<Weapon>().gameObject;
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void Activate()
    {
        if (weapon != null)
        {
            increase = weapon.GetComponent<Weapon>()._accuracy * accuracyIncrease;
            weapon.GetComponent<Weapon>().accuracy += increase;
            skilled = true;
        }
    }

    public void Deactivate()
    {
        if (weapon != null)
        {
            increase = 0;
            weapon.GetComponent<Weapon>().accuracy = weapon.GetComponent<Weapon>()._accuracy;
            skill.unskilled = true;
        }
    }
}
