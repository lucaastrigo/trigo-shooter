using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposedSkill : MonoBehaviour
{
    [Range(0, 1)]
    public float healthDecrease;

    [HideInInspector] public float _healthDecrease;
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

        _healthDecrease = 1 - healthDecrease;
    }
}
