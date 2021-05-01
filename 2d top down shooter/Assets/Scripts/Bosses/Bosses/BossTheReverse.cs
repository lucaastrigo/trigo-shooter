using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTheReverse : MonoBehaviour
{
    public float invulnerableTime;
    public GameObject weapon;

    float vTime;
    float angle;
    float distanceToPlayer;
    GameObject player;
    Boss boss;

    private void Start()
    {
        boss = GetComponent<Boss>();

        vTime = invulnerableTime;
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!GetComponent<Boss>().dead)
        {
            if (GetComponent<Boss>().secondStage)
            {
                if (weapon.GetComponent<WeaponEnemy>() != null)
                {
                    if (vTime <= 0)
                    {
                        //vulnerable
                        GetComponent<Animator>().SetBool("invulnerable", false);
                        GetComponent<Animator>().SetBool("attack2", true);
                        GetComponent<Boss>().canBeHurt = true;

                        //points the weapon towards the player
                        Vector3 difference = player.transform.position - weapon.transform.position;
                        angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                        //shoots the player
                        if (weapon.GetComponent<WeaponEnemy>().canShootSecondStage())
                        {
                            weapon.GetComponent<WeaponEnemy>().Shoot();
                        }


                        if (vTime <= -invulnerableTime)
                        {
                            vTime = invulnerableTime;
                        }
                        else
                        {
                            vTime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        //invulnerable
                        GetComponent<Animator>().SetBool("invulnerable", true);
                        GetComponent<Animator>().SetBool("attack2", false);
                        GetComponent<Boss>().canBeHurt = false;
                        vTime -= Time.deltaTime;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
    }
}
