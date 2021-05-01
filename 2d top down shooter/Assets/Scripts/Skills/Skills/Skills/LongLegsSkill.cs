using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongLegsSkill : MonoBehaviour
{
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }
}
