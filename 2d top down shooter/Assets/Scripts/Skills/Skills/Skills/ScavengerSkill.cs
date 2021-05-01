using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerSkill : MonoBehaviour
{
    [Range(1.5f, 3)]
    public float dropRateIncrease;

    [HideInInspector] public float increase;
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }
}
