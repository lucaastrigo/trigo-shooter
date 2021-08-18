using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcedBarrelSkill : MonoBehaviour
{
    [Range(0, 1)]
    public float damageIncreased;

    [HideInInspector] public float _damageIncreased;
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

        _damageIncreased = 1 + damageIncreased;
    }
}
