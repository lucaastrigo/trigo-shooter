using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBulletsSkill : MonoBehaviour
{
    [HideInInspector] public Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }
}
