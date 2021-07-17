using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhost : MonoBehaviour
{
    public float pursueRange;

    Animator anim;
    GameObject player;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        anim.SetBool("pursue", true);
    }

    void Update()
    {
        if(GetComponent<MovementEnemy>() != null && GetComponent<MovementEnemy>().distToPlayer <= pursueRange && GetComponent<MovementEnemy>().canFollow())
        {
            anim.SetBool("pursue", true);
        }
        else
        {
            anim.SetBool("pursue", false);
        }
    }
}
