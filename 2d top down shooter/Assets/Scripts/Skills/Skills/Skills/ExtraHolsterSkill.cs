using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHolsterSkill : MonoBehaviour
{
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }
}
