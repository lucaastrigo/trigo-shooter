using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBoss : MonoBehaviour
{
    public float speed;
    public float patrolRange;
    public float followRange;
    public float stopFollowRange;
    public bool followThrough, retreat, shooter;
    public LayerMask hitableLayers;

    float distToPlayer;
    [HideInInspector] public float speedK;
    float time, _frostTime;
    bool righted = true;
    bool patrolling;
    bool following;
    [HideInInspector] public bool frost;
    Animator anim;
    GameObject player;
    Boss boss;
    [HideInInspector] public Vector3 nextPatrolPoint;

    void Start()
    {
        anim = GetComponent<Animator>();
        boss = GetComponent<Boss>();
        speedK = speed;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (GetComponent<Boss>() != null)
        {
            if (!GetComponent<Boss>().dead)
            {
                if (!GetComponent<Boss>().secondStage)
                {
                    distToPlayer = Vector2.Distance(transform.position, player.transform.position);
                    if (distToPlayer <= followRange)
                    {
                        if (canFollow())
                        {
                            Follow();
                        }
                    }
                    else
                    {
                        if (!patrolling)
                        {
                            FindPatrolPoint();
                        }

                        if (canPatrol())
                        {
                            Patrol();
                        }
                        else
                        {
                            FindPatrolPoint();
                        }
                    }
                }
            }
        }

        /*
        if(time <= 0)
        {
            anim.SetBool("walk", true);
            speed = speedK;
        }
        else
        {
            anim.SetBool("walk", false);
            speed = 0;
            time -= Time.deltaTime;
        }
        */


        //frost time
        if (_frostTime <= 0)
        {
            _frostTime = 0;
            if (frost)
            {
                Unfrost();
            }
        }
        else
        {
            _frostTime -= Time.deltaTime;
        }

        if (frost)
        {
            if (speed <= 0)
            {
                speed = 0;
            }
        }
    }

    public void Frost(float frostT, float frostP) //color time and freeze percent
    {
        _frostTime = frostT;

        frostP *= speedK;
        speed -= frostP;

        frost = true;
    }

    void Unfrost()
    {
        speed = speedK;
        frost = false;
    }

    public void Shock(float dazedTime)
    {
        anim.SetTrigger("shock");
        time = dazedTime;
    }

    void Patrol()
    {
        if (GetComponent<Boss>() != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPatrolPoint, speed * Time.deltaTime * Global.globalSpeed);

            if (nextPatrolPoint.x < transform.position.x && speed > 0)
            {
                if (righted)
                {
                    Flip();
                }
            }

            if (nextPatrolPoint.x > transform.position.x && speed > 0)
            {
                if (!righted)
                {
                    Flip();
                }
            }
        }

        float distanceToPoint = Vector2.Distance(nextPatrolPoint, transform.position);
        if (distanceToPoint <= 0.15f)
        {
            time = 0.5f;
            FindPatrolPoint();
        }
    }

    public void FindPatrolPoint()
    {
        float patrolX = Random.Range(transform.position.x - patrolRange, transform.position.x + patrolRange);
        float patrolY = Random.Range(transform.position.y - patrolRange, transform.position.y + patrolRange);

        nextPatrolPoint = new Vector2(patrolX, patrolY);

        patrolling = true;
    }

    void Follow()
    {
        if (GetComponent<Boss>() != null)
        {
            if (distToPlayer <= stopFollowRange)
            {
                if (followThrough)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime * Global.globalSpeed);
                }

                if (retreat)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -speed * Time.deltaTime * Global.globalSpeed);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime * Global.globalSpeed);
            }

            if (player.transform.position.x < transform.position.x && speed > 0)
            {
                if (righted)
                {
                    Flip();
                }
            }

            if (player.transform.position.x > transform.position.x && speed > 0)
            {
                if (!righted)
                {
                    Flip();
                }
            }
        }
    }

    bool canPatrol()
    {
        bool can = false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, nextPatrolPoint, hitableLayers);
        if (hit.collider)
        {
            can = false;
        }
        else
        {
            can = true;
        }

        return can;
    }

    bool canFollow()
    {
        bool can = false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, nextPatrolPoint, hitableLayers);
        if (hit.collider)
        {
            can = false;
        }
        else
        {
            can = true;
        }

        return can;
    }

    public IEnumerator WaitPatrol(float waitTime)
    {
        if (GetComponent<Boss>() != null)
        {
            speedK = GetComponent<Enemy>().speed;
            speed = 0;
            anim.SetBool("walk", false);
            yield return new WaitForSeconds(waitTime);
            speed = speedK;
            anim.SetBool("walk", true);
        }
    }

    void Flip()
    {
        righted = !righted;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stopFollowRange);
    }
}
