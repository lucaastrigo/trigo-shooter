using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcedBarrelSkill : MonoBehaviour
{
    [Range(0, 1)]
    public float damageIncreased;

    [HideInInspector] public float _damageIncreased;
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
        _damageIncreased = 1 + damageIncreased;
    }
}
