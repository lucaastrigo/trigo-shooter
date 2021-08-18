using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixthSenseSkill : MonoBehaviour
{
    [Range(0, 1)]
    public float speedDecrease;

    [HideInInspector] public float _speedDecrease;
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
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
        _speedDecrease = 1 - speedDecrease;
    }
}
