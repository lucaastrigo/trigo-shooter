using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodlustSkill : MonoBehaviour
{
    public float healthUpRate;

    [HideInInspector] public float _healthUpRate;
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

        _healthUpRate = healthUpRate;
    }
}
