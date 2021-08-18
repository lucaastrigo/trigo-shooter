using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoExpertSkill : MonoBehaviour
{
    public float ammoUpRate;

    [HideInInspector] public float _ammoUpRate;
    [HideInInspector] public Skill skill;
    GameObject player;

    private void Start()
    {
        skill = GetComponent<Skill>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (skill.skillOn && !skill.skilled)
        {
            ActivateSkill();
        }
    }

    void ActivateSkill()
    {
        skill.skilled = true;

        _ammoUpRate = ammoUpRate;
    }
}
