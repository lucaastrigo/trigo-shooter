using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : MonoBehaviour
{
    public float patrolRange;
    public float followRange;
    public float stopFollowRange;
    public bool follower;
    public bool followThrough;
    public bool retreat;
    public LayerMask hitableLayers;

    [HideInInspector] public float distToPlayer;
    float speedK;
    bool righted = true;
    bool patrolling;
    bool following;
    Animator anim;
    GameObject player;
    [HideInInspector] public Vector3 nextPatrolPoint;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(GetComponent<Enemy>() != null)
        {
            if (!GetComponent<Enemy>().dead)
            {
                distToPlayer = Vector2.Distance(transform.position, player.transform.position);

                RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position, hitableLayers);

                if (distToPlayer <= followRange)
                {
                    if (canFollow() && follower)
                    {
                        Follow();
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
                else
                {
                    if (!patrolling)
                    {
                        FindPatrolPoint();
                    }
                    else
                    {
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
    }

    void Patrol()
    {
        if(GetComponent<Enemy>() != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPatrolPoint, GetComponent<Enemy>().speed * Time.deltaTime * Global.globalSpeed);

            if (nextPatrolPoint.x < transform.position.x && GetComponent<Enemy>().speed > 0)
            {
                if (righted)
                {
                    Flip();
                }
            }

            if (nextPatrolPoint.x > transform.position.x && GetComponent<Enemy>().speed > 0)
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
            StartCoroutine(WaitPatrol(1));
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
        if(GetComponent<Enemy>() != null)
        {
            if (distToPlayer <= stopFollowRange)
            {
                if (followThrough)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, GetComponent<Enemy>().speed * Time.deltaTime * Global.globalSpeed);
                }
                else if (retreat)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -GetComponent<Enemy>().speed * Time.deltaTime * Global.globalSpeed);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, GetComponent<Enemy>().speed * Time.deltaTime * Global.globalSpeed);
            }

            if (player.transform.position.x < transform.position.x && GetComponent<Enemy>().speed > 0)
            {
                if (righted)
                {
                    Flip();
                }
            }

            if (player.transform.position.x > transform.position.x && GetComponent<Enemy>().speed > 0)
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

    public bool canFollow()
    {
        bool can = false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position, hitableLayers);
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
        if(GetComponent<Enemy>() != null)
        {
            speedK = GetComponent<Enemy>().speed;
            GetComponent<Enemy>().speed = 0;
            anim.SetBool("walk", false);
            yield return new WaitForSeconds(waitTime);
            GetComponent<Enemy>().speed = speedK;
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
