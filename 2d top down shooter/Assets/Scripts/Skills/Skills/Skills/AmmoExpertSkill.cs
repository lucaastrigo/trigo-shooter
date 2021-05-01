using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoExpertSkill : MonoBehaviour
{
    public float ammoUpRate;

    [HideInInspector] public float _ammoUpRate;
    [HideInInspector] public Skill skill;
    GameObject player;

    private void Start()
    {
        skill = GetComponent<Skill>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (skill.skillOn)
        {
            _ammoUpRate = ammoUpRate;
        }
        else
        {
            _ammoUpRate = 0;
        }
    }
}
