using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiderMuzzleSkill : MonoBehaviour
{
    [Range(0, 2)]
    public float sizeIncrease;

    [HideInInspector] public float _sizeIncrease;
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
        _sizeIncrease = 1 + sizeIncrease;
    }
}
