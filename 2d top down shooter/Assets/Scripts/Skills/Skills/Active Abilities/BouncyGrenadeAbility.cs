using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyGrenadeAbility : MonoBehaviour
{
    public float cooldown;
    public GameObject explosionGift;
    public Transform muzzle;

    float cooltime, angle;
    bool can;
    GameObject player;
    Skill skill;

    private void Start()
    {
        skill = GetComponent<Skill>();
    }

    void Update()
    {
        skill.cooldown = cooltime;
        skill.maxCooldown = cooldown;

        if (cooltime <= 0)
        {
            cooltime = 0;
            can = true;
        }
        else
        {
            cooltime -= Time.deltaTime;
            can = false;
        }

        if (player != null)
        {
            muzzle.position = player.transform.position;
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - muzzle.position;
            angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            muzzle.rotation = Quaternion.Euler(0, 0, angle - 90);

            if (skill.skillOn)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (can)
                    {
                        Instantiate(explosionGift, player.transform.position, muzzle.rotation);
                        cooltime = cooldown;
                    }
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
