using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafDashAbility : MonoBehaviour
{
    public int leafDashDamage;
    public float dashSpeed;
    public float dashCooldown;
    public float timeBtw;
    public LayerMask hitLayer;
    public LayerMask enemiesLayer;
    public Vector2 leafRange;
    public GameObject leafFX;
    public GameObject dashFX;

    float dashTime;
    bool canDash;
    Vector2 mousePos;
    GameObject player;
    Camera cam;
    Skill skill;

    void Start()
    {
        dashTime = dashCooldown;

        skill = GetComponent<Skill>();
    }

    void Update()
    {
        skill.cooldown = dashTime;
        skill.maxCooldown = dashCooldown;

        if (player != null)
        {
            cam = Camera.main;

            if (skill.skillOn)
            {
                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

                if (dashTime <= 0)
                {
                    canDash = true;
                }
                else
                {
                    canDash = false;
                    dashTime -= Time.deltaTime;
                }

                if (canDash)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        if (!player.GetComponent<PlayerMovement>().moving)
                        {
                            RaycastHit2D dashHit = Physics2D.Raycast(player.transform.position, mousePos, dashSpeed, hitLayer);

                            if (dashHit.collider)
                            {
                                DashToMouse(dashHit.point);
                            }
                            else
                            {
                                DashToMouse(mousePos);
                            }
                        }
                        else
                        {
                            RaycastHit2D dashHit = Physics2D.Raycast(player.transform.position, mousePos, dashSpeed, hitLayer);

                            if (dashHit.collider)
                            {
                                DashToKey(dashHit.distance);
                            }
                            else
                            {
                                DashToKey(dashSpeed);
                            }
                        }
                    }
                }
            }
            else
            {
                canDash = false;
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void DashToMouse(Vector2 dashPos)
    {
        //player.GetComponent<Animator>().SetTrigger("leaf dash");

        Instantiate(leafFX, player.transform.position, Quaternion.identity);

        Collider2D[] enemiesToDamage1 = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(leafRange.x, leafRange.y), 0, enemiesLayer);

        for (int i = 0; i < enemiesToDamage1.Length; i++)
        {
            if (enemiesToDamage1[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage1[i].GetComponent<Enemy>().TakeColor(leafDashDamage, Color.green, 0.2f);
            }
        }

        player.transform.position = Vector2.MoveTowards(player.transform.position, dashPos, dashSpeed);

        Collider2D[] enemiesToDamage2 = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(leafRange.x, leafRange.y), 0, enemiesLayer);

        for (int i = 0; i < enemiesToDamage2.Length; i++)
        {
            if (enemiesToDamage2[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage2[i].GetComponent<Enemy>().TakeColor(leafDashDamage, Color.green, 0.2f);
            }
        }

        Instantiate(dashFX, player.transform.position, Quaternion.identity);
        dashTime = dashCooldown;
    }

    void DashToKey(float dashDist)
    {
        //player.GetComponent<Animator>().SetTrigger("leaf dash");

        Instantiate(leafFX, player.transform.position, Quaternion.identity);

        Collider2D[] enemiesToDamage1 = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(leafRange.x, leafRange.y), 0, enemiesLayer);

        for (int i = 0; i < enemiesToDamage1.Length; i++)
        {
            if (enemiesToDamage1[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage1[i].GetComponent<Enemy>().TakeColor(leafDashDamage, Color.green, 0.2f);
            }
        }

        player.transform.position = Vector2.MoveTowards(player.transform.position, player.GetComponent<PlayerMovement>().dashDirection, dashDist);

        Collider2D[] enemiesToDamage2 = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(leafRange.x, leafRange.y), 0, enemiesLayer);

        for (int i = 0; i < enemiesToDamage2.Length; i++)
        {
            if (enemiesToDamage2[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage2[i].GetComponent<Enemy>().TakeColor(leafDashDamage, Color.green, 0.2f);
            }
        }

        Instantiate(dashFX, player.transform.position, Quaternion.identity);
        dashTime = dashCooldown;
    }
}
